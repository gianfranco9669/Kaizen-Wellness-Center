using Api.Datos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Api.Hubs;

namespace Api.Modulos.Gastronomia;

[ApiController]
[Authorize]
[Route("api/gastronomia")]
public sealed class GastronomiaController : ControllerBase
{
    [HttpGet("productos")]
    public async Task<IActionResult> ListarProductos([FromServices] ContextoWellness contexto, [FromQuery] Guid empresaId, CancellationToken ct)
    {
        var productos = await contexto.ProductosVenta.Where(x => x.EmpresaId == empresaId && x.Activo)
            .Select(x => new { x.Id, x.Codigo, x.Nombre, x.PrecioVenta, x.CostoTeorico })
            .ToListAsync(ct);
        return Ok(productos);
    }

    [HttpPost("pedidos")]
    public async Task<IActionResult> CrearPedido([FromServices] IServicioGastronomia servicio, [FromBody] CrearPedidoDto request, CancellationToken ct)
    {
        var pedido = await servicio.CrearPedidoAsync(request, User.Identity?.Name ?? "sistema", ct);
        return CreatedAtAction(nameof(ObtenerPedido), new { pedidoId = pedido.Id }, new { pedido.Id, pedido.Estado, pedido.Total });
    }

    [HttpGet("pedidos/{pedidoId:guid}")]
    public async Task<IActionResult> ObtenerPedido([FromServices] ContextoWellness contexto, Guid pedidoId, CancellationToken ct)
    {
        var pedido = await contexto.Pedidos.Include(x => x.Detalles).FirstOrDefaultAsync(x => x.Id == pedidoId, ct);
        return pedido is null ? NotFound() : Ok(pedido);
    }

    [HttpPut("pedidos/{pedidoId:guid}/estado/{estado}")]
    public async Task<IActionResult> CambiarEstado([FromServices] IServicioGastronomia servicio, [FromServices] IHubContext<CocinaHub> hub, Guid pedidoId, string estado, CancellationToken ct)
    {
        var ok = await servicio.CambiarEstadoPedidoAsync(pedidoId, estado, User.Identity?.Name ?? "sistema", ct);
        if (!ok) return NotFound(new { mensaje = "Pedido no encontrado" });
        await hub.Clients.All.SendAsync("pedido-actualizado", new { pedidoId, estado, fechaUtc = DateTime.UtcNow }, ct);
        return NoContent();
    }

    [HttpPost("pagos")]
    public async Task<IActionResult> RegistrarPago([FromServices] IServicioGastronomia servicio, [FromBody] RegistrarPagoDto request, CancellationToken ct)
    {
        var pago = await servicio.RegistrarPagoAsync(request, User.Identity?.Name ?? "sistema", ct);
        return Ok(pago);
    }

    [HttpGet("pedidos/sede/{sedeId:guid}")]
    public async Task<IActionResult> ListarPedidos([FromServices] ContextoWellness contexto, Guid sedeId, CancellationToken ct)
    {
        var pedidos = await contexto.Pedidos.Where(x => x.SedeId == sedeId)
            .OrderByDescending(x => x.FechaCreacionUtc)
            .Take(100)
            .Select(x => new { x.Id, x.Canal, x.Estado, x.Total, x.Observaciones })
            .ToListAsync(ct);
        return Ok(pedidos);
    }
}
