using System.ComponentModel.DataAnnotations;

namespace Api.Dtos;

public sealed record CrearClienteRequest(
    [property: Required] Guid EmpresaId,
    [property: Required, MaxLength(120)] string Nombres,
    [property: Required, MaxLength(120)] string Apellidos,
    [property: Required, MaxLength(30)] string Documento,
    [property: Required, MaxLength(30)] string Telefono,
    [property: Required, EmailAddress, MaxLength(120)] string Email,
    bool EsVip,
    [property: MaxLength(300)] string RestriccionesAlimentarias,
    [property: MaxLength(1000)] string NotasInternas);

public sealed record ActualizarClienteRequest(
    [property: Required, MaxLength(120)] string Nombres,
    [property: Required, MaxLength(120)] string Apellidos,
    [property: Required, MaxLength(30)] string Telefono,
    [property: Required, EmailAddress, MaxLength(120)] string Email,
    bool EsVip,
    [property: MaxLength(64)] string Estado,
    [property: MaxLength(300)] string RestriccionesAlimentarias,
    [property: MaxLength(1000)] string NotasInternas,
    decimal SaldoCuenta);

public sealed record ClienteResumenResponse(
    Guid Id,
    string CodigoCliente,
    string NombreCompleto,
    string Documento,
    string Telefono,
    bool EsVip,
    decimal SaldoCuenta,
    string Estado);

public sealed record ClienteDetalleResponse(
    Guid Id,
    string CodigoCliente,
    string Nombres,
    string Apellidos,
    string Documento,
    string Telefono,
    string Email,
    bool EsVip,
    decimal SaldoCuenta,
    string Estado,
    string RestriccionesAlimentarias,
    string NotasInternas,
    DateTime FechaCreacionUtc);
