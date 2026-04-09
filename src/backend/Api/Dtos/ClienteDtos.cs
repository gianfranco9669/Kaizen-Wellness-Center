namespace Api.Dtos;

public sealed record CrearClienteRequest(
    Guid EmpresaId,
    string Nombres,
    string Apellidos,
    string Documento,
    string Telefono,
    string Email,
    bool EsVip,
    string RestriccionesAlimentarias,
    string NotasInternas);

public sealed record ClienteResumenResponse(
    Guid Id,
    string CodigoCliente,
    string NombreCompleto,
    string Documento,
    string Telefono,
    bool EsVip,
    decimal SaldoCuenta,
    string Estado);
