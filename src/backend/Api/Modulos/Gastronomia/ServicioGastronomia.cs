using Api.Datos;
using Microsoft.EntityFrameworkCore;

namespace Api.Modulos.Gastronomia;

public sealed record CrearPedidoItemDto(Guid ProductoVentaId, decimal Cantidad);
public sealed record CrearPedidoDto(Guid SedeId, Guid ClienteId, string Canal, string Observaciones, List<CrearPedidoItemDto> Items);
public sealed record RegistrarPagoDto(Guid PedidoId, string MedioPago, decimal Importe);

public interface IServicioGastronomia
{
    Task<Pedido> CrearPedidoAsync(CrearPedidoDto request, string usuario, CancellationToken ct);
    Task<bool> CambiarEstadoPedidoAsync(Guid pedidoId, string nuevoEstado, string usuario, CancellationToken ct);
    Task<Pago> RegistrarPagoAsync(RegistrarPagoDto request, string usuario, CancellationToken ct);
}

public sealed class ServicioGastronomia : IServicioGastronomia
{
    private readonly ContextoWellness _contexto;
    public ServicioGastronomia(ContextoWellness contexto) => _contexto = contexto;

    public async Task<Pedido> CrearPedidoAsync(CrearPedidoDto request, string usuario, CancellationToken ct)
    {
        var productos = await _contexto.ProductosVenta
            .Where(x => request.Items.Select(i => i.ProductoVentaId).Contains(x.Id) && x.Activo)
            .ToDictionaryAsync(x => x.Id, ct);

        var pedido = new Pedido
        {
            SedeId = request.SedeId,
            ClienteId = request.ClienteId,
            Canal = request.Canal,
            Estado = "pendiente",
            Observaciones = request.Observaciones,
            CreadoPor = usuario
        };

        decimal total = 0;
        foreach (var item in request.Items)
        {
            if (!productos.TryGetValue(item.ProductoVentaId, out var producto)) continue;
            total += producto.PrecioVenta * item.Cantidad;
            pedido.Detalles.Add(new DetallePedido
            {
                ProductoVentaId = item.ProductoVentaId,
                Cantidad = item.Cantidad,
                PrecioUnitario = producto.PrecioVenta,
                CreadoPor = usuario
            });

            _contexto.MovimientosStock.Add(new MovimientoStock
            {
                SedeId = request.SedeId,
                ProductoVentaId = item.ProductoVentaId,
                TipoMovimiento = "egreso-venta",
                Cantidad = item.Cantidad,
                Motivo = $"pedido-{pedido.Id}",
                CreadoPor = usuario
            });
        }

        pedido.Total = total;
        _contexto.Pedidos.Add(pedido);
        _contexto.RegistrosAuditoria.Add(new RegistroAuditoria
        {
            Modulo = "gastronomia",
            Accion = "crear-pedido",
            Usuario = usuario,
            Entidad = "pedido",
            DetalleJson = $"{{\"pedidoId\":\"{pedido.Id}\"}}"
        });

        await _contexto.SaveChangesAsync(ct);
        return pedido;
    }

    public async Task<bool> CambiarEstadoPedidoAsync(Guid pedidoId, string nuevoEstado, string usuario, CancellationToken ct)
    {
        var pedido = await _contexto.Pedidos.FirstOrDefaultAsync(x => x.Id == pedidoId, ct);
        if (pedido is null) return false;

        pedido.Estado = nuevoEstado;
        pedido.FechaActualizacionUtc = DateTime.UtcNow;
        pedido.ActualizadoPor = usuario;

        if (nuevoEstado == "cancelado")
        {
            var detalles = await _contexto.DetallesPedido.Where(x => x.PedidoId == pedidoId).ToListAsync(ct);
            foreach (var detalle in detalles)
            {
                _contexto.Mermas.Add(new Merma
                {
                    SedeId = pedido.SedeId,
                    ProductoVentaId = detalle.ProductoVentaId,
                    Cantidad = detalle.Cantidad,
                    Motivo = "cancelacion-pedido",
                    CreadoPor = usuario
                });
            }
        }

        await _contexto.SaveChangesAsync(ct);
        return true;
    }

    public async Task<Pago> RegistrarPagoAsync(RegistrarPagoDto request, string usuario, CancellationToken ct)
    {
        var pago = new Pago
        {
            PedidoId = request.PedidoId,
            MedioPago = request.MedioPago,
            Importe = request.Importe,
            Estado = "aprobado",
            CreadoPor = usuario
        };
        _contexto.Pagos.Add(pago);
        await _contexto.SaveChangesAsync(ct);
        return pago;
    }
}
