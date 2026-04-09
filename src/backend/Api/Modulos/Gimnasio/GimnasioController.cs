using System.Text;
using System.Security.Cryptography;
using Api.Datos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Api.Modulos.Gimnasio;

[ApiController]
[Authorize]
[Route("api/gimnasio")]
public sealed class GimnasioController : ControllerBase
{
    [HttpGet("socios")]
    public async Task<IActionResult> ListarSocios([FromServices] ContextoWellness contexto, CancellationToken ct)
    {
        var socios = await contexto.SociosGimnasio
            .Select(x => new { x.Id, x.NumeroSocio, x.EstadoAcceso, x.FechaVencimientoMembresiaUtc })
            .ToListAsync(ct);
        return Ok(socios);
    }

    [HttpPost("socios")]
    public async Task<IActionResult> CrearSocio([FromServices] IServicioGimnasio servicio, [FromBody] CrearSocioDto request, CancellationToken ct)
    {
        var socio = await servicio.CrearSocioAsync(request, User.Identity?.Name ?? "sistema", ct);
        return CreatedAtAction(nameof(ObtenerSocio), new { socioId = socio.Id }, socio);
    }

    [HttpGet("socios/{socioId:guid}")]
    public async Task<IActionResult> ObtenerSocio([FromServices] ContextoWellness contexto, Guid socioId, CancellationToken ct)
    {
        var socio = await contexto.SociosGimnasio.FirstOrDefaultAsync(x => x.Id == socioId, ct);
        return socio is null ? NotFound() : Ok(socio);
    }

    [HttpPost("socios/{socioId:guid}/renovar/{meses:int}")]
    public async Task<IActionResult> Renovar([FromServices] IServicioGimnasio servicio, Guid socioId, int meses, CancellationToken ct)
    {
        var ok = await servicio.RenovarMembresiaAsync(socioId, meses, User.Identity?.Name ?? "sistema", ct);
        return ok ? NoContent() : NotFound();
    }

    [HttpPost("membresias")]
    public async Task<IActionResult> CrearMembresia([FromServices] IServicioGimnasio servicio, [FromBody] CrearMembresiaDto request, CancellationToken ct)
    {
        var membresia = await servicio.CrearMembresiaAsync(request, User.Identity?.Name ?? "sistema", ct);
        return Ok(membresia);
    }

    [HttpGet("socios/{socioId:guid}/qr")]
    public IActionResult GenerarQr([FromServices] IServicioGimnasio servicio, Guid socioId)
    {
        var expira = DateTime.UtcNow.AddMinutes(3);
        var qrToken = servicio.GenerarQrAcceso(socioId, expira);
        return Ok(new { qrToken, expiraUtc = expira });
    }

    [HttpPost("acceso/{socioId:guid}")]
    public async Task<IActionResult> RegistrarCheckin([FromServices] ContextoWellness contexto, [FromServices] IConfiguration config, Guid socioId, [FromQuery] string? qrToken, CancellationToken ct)
    {
        var socio = await contexto.SociosGimnasio.FirstOrDefaultAsync(x => x.Id == socioId, ct);
        if (socio is null)
            return NotFound(new { mensaje = "Socio no encontrado" });

        bool qrValido = true;
        if (!string.IsNullOrWhiteSpace(qrToken))
        {
            qrValido = ValidarQr(qrToken, config["Qr:ClaveSecreta"] ?? string.Empty, socioId);
        }

        var permitido = qrValido && socio.EstadoAcceso == "habilitado" && socio.FechaVencimientoMembresiaUtc >= DateTime.UtcNow;
        contexto.HistorialAccesosGimnasio.Add(new HistorialAccesoGimnasio
        {
            SocioGimnasioId = socioId,
            FechaAccesoUtc = DateTime.UtcNow,
            TipoAcceso = "check-in",
            Resultado = permitido ? "permitido" : "bloqueado",
            CreadoPor = User.Identity?.Name ?? "sistema"
        });

        await contexto.SaveChangesAsync(ct);
        return Ok(new { permitido, mensaje = permitido ? "Acceso habilitado" : "Acceso bloqueado" });
    }

    [HttpGet("socios/{socioId:guid}/historial-accesos")]
    public async Task<IActionResult> HistorialAccesos([FromServices] ContextoWellness contexto, Guid socioId, CancellationToken ct)
    {
        var historial = await contexto.HistorialAccesosGimnasio
            .Where(x => x.SocioGimnasioId == socioId)
            .OrderByDescending(x => x.FechaAccesoUtc)
            .Take(200)
            .ToListAsync(ct);
        return Ok(historial);
    }

    private static bool ValidarQr(string qrToken, string secreto, Guid socioId)
    {
        try
        {
            var decoded = Encoding.UTF8.GetString(Convert.FromBase64String(qrToken));
            var partes = decoded.Split('|');
            if (partes.Length != 3 || !Guid.TryParse(partes[0], out var socioEnQr) || socioEnQr != socioId) return false;
            var expira = DateTime.Parse(partes[1], null, System.Globalization.DateTimeStyles.RoundtripKind);
            if (expira < DateTime.UtcNow) return false;
            using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(secreto));
            var firma = Convert.ToHexString(hmac.ComputeHash(Encoding.UTF8.GetBytes($"{partes[0]}|{partes[1]}")));
            return string.Equals(firma, partes[2], StringComparison.OrdinalIgnoreCase);
        }
        catch
        {
            return false;
        }
    }
}
