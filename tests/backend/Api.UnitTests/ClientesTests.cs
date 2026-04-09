using System.Threading.Tasks;
using Xunit;
using Api.Datos;
using Api.Dtos;
using Api.Servicios;
using Microsoft.EntityFrameworkCore;

namespace Api.UnitTests;

public class ClientesTests
{
    [Fact]
    public async Task Crear_y_baja_logica_cliente_funciona()
    {
        var opciones = new DbContextOptionsBuilder<ContextoWellness>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        await using var db = new ContextoWellness(opciones);
        var servicio = new ServicioClientes(db);

        var cliente = await servicio.CrearAsync(new CrearClienteRequest(Guid.NewGuid(), "Ana", "Ruiz", "123", "111", "ana@k.com", false, "", ""), "tester", CancellationToken.None);
        var borrado = await servicio.BajaLogicaAsync(cliente.Id, "tester", CancellationToken.None);

        Assert.True(borrado);
        var detalle = await servicio.ObtenerAsync(cliente.Id, CancellationToken.None);
        Assert.Null(detalle);
    }
}
