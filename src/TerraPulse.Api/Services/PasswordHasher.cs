using System.Security.Cryptography;
using System.Text;

namespace TerraPulse.Api.Services;

public static class PasswordHasher
{
    public static string ComputeHash(string password)
    {
        var bytes = Encoding.UTF8.GetBytes(password);
        var hashBytes = SHA256.HashData(bytes);

        return Convert.ToHexString(hashBytes);
    }

    public static bool Verify(string password, string storedHash)
    {
        var passwordHash = ComputeHash(password);

        return string.Equals(passwordHash, storedHash, StringComparison.OrdinalIgnoreCase);
    }
}