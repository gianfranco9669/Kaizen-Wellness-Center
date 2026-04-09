using System.Threading.Tasks;
using Xunit;
using Api.Configuracion;
using Api.Datos;
using Api.Dtos;
using Api.Seguridad;
using Api.Servicios;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Api.UnitTests;

public class AuthTests
{
    [Fact]
    public async Task Login_devuelve_tokens_validos()
    {
        var opciones = new DbContextOptionsBuilder<ContextoWellness>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        await using var db = new ContextoWellness(opciones);
        var hash = new ServicioHashClave();
        var (claveHash, claveSalt) = hash.GenerarHash("ClaveSegura123*");

        var usuario = new UsuarioSistema
        {
            EmpresaId = Guid.NewGuid(),
            NombreCompleto = "Test Usuario",
            Email = "test@kaizen.local",
            ClaveHash = claveHash,
            ClaveSalt = claveSalt,
            Activo = true,
            CreadoPor = "test"
        };
        db.Usuarios.Add(usuario);
        await db.SaveChangesAsync();

        var token = new ServicioTokenJwt(Options.Create(new OpcionesJwt
        {
            Issuer = "issuer",
            Audience = "audience",
            ClaveSecreta = "CLAVE_SEGURA_MUY_LARGA_DE_PRUEBA_123456",
            MinutosExpiracionAccessToken = 30,
            DiasExpiracionRefreshToken = 7
        }));

        var servicio = new ServicioAuth(db, hash, token, Options.Create(new OpcionesJwt
        {
            Issuer = "issuer",
            Audience = "audience",
            ClaveSecreta = "CLAVE_SEGURA_MUY_LARGA_DE_PRUEBA_123456",
            MinutosExpiracionAccessToken = 30,
            DiasExpiracionRefreshToken = 7
        }));

        var resultado = await servicio.LoginAsync(new LoginRequest("test@kaizen.local", "ClaveSegura123*"), "127.0.0.1", CancellationToken.None);

        Assert.True(resultado.Exitoso);
        Assert.False(string.IsNullOrWhiteSpace(resultado.Valor!.AccessToken));
        Assert.False(string.IsNullOrWhiteSpace(resultado.Valor!.RefreshToken));
    }
}
