using System.Net.Http.Json;
using Api.Datos;
using Microsoft.EntityFrameworkCore;

namespace Api.Servicios;

public sealed record PedidoExternoDto(string IdInterno, decimal Total, string Canal, string ClienteNombre);

public interface IClienteExternoPedidosYa
{
    Task<string> EnviarPedidoAsync(PedidoExternoDto pedido, CancellationToken ct);
}

public interface IClienteExternoRappi
{
    Task<string> EnviarPedidoAsync(PedidoExternoDto pedido, CancellationToken ct);
}

public interface IClienteExternoMercadoPago
{
    Task<string> CrearCobroAsync(string referencia, decimal importe, CancellationToken ct);
}

public sealed class ClienteExternoPedidosYa : IClienteExternoPedidosYa
{
    private readonly IHttpClientFactory _httpClientFactory;
    public ClienteExternoPedidosYa(IHttpClientFactory httpClientFactory) => _httpClientFactory = httpClientFactory;

    public async Task<string> EnviarPedidoAsync(PedidoExternoDto pedido, CancellationToken ct)
    {
        var cliente = _httpClientFactory.CreateClient("pedidosya");
        if (string.IsNullOrWhiteSpace(cliente.BaseAddress?.ToString())) return "mock-pedidosya";
        var response = await cliente.PostAsJsonAsync("/pedidos", pedido, ct);
        return response.IsSuccessStatusCode ? "enviado" : $"error:{(int)response.StatusCode}";
    }
}

public sealed class ClienteExternoRappi : IClienteExternoRappi
{
    private readonly IHttpClientFactory _httpClientFactory;
    public ClienteExternoRappi(IHttpClientFactory httpClientFactory) => _httpClientFactory = httpClientFactory;

    public async Task<string> EnviarPedidoAsync(PedidoExternoDto pedido, CancellationToken ct)
    {
        var cliente = _httpClientFactory.CreateClient("rappi");
        if (string.IsNullOrWhiteSpace(cliente.BaseAddress?.ToString())) return "mock-rappi";
        var response = await cliente.PostAsJsonAsync("/ordenes", pedido, ct);
        return response.IsSuccessStatusCode ? "enviado" : $"error:{(int)response.StatusCode}";
    }
}

public sealed class ClienteExternoMercadoPago : IClienteExternoMercadoPago
{
    private readonly IHttpClientFactory _httpClientFactory;
    public ClienteExternoMercadoPago(IHttpClientFactory httpClientFactory) => _httpClientFactory = httpClientFactory;

    public async Task<string> CrearCobroAsync(string referencia, decimal importe, CancellationToken ct)
    {
        var cliente = _httpClientFactory.CreateClient("mercadopago");
        if (string.IsNullOrWhiteSpace(cliente.BaseAddress?.ToString())) return "mock-mercadopago";
        var response = await cliente.PostAsJsonAsync("/cobros", new { referencia, importe }, ct);
        return response.IsSuccessStatusCode ? "creado" : $"error:{(int)response.StatusCode}";
    }
}

public interface IServicioProcesadorWebhooks
{
    Task ProcesarPendientesAsync(CancellationToken ct);
}

public sealed class ServicioProcesadorWebhooks : IServicioProcesadorWebhooks
{
    private readonly ContextoWellness _contexto;

    public ServicioProcesadorWebhooks(ContextoWellness contexto)
    {
        _contexto = contexto;
    }

    public async Task ProcesarPendientesAsync(CancellationToken ct)
    {
        var eventos = await _contexto.EventosWebhookExternos
            .Where(x => x.EstadoProcesamiento == "pendiente" && x.Reintentos < 5)
            .OrderBy(x => x.FechaCreacionUtc)
            .Take(100)
            .ToListAsync(ct);

        foreach (var evento in eventos)
        {
            evento.EstadoProcesamiento = "procesado";
            evento.Reintentos += 1;
        }

        await _contexto.SaveChangesAsync(ct);
    }
}
