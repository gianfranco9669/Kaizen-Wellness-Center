using Api.Servicios;

namespace Api.Jobs;

public sealed class JobSincronizacionIntegraciones
{
    private readonly ILogger<JobSincronizacionIntegraciones> _logger;
    private readonly IServicioProcesadorWebhooks _procesador;

    public JobSincronizacionIntegraciones(ILogger<JobSincronizacionIntegraciones> logger, IServicioProcesadorWebhooks procesador)
    {
        _logger = logger;
        _procesador = procesador;
    }

    public async Task EjecutarAsync()
    {
        _logger.LogInformation("Ejecutando job de integraciones en {fecha}", DateTime.UtcNow);
        await _procesador.ProcesarPendientesAsync(CancellationToken.None);
    }
}
