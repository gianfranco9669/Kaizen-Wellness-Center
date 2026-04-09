using Api.Datos;
using Api.Dtos;
using Microsoft.EntityFrameworkCore;

namespace Api.Servicios;

public interface IServicioClientes
{
    Task<ClienteResumenResponse> CrearAsync(CrearClienteRequest request, string usuarioActual, CancellationToken ct);
    Task<IReadOnlyCollection<ClienteResumenResponse>> ListarAsync(Guid empresaId, CancellationToken ct);
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
            CodigoCliente = $"CLI-{DateTime.UtcNow:yyyyMMddHHmmss}",
            Nombres = request.Nombres,
            Apellidos = request.Apellidos,
            Documento = request.Documento,
            Telefono = request.Telefono,
            Email = request.Email,
            EsVip = request.EsVip,
            RestriccionesAlimentarias = request.RestriccionesAlimentarias,
            NotasInternas = request.NotasInternas,
            Estado = "activo",
            CreadoPor = usuarioActual
        };

        _contexto.Clientes.Add(cliente);
        _contexto.RegistrosAuditoria.Add(new RegistroAuditoria
        {
            Modulo = "clientes",
            Accion = "crear",
            Usuario = usuarioActual,
            DetalleJson = $"{{\"clienteId\":\"{cliente.Id}\"}}"
        });

        await _contexto.SaveChangesAsync(ct);
        return Map(cliente);
    }

    public async Task<IReadOnlyCollection<ClienteResumenResponse>> ListarAsync(Guid empresaId, CancellationToken ct)
    {
        var data = await _contexto.Clientes
            .Where(x => x.EmpresaId == empresaId && !x.EliminadoLogico)
            .OrderBy(x => x.Apellidos)
            .ThenBy(x => x.Nombres)
            .Select(x => Map(x))
            .ToListAsync(ct);

        return data;
    }

    private static ClienteResumenResponse Map(Cliente x) =>
        new(x.Id, x.CodigoCliente, $"{x.Nombres} {x.Apellidos}", x.Documento, x.Telefono, x.EsVip, x.SaldoCuenta, x.Estado);
}
