using Api.Datos;
using Api.Modulos.Gimnasio;
using Api.Servicios;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Api.UnitTests;

public class WebhooksYGimnasioTests
{
    [Fact]
    public async Task Procesador_webhooks_mueve_pendientes_a_procesado()
    {
        var opciones = new DbContextOptionsBuilder<ContextoWellness>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        await using var db = new ContextoWellness(opciones);
        db.EventosWebhookExternos.Add(new EventoWebhookExterno
        {
            Proveedor = "pedidosya",
            IdempotenciaKey = Guid.NewGuid().ToString("N"),
            TipoEvento = "pedido",
            EstadoProcesamiento = "pendiente",
            CuerpoJson = "{}",
            CreadoPor = "test"
        });
        await db.SaveChangesAsync();

        var servicio = new ServicioProcesadorWebhooks(db);
        await servicio.ProcesarPendientesAsync(CancellationToken.None);

        Assert.Equal("procesado", db.EventosWebhookExternos.First().EstadoProcesamiento);
    }

    [Fact]
    public void Servicio_gimnasio_genera_qr()
    {
        var opciones = new DbContextOptionsBuilder<ContextoWellness>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        using var db = new ContextoWellness(opciones);
        var config = new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string?>
        {
            ["Qr:ClaveSecreta"] = "clave-super-segura"
        }).Build();

        var servicio = new ServicioGimnasio(db, config);
        var qr = servicio.GenerarQrAcceso(Guid.NewGuid(), DateTime.UtcNow.AddMinutes(2));

        Assert.False(string.IsNullOrWhiteSpace(qr));
    }
}
