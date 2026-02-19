using System.Security.Cryptography;
using System.Text;
using Pos.Application.Abstractions;

namespace Pos.Infrastructure.Security;

public sealed class Sha256PasswordHasher : IPasswordHasher
{
    private const string Prefix = "sha256:";

    public bool Verify(string plainText, string passwordHash)
    {
        if (string.IsNullOrWhiteSpace(passwordHash) || !passwordHash.StartsWith(Prefix, StringComparison.Ordinal))
        {
            return false;
        }

        var expected = passwordHash.Substring(Prefix.Length);
        var computed = ComputeSha256(plainText);
        return string.Equals(expected, computed, StringComparison.OrdinalIgnoreCase);
    }

    private static string ComputeSha256(string input)
    {
        var bytes = Encoding.UTF8.GetBytes(input);
        var hash = SHA256.HashData(bytes);
        var sb = new StringBuilder(hash.Length * 2);
        foreach (var b in hash)
        {
            sb.Append(b.ToString("x2"));
        }

        return sb.ToString();
    }
}
