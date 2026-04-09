using Api.Datos;
using Api.Dtos;
using Microsoft.EntityFrameworkCore;

namespace Api.Servicios;

public interface IServicioClientes
{
    Task<ClienteResumenResponse> CrearAsync(CrearClienteRequest request, string usuarioActual, CancellationToken ct);
    Task<IReadOnlyCollection<ClienteResumenResponse>> ListarAsync(Guid empresaId, string? busqueda, string? estado, bool? esVip, CancellationToken ct);
    Task<ClienteDetalleResponse?> ObtenerAsync(Guid clienteId, CancellationToken ct);
    Task<ClienteDetalleResponse?> ActualizarAsync(Guid clienteId, ActualizarClienteRequest request, string usuarioActual, CancellationToken ct);
    Task<bool> BajaLogicaAsync(Guid clienteId, string usuarioActual, CancellationToken ct);
}

public sealed class ServicioClientes : IServicioClientes
{
    private readonly ContextoWellness _contexto;

    public ServicioClientes(ContextoWellness contexto)
    {
        _contexto = contexto;
    }

    public async Task<ClienteResumenResponse> CrearAsync(CrearClienteRequest request, string usuarioActual, CancellationToken ct)
    {
        var cliente = new Cliente
        {
            EmpresaId = request.EmpresaId,
            CodigoCliente = $"CLI-{DateTime.UtcNow:yyyyMMddHHmmssfff}",
            Nombres = request.Nombres.Trim(),
            Apellidos = request.Apellidos.Trim(),
            Documento = request.Documento.Trim(),
            Telefono = request.Telefono.Trim(),
            Email = request.Email.Trim().ToLowerInvariant(),
            EsVip = request.EsVip,
            RestriccionesAlimentarias = request.RestriccionesAlimentarias,
            NotasInternas = request.NotasInternas,
            Estado = "activo",
            CreadoPor = usuarioActual
        };

        _contexto.Clientes.Add(cliente);
        _contexto.RegistrosAuditoria.Add(CrearAuditoria("clientes", "crear", usuarioActual, cliente.Id));
        await _contexto.SaveChangesAsync(ct);
        return MapResumen(cliente);
    }

    public async Task<IReadOnlyCollection<ClienteResumenResponse>> ListarAsync(Guid empresaId, string? busqueda, string? estado, bool? esVip, CancellationToken ct)
    {
        var query = _contexto.Clientes.Where(x => x.EmpresaId == empresaId && !x.EliminadoLogico);

        if (!string.IsNullOrWhiteSpace(busqueda))
        {
            var valor = busqueda.Trim().ToLower();
            query = query.Where(x =>
                x.Nombres.ToLower().Contains(valor) ||
                x.Apellidos.ToLower().Contains(valor) ||
                x.Documento.ToLower().Contains(valor) ||
                x.Telefono.ToLower().Contains(valor));
        }

        if (!string.IsNullOrWhiteSpace(estado))
        {
            query = query.Where(x => x.Estado == estado);
        }

        if (esVip.HasValue)
        {
            query = query.Where(x => x.EsVip == esVip.Value);
        }

        return await query
            .OrderBy(x => x.Apellidos)
            .ThenBy(x => x.Nombres)
            .Select(x => MapResumen(x))
            .ToListAsync(ct);
    }

    public async Task<ClienteDetalleResponse?> ObtenerAsync(Guid clienteId, CancellationToken ct)
    {
        var cliente = await _contexto.Clientes.FirstOrDefaultAsync(x => x.Id == clienteId && !x.EliminadoLogico, ct);
        return cliente is null ? null : MapDetalle(cliente);
    }

    public async Task<ClienteDetalleResponse?> ActualizarAsync(Guid clienteId, ActualizarClienteRequest request, string usuarioActual, CancellationToken ct)
    {
        var cliente = await _contexto.Clientes.FirstOrDefaultAsync(x => x.Id == clienteId && !x.EliminadoLogico, ct);
        if (cliente is null)
        {
            return null;
        }

        cliente.Nombres = request.Nombres.Trim();
        cliente.Apellidos = request.Apellidos.Trim();
        cliente.Telefono = request.Telefono.Trim();
        cliente.Email = request.Email.Trim().ToLowerInvariant();
        cliente.EsVip = request.EsVip;
        cliente.Estado = request.Estado;
        cliente.RestriccionesAlimentarias = request.RestriccionesAlimentarias;
        cliente.NotasInternas = request.NotasInternas;
        cliente.SaldoCuenta = request.SaldoCuenta;
        cliente.FechaActualizacionUtc = DateTime.UtcNow;
        cliente.ActualizadoPor = usuarioActual;

        _contexto.RegistrosAuditoria.Add(CrearAuditoria("clientes", "actualizar", usuarioActual, cliente.Id));
        await _contexto.SaveChangesAsync(ct);
        return MapDetalle(cliente);
    }

    public async Task<bool> BajaLogicaAsync(Guid clienteId, string usuarioActual, CancellationToken ct)
    {
        var cliente = await _contexto.Clientes.FirstOrDefaultAsync(x => x.Id == clienteId && !x.EliminadoLogico, ct);
        if (cliente is null)
        {
            return false;
        }

        cliente.EliminadoLogico = true;
        cliente.Estado = "inactivo";
        cliente.FechaActualizacionUtc = DateTime.UtcNow;
        cliente.ActualizadoPor = usuarioActual;
        _contexto.RegistrosAuditoria.Add(CrearAuditoria("clientes", "baja-logica", usuarioActual, cliente.Id));
        await _contexto.SaveChangesAsync(ct);
        return true;
    }

    private static RegistroAuditoria CrearAuditoria(string modulo, string accion, string usuario, Guid entidadId) => new()
    {
        Modulo = modulo,
        Accion = accion,
        Usuario = usuario,
        Entidad = "cliente",
        DetalleJson = $"{{\"entidadId\":\"{entidadId}\"}}"
    };

    private static ClienteResumenResponse MapResumen(Cliente x) =>
        new(x.Id, x.CodigoCliente, $"{x.Nombres} {x.Apellidos}", x.Documento, x.Telefono, x.EsVip, x.SaldoCuenta, x.Estado);

    private static ClienteDetalleResponse MapDetalle(Cliente x) =>
        new(x.Id, x.CodigoCliente, x.Nombres, x.Apellidos, x.Documento, x.Telefono, x.Email, x.EsVip, x.SaldoCuenta, x.Estado,
            x.RestriccionesAlimentarias, x.NotasInternas, x.FechaCreacionUtc);
}
