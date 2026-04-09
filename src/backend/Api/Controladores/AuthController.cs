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
    [HttpPost("revocar-sesiones")]
    public async Task<IActionResult> RevocarSesiones([FromServices] IServicioAuth servicio, CancellationToken ct)
    {
        var usuarioId = User.Claims.FirstOrDefault(c => c.Type == "sub")?.Value;
        if (!Guid.TryParse(usuarioId, out var idUsuario)) return BadRequest(new { mensaje = "Usuario invalido" });
        await servicio.RevocarSesionesUsuarioAsync(idUsuario, ct);
        return NoContent();
    }

    [AllowAnonymous]
    [HttpPost("solicitar-recupero-clave")]
    public async Task<IActionResult> SolicitarRecuperoClave([FromServices] IServicioAuth servicio, [FromBody] SolicitarRecuperoClaveRequest request, CancellationToken ct)
    {
        await servicio.SolicitarRecuperoClaveAsync(request.Email, ct);
        return Accepted(new { mensaje = "Si el usuario existe recibira instrucciones" });
    }

    [AllowAnonymous]
    [HttpPost("confirmar-recupero-clave")]
    public async Task<IActionResult> ConfirmarRecuperoClave([FromServices] IServicioAuth servicio, [FromBody] ConfirmarRecuperoClaveRequest request, CancellationToken ct)
    {
        var ok = await servicio.ConfirmarRecuperoClaveAsync(request.Token, request.NuevaClave, ct);
        return ok ? Ok(new { mensaje = "Clave actualizada" }) : BadRequest(new { mensaje = "Token invalido o expirado" });
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
