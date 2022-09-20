using Micro.Security.Encryption;
using Micro.Security.Hashing;
using Micro.Security.Random;
using Micro.Security.Signing;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Micro.Security;

public static class Extensions
{
    public static IServiceCollection AddSecurity(this IServiceCollection services, IConfiguration configuration)
    {
        var section = configuration.GetSection("security");
        services.Configure<SecurityOptions>(section);

        services
            .AddSingleton<IEncryptor, AesEncryptor>()
            .AddSingleton<IShaHasher, ShaHasher>()
            .AddSingleton<IRng, Rng>()
            .AddSingleton<ISigner, Signer>()
            .AddSingleton<IPasswordManager, PasswordManager>()
            .AddSingleton<IPasswordHasher<object>, PasswordHasher<object>>();

        return services;
    }
}