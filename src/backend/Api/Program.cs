using System.Security.Claims;
using System.Text;
using Api.Configuracion;
using Api.Datos;
using Api.Hubs;
using Api.Jobs;
using Api.Middleware;
using Api.Modulos.Gastronomia;
using Api.Modulos.Gimnasio;
using Api.Seguridad;
using Api.Servicios;
using Hangfire;
using Hangfire.PostgreSql;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<OpcionesJwt>(builder.Configuration.GetSection(OpcionesJwt.Seccion));
var opcionesJwt = builder.Configuration.GetSection(OpcionesJwt.Seccion).Get<OpcionesJwt>() ?? new OpcionesJwt();
if (string.IsNullOrWhiteSpace(opcionesJwt.ClaveSecreta) || opcionesJwt.ClaveSecreta.Length < 32)
{
    throw new InvalidOperationException("Jwt:ClaveSecreta debe estar configurada con al menos 32 caracteres");
}

var qrClaveSecreta = builder.Configuration["Qr:ClaveSecreta"];
if (string.IsNullOrWhiteSpace(qrClaveSecreta) || qrClaveSecreta.Length < 16)
{
    throw new InvalidOperationException("Qr:ClaveSecreta debe estar configurada");
}

var conexionPostgreSql = builder.Configuration.GetConnectionString("PostgreSql");
var conexionRedis = builder.Configuration.GetConnectionString("Redis");
if (string.IsNullOrWhiteSpace(conexionPostgreSql) || string.IsNullOrWhiteSpace(conexionRedis))
{
    throw new InvalidOperationException("ConnectionStrings PostgreSql y Redis deben configurarse por variables de entorno o secretos");
}

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSignalR();

builder.Services.AddDbContext<ContextoWellness>(options =>
    options.UseNpgsql(conexionPostgreSql));

builder.Services.AddSingleton<IConnectionMultiplexer>(_ =>
    ConnectionMultiplexer.Connect(conexionRedis));

builder.Services.AddScoped<IServicioAuth, ServicioAuth>();
builder.Services.AddScoped<IServicioClientes, ServicioClientes>();
builder.Services.AddScoped<IServicioHashClave, ServicioHashClave>();
builder.Services.AddScoped<IServicioTokenJwt, ServicioTokenJwt>();
builder.Services.AddScoped<IClienteExternoPedidosYa, ClienteExternoPedidosYa>();
builder.Services.AddScoped<IClienteExternoRappi, ClienteExternoRappi>();
builder.Services.AddScoped<IClienteExternoMercadoPago, ClienteExternoMercadoPago>();
builder.Services.AddScoped<JobSincronizacionIntegraciones>();
builder.Services.AddScoped<IServicioGastronomia, ServicioGastronomia>();
builder.Services.AddScoped<IServicioGimnasio, ServicioGimnasio>();
builder.Services.AddScoped<IServicioProcesadorWebhooks, ServicioProcesadorWebhooks>();

builder.Services.AddHttpClient("pedidosya", c => { var url = builder.Configuration["Integraciones:PedidosYa:BaseUrl"]; if (!string.IsNullOrWhiteSpace(url)) c.BaseAddress = new Uri(url); });
builder.Services.AddHttpClient("rappi", c => { var url = builder.Configuration["Integraciones:Rappi:BaseUrl"]; if (!string.IsNullOrWhiteSpace(url)) c.BaseAddress = new Uri(url); });
builder.Services.AddHttpClient("mercadopago", c => { var url = builder.Configuration["Integraciones:MercadoPago:BaseUrl"]; if (!string.IsNullOrWhiteSpace(url)) c.BaseAddress = new Uri(url); });

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = opcionesJwt.Issuer,
            ValidateAudience = true,
            ValidAudience = opcionesJwt.Audience,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(opcionesJwt.ClaveSecreta))
        };
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("permiso:clientes.eliminar", policy =>
        policy.RequireAssertion(ctx =>
            ctx.User.HasClaim(c => c.Type == "permiso" && c.Value == "clientes.eliminar") ||
            ctx.User.HasClaim(c => c.Type == ClaimTypes.Role && c.Value == "admin")));
});

builder.Services.AddHangfire(config =>
    config.UsePostgreSqlStorage(c => c.UseNpgsqlConnection(conexionPostgreSql)));
builder.Services.AddHangfireServer();

builder.Services.AddHealthChecks()
    .AddNpgSql(conexionPostgreSql)
    .AddRedis(conexionRedis);

builder.Services.AddCors(options =>
{
    var origenes = builder.Configuration.GetSection("Cors:OrigenesPermitidos").Get<string[]>() ?? ["http://localhost:4200"];
    options.AddPolicy("politica-front", policy => policy.WithOrigins(origenes).AllowAnyMethod().AllowAnyHeader().AllowCredentials());
});

builder.Services.AddRateLimiter(options =>
{
    options.AddFixedWindowLimiter("auth", opt =>
    {
        opt.Window = TimeSpan.FromMinutes(1);
        opt.PermitLimit = 30;
    });
});

var app = builder.Build();

app.UseMiddleware<MiddlewareErrores>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseRateLimiter();
app.UseCors("politica-front");
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapHub<CocinaHub>("/hubs/cocina");
app.MapHealthChecks("/health");
app.MapHangfireDashboard("/hangfire");

using (var scope = app.Services.CreateScope())
{
    var contexto = scope.ServiceProvider.GetRequiredService<ContextoWellness>();
    await contexto.Database.MigrateAsync();
    var sembrador = new SembradorInicial(contexto, scope.ServiceProvider.GetRequiredService<IServicioHashClave>());
    await sembrador.SemillarAsync();
}

RecurringJob.AddOrUpdate<JobSincronizacionIntegraciones>(
    "sincronizacion-integraciones",
    x => x.EjecutarAsync(),
    "*/5 * * * *");

app.Run();
