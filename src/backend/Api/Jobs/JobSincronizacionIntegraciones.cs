namespace Api.Jobs;

public sealed class JobSincronizacionIntegraciones
{
    private readonly ILogger<JobSincronizacionIntegraciones> _logger;

    public JobSincronizacionIntegraciones(ILogger<JobSincronizacionIntegraciones> logger)
    {
        _logger = logger;
    }

    public Task EjecutarAsync()
    {
        _logger.LogInformation("Ejecutando job de sincronizacion externa en {fecha}", DateTime.UtcNow);
        return Task.CompletedTask;
    }
}
