using System.Security.Cryptography;
using System.Text;

namespace Micro.Security.Hashing;

internal sealed class ShaHasher : IShaHasher
{
    public string Sha256(string data)
    {
        using var sha512 = SHA256.Create();
        var bytes = sha512.ComputeHash(Encoding.UTF8.GetBytes(data));
        var builder = new StringBuilder();
        foreach (var @byte in bytes)
        {
            builder.Append(@byte.ToString("x2"));
        }

        return builder.ToString();
    }

    public string Sha512(string data)
    {
        using var sha512 = SHA512.Create();
        var bytes = sha512.ComputeHash(Encoding.UTF8.GetBytes(data));
        var builder = new StringBuilder();
        foreach (var @byte in bytes)
        {
            builder.Append(@byte.ToString("x2"));
        }

        return builder.ToString();
    }
}