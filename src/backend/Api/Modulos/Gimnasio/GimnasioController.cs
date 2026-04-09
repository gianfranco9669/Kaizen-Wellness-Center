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

    [HttpPost("acceso/{socioId:guid}")]
    public async Task<IActionResult> RegistrarCheckin([FromServices] ContextoWellness contexto, Guid socioId, CancellationToken ct)
    {
        var socio = await contexto.SociosGimnasio.FirstOrDefaultAsync(x => x.Id == socioId, ct);
        if (socio is null)
        {
            return NotFound(new { mensaje = "Socio no encontrado" });
        }

        var permitido = socio.EstadoAcceso == "habilitado" && socio.FechaVencimientoMembresiaUtc >= DateTime.UtcNow;
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
}
