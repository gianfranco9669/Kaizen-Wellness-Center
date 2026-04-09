using Api.Dtos;
using Api.Servicios;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controladores;

[ApiController]
[Route("api/clientes")]
public sealed class ClientesController : ControllerBase
{
    [HttpGet("empresa/{empresaId:guid}")]
    public async Task<IActionResult> Listar([FromServices] IServicioClientes servicio, Guid empresaId, CancellationToken ct)
    {
        var respuesta = await servicio.ListarAsync(empresaId, ct);
        return Ok(respuesta);
    }

    [HttpPost]
    public async Task<IActionResult> Crear([FromServices] IServicioClientes servicio, [FromBody] CrearClienteRequest request, CancellationToken ct)
    {
        var usuarioActual = User.Identity?.Name ?? "sistema";
        var respuesta = await servicio.CrearAsync(request, usuarioActual, ct);
        return CreatedAtAction(nameof(Listar), new { empresaId = request.EmpresaId }, respuesta);
    }
}
