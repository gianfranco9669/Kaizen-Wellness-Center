using Microsoft.AspNetCore.SignalR;

namespace Api.Hubs;

public sealed class CocinaHub : Hub
{
    public async Task PublicarCambioEstadoPedido(string pedidoId, string estado)
        => await Clients.All.SendAsync("pedido-actualizado", new { pedidoId, estado, fechaUtc = DateTime.UtcNow });
}
