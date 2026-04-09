using Api.Dtos;
using Api.Servicios;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controladores;

[ApiController]
[Authorize]
[Route("api/clientes")]
public sealed class ClientesController : ControllerBase
{
    [HttpGet("empresa/{empresaId:guid}")]
    public async Task<IActionResult> Listar(
        [FromServices] IServicioClientes servicio,
        Guid empresaId,
        [FromQuery] string? busqueda,
        [FromQuery] string? estado,
        [FromQuery] bool? esVip,
        CancellationToken ct)
    {
        var respuesta = await servicio.ListarAsync(empresaId, busqueda, estado, esVip, ct);
        return Ok(respuesta);
    }

    [HttpGet("{clienteId:guid}")]
    public async Task<IActionResult> Obtener([FromServices] IServicioClientes servicio, Guid clienteId, CancellationToken ct)
    {
        var respuesta = await servicio.ObtenerAsync(clienteId, ct);
        return respuesta is null ? NotFound(new { mensaje = "Cliente no encontrado" }) : Ok(respuesta);
    }

    [HttpPost]
    public async Task<IActionResult> Crear([FromServices] IServicioClientes servicio, [FromBody] CrearClienteRequest request, CancellationToken ct)
    {
        var usuarioActual = User.Identity?.Name ?? "sistema";
        var respuesta = await servicio.CrearAsync(request, usuarioActual, ct);
        return CreatedAtAction(nameof(Obtener), new { clienteId = respuesta.Id }, respuesta);
    }

    [HttpPut("{clienteId:guid}")]
    public async Task<IActionResult> Actualizar([FromServices] IServicioClientes servicio, Guid clienteId, [FromBody] ActualizarClienteRequest request, CancellationToken ct)
    {
        var usuarioActual = User.Identity?.Name ?? "sistema";
        var respuesta = await servicio.ActualizarAsync(clienteId, request, usuarioActual, ct);
        return respuesta is null ? NotFound(new { mensaje = "Cliente no encontrado" }) : Ok(respuesta);
    }

    [Authorize(Policy = "permiso:clientes.eliminar")]
    [HttpDelete("{clienteId:guid}")]
    public async Task<IActionResult> Eliminar([FromServices] IServicioClientes servicio, Guid clienteId, CancellationToken ct)
    {
        var usuarioActual = User.Identity?.Name ?? "sistema";
        var ok = await servicio.BajaLogicaAsync(clienteId, usuarioActual, ct);
        return ok ? NoContent() : NotFound(new { mensaje = "Cliente no encontrado" });
    }
}
