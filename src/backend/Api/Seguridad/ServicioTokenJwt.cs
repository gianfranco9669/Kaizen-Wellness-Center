using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Api.Configuracion;
using Api.Datos;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Api.Seguridad;

public interface IServicioTokenJwt
{
    string CrearAccessToken(UsuarioSistema usuario, IEnumerable<string> roles, IEnumerable<string> permisos);
    string CrearRefreshToken();
}

public sealed class ServicioTokenJwt : IServicioTokenJwt
{
    private readonly OpcionesJwt _opciones;

    public ServicioTokenJwt(IOptions<OpcionesJwt> opciones)
    {
        _opciones = opciones.Value;
    }

    public string CrearAccessToken(UsuarioSistema usuario, IEnumerable<string> roles, IEnumerable<string> permisos)
    {
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, usuario.Id.ToString()),
            new(JwtRegisteredClaimNames.Email, usuario.Email),
            new(ClaimTypes.Name, usuario.NombreCompleto),
            new("empresa_id", usuario.EmpresaId.ToString())
        };

        claims.AddRange(roles.Select(r => new Claim(ClaimTypes.Role, r)));
        claims.AddRange(permisos.Select(p => new Claim("permiso", p)));

        var credenciales = new SigningCredentials(
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_opciones.ClaveSecreta)),
            SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _opciones.Issuer,
            audience: _opciones.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(_opciones.MinutosExpiracionAccessToken),
            signingCredentials: credenciales);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public string CrearRefreshToken()
    {
        var bytes = RandomNumberGenerator.GetBytes(64);
        return Convert.ToBase64String(bytes);
    }
}
