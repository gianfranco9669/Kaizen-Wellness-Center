using Api.Datos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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

    [HttpGet("pedidos/{sedeId:guid}")]
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
