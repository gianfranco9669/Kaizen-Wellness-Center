using Api.Configuracion;
using Api.Datos;
using Api.Dtos;
using Api.Seguridad;
using Comunes.Aplicacion;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Api.Servicios;

public interface IServicioAuth
{
    Task<Resultado<AuthResponse>> LoginAsync(LoginRequest request, string? ip, CancellationToken ct);
    Task<Resultado<AuthResponse>> RenovarTokenAsync(string refreshToken, string? ip, CancellationToken ct);
    Task CerrarSesionAsync(string refreshToken, CancellationToken ct);
    Task RevocarSesionesUsuarioAsync(Guid usuarioId, CancellationToken ct);
    Task SolicitarRecuperoClaveAsync(string email, CancellationToken ct);
    Task<bool> ConfirmarRecuperoClaveAsync(string token, string nuevaClave, CancellationToken ct);
}

public sealed class ServicioAuth : IServicioAuth
{
    private readonly ContextoWellness _contexto;
    private readonly IServicioHashClave _hash;
    private readonly IServicioTokenJwt _tokenJwt;
    private readonly OpcionesJwt _opcionesJwt;

    public ServicioAuth(ContextoWellness contexto, IServicioHashClave hash, IServicioTokenJwt tokenJwt, IOptions<OpcionesJwt> opcionesJwt)
    {
        _contexto = contexto;
        _hash = hash;
        _tokenJwt = tokenJwt;
        _opcionesJwt = opcionesJwt.Value;
    }

    public async Task<Resultado<AuthResponse>> LoginAsync(LoginRequest request, string? ip, CancellationToken ct)
    {
        var usuario = await _contexto.Usuarios
            .Include(x => x.Roles)
            .ThenInclude(x => x.RolSistema)
            .ThenInclude(x => x!.Permisos)
            .ThenInclude(x => x.PermisoSistema)
            .FirstOrDefaultAsync(x => x.Email == request.Email, ct);

        if (usuario is null || !usuario.Activo)
            return Resultado<AuthResponse>.Fallo("Credenciales invalidas");

        if (usuario.BloqueadoHastaUtc is not null && usuario.BloqueadoHastaUtc > DateTime.UtcNow)
            return Resultado<AuthResponse>.Fallo("Usuario bloqueado temporalmente");

        if (!_hash.Verificar(request.Clave, usuario.ClaveHash, usuario.ClaveSalt))
        {
            usuario.IntentosFallidos += 1;
            if (usuario.IntentosFallidos >= 5)
            {
                usuario.BloqueadoTemporalmente = true;
                usuario.BloqueadoHastaUtc = DateTime.UtcNow.AddMinutes(15);
                usuario.IntentosFallidos = 0;
            }
            await _contexto.SaveChangesAsync(ct);
            return Resultado<AuthResponse>.Fallo("Credenciales invalidas");
        }

        usuario.BloqueadoTemporalmente = false;
        usuario.BloqueadoHastaUtc = null;
        usuario.IntentosFallidos = 0;

        var roles = usuario.Roles.Select(x => x.RolSistema!.Codigo).Distinct().ToArray();
        var permisos = usuario.Roles.SelectMany(x => x.RolSistema!.Permisos).Select(x => x.PermisoSistema!.Codigo).Distinct().ToArray();

        var accessToken = _tokenJwt.CrearAccessToken(usuario, roles, permisos);
        var refreshTokenPlano = _tokenJwt.CrearRefreshToken();

        _contexto.RefreshTokensSesiones.Add(new RefreshTokenSesion
        {
            UsuarioSistemaId = usuario.Id,
            TokenHash = _hash.CalcularSha256(refreshTokenPlano),
            ExpiraUtc = DateTime.UtcNow.AddDays(_opcionesJwt.DiasExpiracionRefreshToken),
            IpCreacion = ip,
            CreadoPor = usuario.Email
        });

        await _contexto.SaveChangesAsync(ct);

        return Resultado<AuthResponse>.Ok(new AuthResponse(
            accessToken,
            refreshTokenPlano,
            DateTime.UtcNow.AddMinutes(_opcionesJwt.MinutosExpiracionAccessToken),
            usuario.NombreCompleto,
            roles,
            permisos));
    }

