using Microsoft.AspNetCore.Mvc;

namespace Api.Controladores;

[ApiController]
[Route("api/salud")]
public sealed class SaludController : ControllerBase
{
    [HttpGet]
    public IActionResult Obtener() => Ok(new { estado = "ok", servicio = "kaizen-wellness-api", fechaUtc = DateTime.UtcNow });
}
