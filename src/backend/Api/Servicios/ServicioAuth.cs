using Api.Datos;
using Api.Dtos;
using Comunes.Aplicacion;
using Microsoft.EntityFrameworkCore;

namespace Api.Servicios;

public interface IServicioAuth
{
    Task<Resultado<AuthResponse>> LoginAsync(LoginRequest request, CancellationToken ct);
}

public sealed class ServicioAuth : IServicioAuth
{
    private readonly ContextoWellness _contexto;

    public ServicioAuth(ContextoWellness contexto)
    {
        _contexto = contexto;
    }

    public async Task<Resultado<AuthResponse>> LoginAsync(LoginRequest request, CancellationToken ct)
    {
        var usuario = await _contexto.Usuarios.FirstOrDefaultAsync(x => x.Email == request.Email, ct);
        if (usuario is null)
        {
            return Resultado<AuthResponse>.Fallo("Credenciales invalidas");
        }

        if (usuario.BloqueadoTemporalmente)
        {
            return Resultado<AuthResponse>.Fallo("Usuario bloqueado temporalmente");
        }

        if (request.Clave != "Admin123*")
        {
            usuario.IntentosFallidos += 1;
            usuario.BloqueadoTemporalmente = usuario.IntentosFallidos >= 5;
            await _contexto.SaveChangesAsync(ct);
            return Resultado<AuthResponse>.Fallo("Credenciales invalidas");
        }

        usuario.IntentosFallidos = 0;
        await _contexto.SaveChangesAsync(ct);

        var token = Convert.ToBase64String(Guid.NewGuid().ToByteArray());
        var refresh = Convert.ToBase64String(Guid.NewGuid().ToByteArray());

        return Resultado<AuthResponse>.Ok(new AuthResponse(token, refresh, DateTime.UtcNow.AddHours(2)));
    }
}
