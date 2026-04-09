using Api.Dtos;
using Api.Servicios;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controladores;

[ApiController]
[Route("api/seguridad/auth")]
public sealed class AuthController : ControllerBase
{
    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromServices] IServicioAuth servicio, [FromBody] LoginRequest request, CancellationToken ct)
    {
        var resultado = await servicio.LoginAsync(request, HttpContext.Connection.RemoteIpAddress?.ToString(), ct);
        return resultado.Exitoso ? Ok(resultado.Valor) : Unauthorized(new { mensaje = resultado.Error });
    }

    [AllowAnonymous]
    [HttpPost("refresh-token")]
    public async Task<IActionResult> RefreshToken([FromServices] IServicioAuth servicio, [FromBody] RefreshTokenRequest request, CancellationToken ct)
    {
        var resultado = await servicio.RenovarTokenAsync(request.RefreshToken, HttpContext.Connection.RemoteIpAddress?.ToString(), ct);
        return resultado.Exitoso ? Ok(resultado.Valor) : Unauthorized(new { mensaje = resultado.Error });
    }

    [Authorize]
    [HttpPost("logout")]
    public async Task<IActionResult> Logout([FromServices] IServicioAuth servicio, [FromBody] RefreshTokenRequest request, CancellationToken ct)
    {
        await servicio.CerrarSesionAsync(request.RefreshToken, ct);
        return NoContent();
    }

    [Authorize]
    [HttpGet("me")]
    public IActionResult Me() => Ok(new
    {
        nombre = User.Identity?.Name,
        usuarioId = User.Claims.FirstOrDefault(c => c.Type == "sub")?.Value,
        empresaId = User.Claims.FirstOrDefault(c => c.Type == "empresa_id")?.Value,
        roles = User.Claims.Where(c => c.Type.EndsWith("role")).Select(c => c.Value)
    });
}
