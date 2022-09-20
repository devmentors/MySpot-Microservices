using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using Micro.Auth.JWT;
using Microsoft.AspNetCore.Authentication.Certificate;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace Micro.Auth;

public static class Extensions
{
    private const string SectionName = "auth";

    public static IServiceCollection AddAuth(this IServiceCollection services, IConfiguration configuration)
    {
        var section = configuration.GetSection(SectionName);
        var options = section.BindOptions<AuthOptions>();
        services.Configure<AuthOptions>(section);
        
        if (!section.Exists())
        {
            return services;
        }
        
        if (options.Jwt is null)
        {
            throw new InvalidOperationException("JWT options cannot be null.");
        }

        var tokenValidationParameters = new TokenValidationParameters
        {
            RequireAudience = options.Jwt.RequireAudience,
            ValidIssuer = options.Jwt.ValidIssuer,
            ValidIssuers = options.Jwt.ValidIssuers,
            ValidateActor = options.Jwt.ValidateActor,
            ValidAudience = options.Jwt.ValidAudience,
            ValidAudiences = options.Jwt.ValidAudiences,
            ValidateAudience = options.Jwt.ValidateAudience,
            ValidateIssuer = options.Jwt.ValidateIssuer,
            ValidateLifetime = options.Jwt.ValidateLifetime,
            ValidateTokenReplay = options.Jwt.ValidateTokenReplay,
            ValidateIssuerSigningKey = options.Jwt.ValidateIssuerSigningKey,
            SaveSigninToken = options.Jwt.SaveSigninToken,
            RequireExpirationTime = options.Jwt.RequireExpirationTime,
            RequireSignedTokens = options.Jwt.RequireSignedTokens,
            ClockSkew = TimeSpan.Zero
        };

        var hasCertificate = false;
        var algorithm = options.Algorithm;
        SecurityKey? securityKey = null;
        if (options.Certificate is not null)
        {
            X509Certificate2? certificate = null;
            var password = options.Certificate.Password;
            var hasPassword = !string.IsNullOrWhiteSpace(password);
            if (!string.IsNullOrWhiteSpace(options.Certificate.Location))
            {
                certificate = hasPassword
                    ? new X509Certificate2(options.Certificate.Location, password)
                    : new X509Certificate2(options.Certificate.Location);
                var keyType = certificate.HasPrivateKey ? "with private key" : "with public key only";
                Console.WriteLine( $"Loaded X.509 certificate from location: '{options.Certificate.Location}' {keyType}.");
            }

            if (!string.IsNullOrWhiteSpace(options.Certificate.RawData))
            {
                var rawData = Convert.FromBase64String(options.Certificate.RawData);
                certificate = hasPassword
                    ? new X509Certificate2(rawData, password)
                    : new X509Certificate2(rawData);
                var keyType = certificate.HasPrivateKey ? "with private key" : "with public key only";
                Console.WriteLine($"Loaded X.509 certificate from raw data {keyType}.");
            }

            if (certificate is not null)
            {
                if (string.IsNullOrWhiteSpace(options.Algorithm))
                {
                    algorithm = SecurityAlgorithms.RsaSha256;
                }

                hasCertificate = true;
                securityKey = new X509SecurityKey(certificate);
                tokenValidationParameters.IssuerSigningKey = securityKey;
                var actionType = certificate.HasPrivateKey ? "issuing" : "validating";
                Console.WriteLine($"Using X.509 certificate for {actionType} tokens.");
            }
        }

        if (!hasCertificate)
        {
            if (string.IsNullOrWhiteSpace(options.Jwt.IssuerSigningKey))
            {
                throw new InvalidOperationException("Missing issuer signing key.");
            }

            if (string.IsNullOrWhiteSpace(options.Algorithm))
            {
                algorithm = SecurityAlgorithms.HmacSha256;
            }

            var rawKey = Encoding.UTF8.GetBytes(options.Jwt.IssuerSigningKey);
            securityKey = new SymmetricSecurityKey(rawKey);
            tokenValidationParameters.IssuerSigningKey = securityKey;
            Console.WriteLine("Using symmetric encryption for issuing tokens.");
        }

        if (!string.IsNullOrWhiteSpace(options.Jwt.AuthenticationType))
        {
            tokenValidationParameters.AuthenticationType = options.Jwt.AuthenticationType;
        }

        if (!string.IsNullOrWhiteSpace(options.Jwt.NameClaimType))
        {
            tokenValidationParameters.NameClaimType = options.Jwt.NameClaimType;
        }

        if (!string.IsNullOrWhiteSpace(options.Jwt.RoleClaimType))
        {
            tokenValidationParameters.RoleClaimType = options.Jwt.RoleClaimType;
        }

        services
            .AddAuthentication(CertificateAuthenticationDefaults.AuthenticationScheme)
            .AddCertificate(certificateAuthenticationOptions =>
            {
                certificateAuthenticationOptions.ValidateValidityPeriod = false;
                certificateAuthenticationOptions.AllowedCertificateTypes = CertificateTypes.All;
                certificateAuthenticationOptions.Events = new CertificateAuthenticationEvents
                {
                    OnChallenge = context => Task.CompletedTask,
                    OnCertificateValidated = context =>
                    {
                        var claims = new[]
                        {
                            new Claim(
                                ClaimTypes.NameIdentifier,
                                context.ClientCertificate.Subject,
                                ClaimValueTypes.String, context.Options.ClaimsIssuer),
                            new Claim(
                                ClaimTypes.Name,
                                context.ClientCertificate.Subject,
                                ClaimValueTypes.String, context.Options.ClaimsIssuer)
                        };

                        context.Principal = new ClaimsPrincipal(new ClaimsIdentity(claims, context.Scheme.Name));
                        context.Success();

                        return Task.CompletedTask;
                    },
                    OnAuthenticationFailed = context => Task.CompletedTask,
                };
            })
            .AddCertificateCache(cache =>
            {
                cache.CacheSize = 1024;
                cache.CacheEntryExpiration = TimeSpan.FromMinutes(5);
            });

        services.AddAuthentication(o =>
            {
                o.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                o.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(jwtBearerOptions =>
            {
                jwtBearerOptions.Authority = options.Jwt.Authority;
                jwtBearerOptions.Audience = options.Jwt.Audience;
                jwtBearerOptions.MetadataAddress = options.Jwt.MetadataAddress ?? string.Empty;
                jwtBearerOptions.SaveToken = options.Jwt.SaveToken;
                jwtBearerOptions.RefreshOnIssuerKeyNotFound = options.Jwt.RefreshOnIssuerKeyNotFound;
                jwtBearerOptions.RequireHttpsMetadata = options.Jwt.RequireHttpsMetadata;
                jwtBearerOptions.IncludeErrorDetails = options.Jwt.IncludeErrorDetails;
                jwtBearerOptions.TokenValidationParameters = tokenValidationParameters;
                if (!string.IsNullOrWhiteSpace(options.Jwt.Challenge))
                {
                    jwtBearerOptions.Challenge = options.Jwt.Challenge;
                }
            });

        if (securityKey is not null)
        {
            services.AddSingleton(new SecurityKeyDetails(securityKey, algorithm));
            services.AddSingleton<IJsonWebTokenManager, JsonWebTokenManager>();
        }

        services.AddSingleton(tokenValidationParameters);
        services.AddAuthorization(a =>
        {
            a.AddPolicy("cert", p =>
            {
                p.AddAuthenticationSchemes(CertificateAuthenticationDefaults.AuthenticationScheme).RequireAuthenticatedUser();
            });
        });

        return services;
    }
}