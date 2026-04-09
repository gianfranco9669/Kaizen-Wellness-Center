namespace Api.Dtos;

public sealed record LoginRequest(string Email, string Clave);
public sealed record AuthResponse(string AccessToken, string RefreshToken, DateTime ExpiraUtc);
