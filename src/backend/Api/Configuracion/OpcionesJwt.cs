namespace Api.Configuracion;

public sealed class OpcionesJwt
{
    public const string Seccion = "Jwt";
    public string Issuer { get; set; } = "kaizen-wellness";
    public string Audience { get; set; } = "kaizen-wellness-clientes";
    public string ClaveSecreta { get; set; } = string.Empty;
    public int MinutosExpiracionAccessToken { get; set; } = 30;
    public int DiasExpiracionRefreshToken { get; set; } = 7;
}
