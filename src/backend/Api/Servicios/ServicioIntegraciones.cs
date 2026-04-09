namespace Api.Servicios;

public interface IClienteExternoPedidosYa
{
    Task<string> EnviarPedidoAsync(string payload, CancellationToken ct);
}

public sealed class ClienteExternoPedidosYaMock : IClienteExternoPedidosYa
{
    public Task<string> EnviarPedidoAsync(string payload, CancellationToken ct)
        => Task.FromResult("mock-ok");
}
