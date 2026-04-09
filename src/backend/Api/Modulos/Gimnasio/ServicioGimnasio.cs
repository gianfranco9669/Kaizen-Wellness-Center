using System.Security.Cryptography;
using System.Text;
using Api.Datos;
using Microsoft.EntityFrameworkCore;

namespace Api.Modulos.Gimnasio;

public sealed record CrearSocioDto(Guid ClienteId, string NumeroSocio, DateTime FechaVencimientoMembresiaUtc);
public sealed record CrearMembresiaDto(Guid SocioId, string PlanNombre, decimal Precio, DateTime FechaInicioUtc, DateTime FechaFinUtc);

public interface IServicioGimnasio
{
    Task<SocioGimnasio> CrearSocioAsync(CrearSocioDto request, string usuario, CancellationToken ct);
    Task<Membresia> CrearMembresiaAsync(CrearMembresiaDto request, string usuario, CancellationToken ct);
    Task<bool> RenovarMembresiaAsync(Guid socioId, int meses, string usuario, CancellationToken ct);
    string GenerarQrAcceso(Guid socioId, DateTime expiraUtc);
}

public sealed class ServicioGimnasio : IServicioGimnasio
{
    private readonly ContextoWellness _contexto;
    private readonly IConfiguration _configuration;

    public ServicioGimnasio(ContextoWellness contexto, IConfiguration configuration)
    {
        _contexto = contexto;
        _configuration = configuration;
    }

    public async Task<SocioGimnasio> CrearSocioAsync(CrearSocioDto request, string usuario, CancellationToken ct)
    {
        var socio = new SocioGimnasio
        {
            ClienteId = request.ClienteId,
            NumeroSocio = request.NumeroSocio,
            EstadoAcceso = "habilitado",
            FechaVencimientoMembresiaUtc = request.FechaVencimientoMembresiaUtc,
            CreadoPor = usuario
        };
        _contexto.SociosGimnasio.Add(socio);
        await _contexto.SaveChangesAsync(ct);
        return socio;
    }

    public async Task<Membresia> CrearMembresiaAsync(CrearMembresiaDto request, string usuario, CancellationToken ct)
    {
        var membresia = new Membresia
        {
            SocioGimnasioId = request.SocioId,
            PlanNombre = request.PlanNombre,
            Precio = request.Precio,
            FechaInicioUtc = request.FechaInicioUtc,
            FechaFinUtc = request.FechaFinUtc,
            Estado = "vigente",
            CreadoPor = usuario
        };
        _contexto.Membresias.Add(membresia);
        var socio = await _contexto.SociosGimnasio.FirstAsync(x => x.Id == request.SocioId, ct);
        socio.FechaVencimientoMembresiaUtc = request.FechaFinUtc;
        await _contexto.SaveChangesAsync(ct);
        return membresia;
    }

    public async Task<bool> RenovarMembresiaAsync(Guid socioId, int meses, string usuario, CancellationToken ct)
    {
        var socio = await _contexto.SociosGimnasio.FirstOrDefaultAsync(x => x.Id == socioId, ct);
        if (socio is null) return false;

        var inicio = socio.FechaVencimientoMembresiaUtc > DateTime.UtcNow ? socio.FechaVencimientoMembresiaUtc : DateTime.UtcNow;
        var fin = inicio.AddMonths(meses);
        await CrearMembresiaAsync(new CrearMembresiaDto(socioId, "Renovacion", 0, inicio, fin), usuario, ct);
        socio.EstadoAcceso = "habilitado";
        await _contexto.SaveChangesAsync(ct);
        return true;
    }

    public string GenerarQrAcceso(Guid socioId, DateTime expiraUtc)
    {
        var secreto = _configuration["Qr:ClaveSecreta"] ?? throw new InvalidOperationException("Qr:ClaveSecreta no configurada");
        var payload = $"{socioId}|{expiraUtc:O}";
        using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(secreto));
        var firma = Convert.ToHexString(hmac.ComputeHash(Encoding.UTF8.GetBytes(payload)));
        return Convert.ToBase64String(Encoding.UTF8.GetBytes($"{payload}|{firma}"));
    }
}
