using Api.Datos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Api.Modulos.Integraciones;

[ApiController]
[AllowAnonymous]
[Route("api/integraciones/webhooks")]
public sealed class WebhooksController : ControllerBase
{
    [HttpPost("{proveedor}")]
    public async Task<IActionResult> Recibir([FromServices] ContextoWellness contexto, string proveedor, [FromBody] object evento, CancellationToken ct)
    {
        var idempotencia = Request.Headers["X-Idempotencia-Key"].FirstOrDefault() ?? Guid.NewGuid().ToString("N");
        var existe = await contexto.EventosWebhookExternos.AnyAsync(x => x.IdempotenciaKey == idempotencia, ct);
        if (existe)
        {
            return Ok(new { mensaje = "evento_duplicado" });
        }

        contexto.EventosWebhookExternos.Add(new EventoWebhookExterno
        {
            Proveedor = proveedor,
            TipoEvento = Request.Headers["X-Tipo-Evento"].FirstOrDefault() ?? "desconocido",
            IdempotenciaKey = idempotencia,
            EstadoProcesamiento = "pendiente",
            CuerpoJson = System.Text.Json.JsonSerializer.Serialize(evento),
            CreadoPor = $"webhook-{proveedor}"
        });

        await contexto.SaveChangesAsync(ct);
        return Accepted(new { mensaje = "evento_recibido" });
    }
}
