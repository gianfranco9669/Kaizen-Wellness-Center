using System.ComponentModel.DataAnnotations;

namespace Api.Dtos;

public sealed record LoginRequest(
    [property: Required, EmailAddress] string Email,
    [property: Required, MinLength(8)] string Clave);

public sealed record RefreshTokenRequest([property: Required] string RefreshToken);

public sealed record AuthResponse(
    string AccessToken,
    string RefreshToken,
    DateTime AccessTokenExpiraUtc,
    string NombreUsuario,
    IReadOnlyCollection<string> Roles,
    IReadOnlyCollection<string> Permisos);
