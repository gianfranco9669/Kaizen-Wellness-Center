using Api.Datos;
using Api.Hubs;
using Api.Jobs;
using Api.Servicios;
using Hangfire;
using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSignalR();

builder.Services.AddDbContext<ContextoWellness>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("PostgreSql")));

builder.Services.AddSingleton<IConnectionMultiplexer>(_ =>
    ConnectionMultiplexer.Connect(builder.Configuration.GetConnectionString("Redis") ?? "localhost:6379"));

builder.Services.AddScoped<IServicioAuth, ServicioAuth>();
builder.Services.AddScoped<IServicioClientes, ServicioClientes>();
builder.Services.AddScoped<IClienteExternoPedidosYa, ClienteExternoPedidosYaMock>();
builder.Services.AddScoped<JobSincronizacionIntegraciones>();

builder.Services.AddHangfire(config => config.UseSimpleAssemblyNameTypeSerializer());
builder.Services.AddHangfireServer();

builder.Services.AddHealthChecks()
    .AddNpgSql(builder.Configuration.GetConnectionString("PostgreSql")!)
    .AddRedis(builder.Configuration.GetConnectionString("Redis")!);

builder.Services.AddCors(options =>
{
    options.AddPolicy("politica-front", policy => policy
        .WithOrigins("http://localhost:4200")
        .AllowAnyMethod()
        .AllowAnyHeader()
        .AllowCredentials());
});

builder.Services.AddRateLimiter(options =>
{
    options.AddFixedWindowLimiter("auth", opt =>
    {
        opt.Window = TimeSpan.FromMinutes(1);
        opt.PermitLimit = 20;
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseRateLimiter();
app.UseCors("politica-front");
app.UseAuthorization();

app.MapControllers();
app.MapHub<CocinaHub>("/hubs/cocina");
app.MapHealthChecks("/health");

using (var scope = app.Services.CreateScope())
{
    var sembrador = new SembradorInicial(scope.ServiceProvider.GetRequiredService<ContextoWellness>());
    await sembrador.SemillarAsync();
}

RecurringJob.AddOrUpdate<JobSincronizacionIntegraciones>(
    "sincronizacion-integraciones",
    x => x.EjecutarAsync(),
    "*/10 * * * *");

app.Run();
