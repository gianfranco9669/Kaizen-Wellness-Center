using Api.Dtos;
using Api.Servicios;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controladores;

[ApiController]
[Route("api/seguridad/auth")]
public sealed class AuthController : ControllerBase
{
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromServices] IServicioAuth servicio, [FromBody] LoginRequest request, CancellationToken ct)
    {
        var resultado = await servicio.LoginAsync(request, ct);
        return resultado.Exitoso ? Ok(resultado.Valor) : Unauthorized(new { mensaje = resultado.Error });
    }
}
