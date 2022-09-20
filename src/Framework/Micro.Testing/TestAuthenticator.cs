using System.Security.Cryptography.X509Certificates;
using System.Text;
using Micro.Auth;
using Micro.Auth.JWT;
using Micro.Time;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Micro.Testing;

public sealed class TestAuthenticator
{
    public IJsonWebTokenManager JsonWebTokenManager { get; }

    public JsonWebToken GenerateJsonWebToken(string userId) => JsonWebTokenManager.CreateToken(userId);

    public TestAuthenticator()
    {
        SecurityKey? securityKey = null;
        var options = new OptionsProvider().Get<AuthOptions>("auth");
        var algorithm = string.Empty;
        
        if (options.Certificate is not null && !string.IsNullOrWhiteSpace(options.Certificate.Location))
        {
            var hasPassword = !string.IsNullOrWhiteSpace(options.Certificate.Password);
            var certificate = hasPassword
                ? new X509Certificate2(options.Certificate.Location, options.Certificate.Password)
                : new X509Certificate2(options.Certificate.Location);
            securityKey = new X509SecurityKey(certificate);
            algorithm = SecurityAlgorithms.RsaSha256;
        }

        if (options.Jwt is not null && !string.IsNullOrWhiteSpace(options.Jwt.IssuerSigningKey))
        {
            var rawKey = Encoding.UTF8.GetBytes(options.Jwt.IssuerSigningKey);
            securityKey = new SymmetricSecurityKey(rawKey);
            algorithm = SecurityAlgorithms.HmacSha256;
        }

        if (securityKey is null)
        {
            throw new InvalidOperationException("Security key cannot be null.");
        }

        JsonWebTokenManager = new JsonWebTokenManager(new OptionsWrapper<AuthOptions>(options),
            new SecurityKeyDetails(securityKey, algorithm), new UtcClock());
    }
}