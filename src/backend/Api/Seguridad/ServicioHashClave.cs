using System.Security.Cryptography;

namespace Api.Seguridad;

public interface IServicioHashClave
{
    (string Hash, string Salt) GenerarHash(string clavePlano);
    bool Verificar(string clavePlano, string hash, string salt);
    string CalcularSha256(string valor);
}

public sealed class ServicioHashClave : IServicioHashClave
{
    public (string Hash, string Salt) GenerarHash(string clavePlano)
    {
        var saltBytes = RandomNumberGenerator.GetBytes(16);
        var hash = Convert.ToBase64String(Rfc2898DeriveBytes.Pbkdf2(clavePlano, saltBytes, 100_000, HashAlgorithmName.SHA256, 32));
        return (hash, Convert.ToBase64String(saltBytes));
    }

    public bool Verificar(string clavePlano, string hash, string salt)
    {
        var saltBytes = Convert.FromBase64String(salt);
        var nuevoHash = Convert.ToBase64String(Rfc2898DeriveBytes.Pbkdf2(clavePlano, saltBytes, 100_000, HashAlgorithmName.SHA256, 32));
        return CryptographicOperations.FixedTimeEquals(Convert.FromBase64String(hash), Convert.FromBase64String(nuevoHash));
    }

    public string CalcularSha256(string valor)
    {
        var bytes = SHA256.HashData(System.Text.Encoding.UTF8.GetBytes(valor));
        return Convert.ToHexString(bytes);
    }
}
