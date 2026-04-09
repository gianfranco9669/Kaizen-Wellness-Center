namespace Comunes.Dominio;

public abstract class EntidadBase
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public DateTime FechaCreacionUtc { get; set; } = DateTime.UtcNow;
    public DateTime? FechaActualizacionUtc { get; set; }
    public bool EliminadoLogico { get; set; }
    public string CreadoPor { get; set; } = "sistema";
    public string? ActualizadoPor { get; set; }
}