    public async Task<Resultado<AuthResponse>> RenovarTokenAsync(string refreshToken, string? ip, CancellationToken ct)
    {
        var hash = _hash.CalcularSha256(refreshToken);
        var sesion = await _contexto.RefreshTokensSesiones
            .Include(x => x.UsuarioSistema)
            .ThenInclude(x => x!.Roles)
            .ThenInclude(x => x.RolSistema)
            .ThenInclude(x => x!.Permisos)
            .ThenInclude(x => x.PermisoSistema)
            .FirstOrDefaultAsync(x => x.TokenHash == hash, ct);

        if (sesion is null || sesion.RevocadoUtc is not null || sesion.ExpiraUtc < DateTime.UtcNow)
            return Resultado<AuthResponse>.Fallo("Refresh token invalido");

        sesion.RevocadoUtc = DateTime.UtcNow;
        var usuario = sesion.UsuarioSistema!;
        var roles = usuario.Roles.Select(x => x.RolSistema!.Codigo).Distinct().ToArray();
        var permisos = usuario.Roles.SelectMany(x => x.RolSistema!.Permisos).Select(x => x.PermisoSistema!.Codigo).Distinct().ToArray();

        var accessToken = _tokenJwt.CrearAccessToken(usuario, roles, permisos);
        var nuevoRefreshPlano = _tokenJwt.CrearRefreshToken();
        _contexto.RefreshTokensSesiones.Add(new RefreshTokenSesion
        {
            UsuarioSistemaId = usuario.Id,
            TokenHash = _hash.CalcularSha256(nuevoRefreshPlano),
            ExpiraUtc = DateTime.UtcNow.AddDays(_opcionesJwt.DiasExpiracionRefreshToken),
            IpCreacion = ip,
            CreadoPor = usuario.Email
        });

        await _contexto.SaveChangesAsync(ct);
        return Resultado<AuthResponse>.Ok(new AuthResponse(accessToken, nuevoRefreshPlano,
            DateTime.UtcNow.AddMinutes(_opcionesJwt.MinutosExpiracionAccessToken), usuario.NombreCompleto, roles, permisos));
    }

    public async Task CerrarSesionAsync(string refreshToken, CancellationToken ct)
    {
        var hash = _hash.CalcularSha256(refreshToken);
        var sesion = await _contexto.RefreshTokensSesiones.FirstOrDefaultAsync(x => x.TokenHash == hash, ct);
        if (sesion is null) return;
        sesion.RevocadoUtc = DateTime.UtcNow;
        await _contexto.SaveChangesAsync(ct);
    }

    public async Task RevocarSesionesUsuarioAsync(Guid usuarioId, CancellationToken ct)
    {
        var sesiones = await _contexto.RefreshTokensSesiones.Where(x => x.UsuarioSistemaId == usuarioId && x.RevocadoUtc == null).ToListAsync(ct);
        foreach (var sesion in sesiones)
            sesion.RevocadoUtc = DateTime.UtcNow;
        await _contexto.SaveChangesAsync(ct);
    }

    public async Task SolicitarRecuperoClaveAsync(string email, CancellationToken ct)
    {
        var usuario = await _contexto.Usuarios.FirstOrDefaultAsync(x => x.Email == email && x.Activo, ct);
        if (usuario is null) return;

        var tokenPlano = _tokenJwt.CrearRefreshToken();
        _contexto.TokensRecuperoClaves.Add(new TokenRecuperoClave
        {
            UsuarioSistemaId = usuario.Id,
            TokenHash = _hash.CalcularSha256(tokenPlano),
            ExpiraUtc = DateTime.UtcNow.AddMinutes(30),
            CreadoPor = usuario.Email
        });

        _contexto.RegistrosAuditoria.Add(new RegistroAuditoria
        {
            Modulo = "seguridad",
            Accion = "solicitar-recupero-clave",
            Usuario = usuario.Email,
            Entidad = "usuario",
            DetalleJson = $"{{\"tokenRecupero\":\"{tokenPlano}\"}}"
        });

        await _contexto.SaveChangesAsync(ct);
    }

    public async Task<bool> ConfirmarRecuperoClaveAsync(string token, string nuevaClave, CancellationToken ct)
    {
        var hashToken = _hash.CalcularSha256(token);
        var recupero = await _contexto.TokensRecuperoClaves.FirstOrDefaultAsync(x => x.TokenHash == hashToken, ct);
        if (recupero is null || recupero.UsadoUtc != null || recupero.ExpiraUtc < DateTime.UtcNow)
            return false;

        var usuario = await _contexto.Usuarios.FirstAsync(x => x.Id == recupero.UsuarioSistemaId, ct);
        var (hash, salt) = _hash.GenerarHash(nuevaClave);
        usuario.ClaveHash = hash;
        usuario.ClaveSalt = salt;
        usuario.RequiereCambioClave = false;
        recupero.UsadoUtc = DateTime.UtcNow;

        await RevocarSesionesUsuarioAsync(usuario.Id, ct);
        await _contexto.SaveChangesAsync(ct);
        return true;
    }
}
