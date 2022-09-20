using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text.Json;
using Microsoft.Extensions.Logging;

namespace Micro.Security.Signing;

internal sealed class Signer : ISigner
{
    private readonly ILogger<Signer> _logger;

    public Signer(ILogger<Signer> logger)
    {
        _logger = logger;
    }
        
    public string Sign(object context, X509Certificate2 certificate)
    {
        var bytes = JsonSerializer.SerializeToUtf8Bytes(context);
        var signature = GetSignature(certificate, bytes);
            
        return BitConverter.ToString(signature).Replace("-", string.Empty);
    }

    public bool Verify(object context, X509Certificate2 certificate, string signature)
    {
        var bytes = JsonSerializer.SerializeToUtf8Bytes(context);

        try
        {
            return ValidateSignature(certificate, bytes, ToByteArray(signature));
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, exception.Message);
            return false;
        }
    }

    private static byte[] ToByteArray(string hex)
    {
        var bytes = new byte[hex.Length / 2];
        for (var i = 0; i < hex.Length; i += 2)
        {
            bytes[i / 2] = Convert.ToByte(hex.Substring(i, 2), 16);
        }
            
        return bytes;
    }
        
    private static byte[] GetSignature(X509Certificate2 certificate, byte[] data)
    {
        using var rsa = certificate.GetRSAPrivateKey();
        if (rsa is null)
        {
            throw new InvalidOperationException("RSA private key cannot be null");
        }
            
        return rsa.SignData(data, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
    }

    private static bool ValidateSignature(X509Certificate2 certificate, byte[] data, byte[] signature)
    {
        using var rsa = certificate.GetRSAPublicKey();
        if (rsa is null)
        {
            throw new InvalidOperationException("RSA public key cannot be null");
        }
            
        return rsa.VerifyData(data, signature, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
    }
}