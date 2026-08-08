using System.Security.Cryptography;

namespace Siv.Application.Seguridad;

public static class PasswordHasher
{
    private const int Iteraciones = 120_000;
    private const int TamanoSalt = 16;
    private const int TamanoHash = 32;

    public static string Crear(string password)
    {
        var salt = RandomNumberGenerator.GetBytes(TamanoSalt);
        var hash = Rfc2898DeriveBytes.Pbkdf2(
            password, salt, Iteraciones, HashAlgorithmName.SHA256, TamanoHash);

        return $"PBKDF2-SHA256${Iteraciones}${Convert.ToBase64String(salt)}${Convert.ToBase64String(hash)}";
    }

    public static bool Verificar(string password, string formato)
    {
        try
        {
            var partes = formato.Split('$');
            if (partes.Length != 4 || !int.TryParse(partes[1], out var iteraciones))
                return false;

            var salt = Convert.FromBase64String(partes[2]);
            var hashEsperado = Convert.FromBase64String(partes[3]);
            var hashActual = Rfc2898DeriveBytes.Pbkdf2(
                password, salt, iteraciones, HashAlgorithmName.SHA256, hashEsperado.Length);

            return CryptographicOperations.FixedTimeEquals(hashActual, hashEsperado);
        }
        catch (FormatException)
        {
            return false;
        }
    }
}
