using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Options;

namespace Micro.Security.Encryption;

internal sealed class AesEncryptor : IEncryptor
{
    private readonly string? _defaultKey;

    public AesEncryptor(IOptions<SecurityOptions> options)
    {
        _defaultKey = options.Value.Encryption.Key;
    }
    
    public string Encrypt(string data, string? key = null)
    {
        if (string.IsNullOrWhiteSpace(data))
        {
            throw new InvalidOperationException("Data to be encrypted cannot be empty.");
        }

        key = GetKey(key);

        using var aes = Aes.Create();
        aes.Key = Encoding.UTF8.GetBytes(key);
        var iv = Convert.ToBase64String(aes.IV);
        var transform = aes.CreateEncryptor(aes.Key, aes.IV);
        using var memoryStream = new MemoryStream();
        using var cryptoStream = new CryptoStream(memoryStream, transform, CryptoStreamMode.Write);
        using (var streamWriter = new StreamWriter(cryptoStream))
        {
            streamWriter.Write(data);
        }

        return iv + Convert.ToBase64String(memoryStream.ToArray());
    }

    public string Decrypt(string data, string? key = null)
    {
        if (string.IsNullOrWhiteSpace(data))
        {
            throw new InvalidOperationException("Data to be decrypted cannot be empty.");
        }

        key = GetKey(key);

        using var aes = Aes.Create();
        aes.Key = Encoding.UTF8.GetBytes(key);
        aes.IV = Convert.FromBase64String(data.Substring(0, 24));
        var transform = aes.CreateDecryptor(aes.Key, aes.IV);
        using var memoryStream = new MemoryStream(Convert.FromBase64String(data.Substring(24)));
        using var cryptoStream = new CryptoStream(memoryStream, transform, CryptoStreamMode.Read);
        using var streamReader = new StreamReader(cryptoStream);

        return streamReader.ReadToEnd();
    }

    private string GetKey(string? key)
    {
        key = string.IsNullOrWhiteSpace(key) ? _defaultKey : key;
        
        if (string.IsNullOrWhiteSpace(key))
        {
            throw new InvalidOperationException("Encryption key cannot be empty.");
        }
        
        if (key.Length != 32)
        {
            throw new InvalidOperationException($"Invalid encryption key length: {key.Length} (required: 32 chars).");
        }

        return key;
    }
}