using Micro.Security.Vault.Internals;
using Micro.Serialization;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using VaultSharp;
using VaultSharp.V1.AuthMethods;
using VaultSharp.V1.AuthMethods.Token;
using VaultSharp.V1.AuthMethods.UserPass;
using VaultSharp.V1.SecretsEngines;

namespace Micro.Security.Vault;

public static class Extensions
{
    private const string SectionName = "vault";
    private static readonly ILeaseService LeaseService = new LeaseService();
    private static readonly ICertificatesStore CertificatesStore = new CertificatesStore();
    
    public static WebApplicationBuilder AddVault(this WebApplicationBuilder builder)
    {
        var section = builder.Configuration.GetSection(SectionName);
        var options = section.BindOptions<VaultOptions>();
        builder.Services.Configure<VaultOptions>(section);
        
        if (!options.Enabled)
        {
            return builder;
        }
        
        VerifyOptions(options);
        builder.Services.AddVault(options);
        builder.Configuration.AddVaultAsync(options).GetAwaiter().GetResult();
        
        return builder;
    }

    private static void AddVault(this IServiceCollection services, VaultOptions options)
    {
        var (client, settings) = GetClientAndSettings(options);
        services.AddTransient<IKeyValueSecrets, KeyValueSecrets>();
        services.AddSingleton(settings);
        services.AddSingleton(client);
        services.AddSingleton(LeaseService);
        services.AddSingleton(CertificatesStore);
        services.AddHostedService<VaultHostedService>();
        
        if (options.PKI.Enabled)
        {
            services.AddSingleton<ICertificatesIssuer, CertificatesIssuer>();
        }
        else
        {
            services.AddSingleton<ICertificatesIssuer, EmptyCertificatesIssuer>();
        }
    }

    private static void VerifyOptions(VaultOptions options)
    {
        options.KV.EngineVersion = options.KV.EngineVersion switch
        {
            > 2 or < 0 => throw new VaultException($"Invalid KV engine version: {options.KV.EngineVersion} (available: 1 or 2)."),
            0 => 2,
            _ => options.KV.EngineVersion
        };
    }

    private static async Task AddVaultAsync(this IConfigurationBuilder builder, VaultOptions options)
    {
        var (client, _) = GetClientAndSettings(options);
        if (options.KV.Enabled)
        {
            var kvPath = options.KV.Path;
            var mountPoint = options.KV.MountPoint;
            if (string.IsNullOrWhiteSpace(kvPath))
            {
                throw new VaultException("KV path is missing.");
            }
            
            if (string.IsNullOrWhiteSpace(mountPoint))
            {
                throw new VaultException("KV mount point is missing.");
            }
            
            Console.WriteLine($"Loading settings from Vault: '{options.Url}', KV path: '{mountPoint}/{kvPath}'...");
            var jsonSerializer = new SystemTextJsonSerializer();
            var keyValueSecrets = new KeyValueSecrets(client, new OptionsWrapper<VaultOptions>(options),
                jsonSerializer);
            var secret = await keyValueSecrets.GetAsync(kvPath);
            var parser = new JsonParser();
            var json = JsonConvert.SerializeObject(secret);
            var data = parser.Parse(json);
            var source = new MemoryConfigurationSource {InitialData = data!};
            builder.Add(source);
        }

        if (options.PKI.Enabled)
        {
            Console.WriteLine("Initializing Vault PKI.");
            await SetPkiSecretsAsync(client, options);
        }

        if (!options.Lease.Any())
        {
            return;
        }

        var configuration = new Dictionary<string, string>();
        foreach (var (key, lease) in options.Lease)
        {
            if (!lease.Enabled || string.IsNullOrWhiteSpace(lease.Type))
            {
                continue;
            }

            Console.WriteLine($"Initializing Vault lease for: '{key}', type: '{lease.Type}'.");
            await InitLeaseAsync(key, client, lease, configuration);
        }

        if (configuration.Any())
        {
            var source = new MemoryConfigurationSource {InitialData = configuration!};
            builder.Add(source);
        }
    }

    private static Task InitLeaseAsync(string key, IVaultClient client, VaultOptions.LeaseOptions options,
        IDictionary<string, string> configuration)
        => options.Type.ToLowerInvariant() switch
        {
            "activedirectory" => SetActiveDirectorySecretsAsync(key, client, options, configuration),
            "azure" => SetAzureSecretsAsync(key, client, options, configuration),
            "consul" => SetConsulSecretsAsync(key, client, options, configuration),
            "database" => SetDatabaseSecretsAsync(key, client, options, configuration),
            "rabbitmq" => SetRabbitMqSecretsAsync(key, client, options, configuration),
            _ => Task.CompletedTask
        };

    private static async Task SetActiveDirectorySecretsAsync(string key, IVaultClient client,
        VaultOptions.LeaseOptions options, IDictionary<string, string> configuration)
    {
        const string name = SecretsEngineMountPoints.Defaults.ActiveDirectory;
        var mountPoint = string.IsNullOrWhiteSpace(options.MountPoint) ? name : options.MountPoint;
        var credentials =
            await client.V1.Secrets.ActiveDirectory.GetCredentialsAsync(options.RoleName, mountPoint);
        SetSecrets(key, options, configuration, name, () => (credentials, new Dictionary<string, string>
        {
            ["username"] = credentials.Data.Username,
            ["currentPassword"] = credentials.Data.CurrentPassword,
            ["lastPassword"] = credentials.Data.LastPassword
        }, credentials.LeaseId, credentials.LeaseDurationSeconds, credentials.Renewable));
    }

    private static async Task SetAzureSecretsAsync(string key, IVaultClient client,
        VaultOptions.LeaseOptions options,
        IDictionary<string, string> configuration)
    {
        const string name = SecretsEngineMountPoints.Defaults.Azure;
        var mountPoint = string.IsNullOrWhiteSpace(options.MountPoint) ? name : options.MountPoint;
        var credentials =
            await client.V1.Secrets.Azure.GetCredentialsAsync(options.RoleName, mountPoint);
        SetSecrets(key, options, configuration, name, () => (credentials, new Dictionary<string, string>
        {
            ["clientId"] = credentials.Data.ClientId,
            ["clientSecret"] = credentials.Data.ClientSecret
        }, credentials.LeaseId, credentials.LeaseDurationSeconds, credentials.Renewable));
    }

    private static async Task SetConsulSecretsAsync(string key, IVaultClient client,
        VaultOptions.LeaseOptions options,
        IDictionary<string, string> configuration)
    {
        const string name = SecretsEngineMountPoints.Defaults.Consul;
        var mountPoint = string.IsNullOrWhiteSpace(options.MountPoint) ? name : options.MountPoint;
        var credentials =
            await client.V1.Secrets.Consul.GetCredentialsAsync(options.RoleName, mountPoint);
        SetSecrets(key, options, configuration, name, () => (credentials, new Dictionary<string, string>
        {
            ["token"] = credentials.Data.Token
        }, credentials.LeaseId, credentials.LeaseDurationSeconds, credentials.Renewable));
    }

    private static async Task SetDatabaseSecretsAsync(string key, IVaultClient client,
        VaultOptions.LeaseOptions options,
        IDictionary<string, string> configuration)
    {
        const string name = SecretsEngineMountPoints.Defaults.Database;
        var mountPoint = string.IsNullOrWhiteSpace(options.MountPoint) ? name : options.MountPoint;
        var credentials =
            await client.V1.Secrets.Database.GetCredentialsAsync(options.RoleName, mountPoint);
        SetSecrets(key, options, configuration, name, () => (credentials, new Dictionary<string, string>
        {
            ["username"] = credentials.Data.Username,
            ["password"] = credentials.Data.Password
        }, credentials.LeaseId, credentials.LeaseDurationSeconds, credentials.Renewable));
    }

    private static async Task SetPkiSecretsAsync(IVaultClient client, VaultOptions options)
    {
        var issuer = new CertificatesIssuer(client, new OptionsWrapper<VaultOptions>(options));
        var certificate = await issuer.IssueAsync();
        if (certificate is not null)
        {
            CertificatesStore.Set(options.PKI.RoleName, certificate);
        }
    }

    private static async Task SetRabbitMqSecretsAsync(string key, IVaultClient client,
        VaultOptions.LeaseOptions options,
        IDictionary<string, string> configuration)
    {
        const string name = SecretsEngineMountPoints.Defaults.RabbitMQ;
        var mountPoint = string.IsNullOrWhiteSpace(options.MountPoint) ? name : options.MountPoint;
        var credentials =
            await client.V1.Secrets.RabbitMQ.GetCredentialsAsync(options.RoleName, mountPoint);
        SetSecrets(key, options, configuration, name, () => (credentials, new Dictionary<string, string>
        {
            ["username"] = credentials.Data.Username,
            ["password"] = credentials.Data.Password
        }, credentials.LeaseId, credentials.LeaseDurationSeconds, credentials.Renewable));
    }

    private static void SetSecrets(string key, VaultOptions.LeaseOptions options,
        IDictionary<string, string> configuration, string name,
        Func<(object, Dictionary<string, string>, string, int, bool)> lease)
    {
        var createdAt = DateTime.UtcNow;
        var (credentials, values, leaseId, duration, renewable) = lease();
        SetTemplates(key, options, configuration, values);
        var leaseData = new LeaseData(name, leaseId, duration, renewable, createdAt, credentials);
        LeaseService.Set(key, leaseData);
    }

    private static (IVaultClient client, VaultClientSettings settings) GetClientAndSettings(VaultOptions options)
    {
        var settings = new VaultClientSettings(options.Url, GetAuthMethod(options.Authentication));
        var client = new VaultClient(settings);

        return (client, settings);
    }

    private static void SetTemplates(string key, VaultOptions.LeaseOptions lease,
        IDictionary<string, string> configuration, IDictionary<string, string> values)
    {
        if (!lease.Templates.Any())
        {
            return;
        }

        foreach (var (property, template) in lease.Templates)
        {
            if (string.IsNullOrWhiteSpace(property) || string.IsNullOrWhiteSpace(template))
            {
                continue;
            }

            var templateValue = $"{template}";
            templateValue = values.Aggregate(templateValue,
                (current, value) => current.Replace($"{{{{{value.Key}}}}}", value.Value));
            configuration.Add($"{key}:{property}", templateValue);
        }
    }

    private static IAuthMethodInfo GetAuthMethod(VaultOptions.AuthenticationOptions options)
    {
        if (string.IsNullOrWhiteSpace(options.Type))
        {
            throw new VaultException("Vault authentication type is empty.");
        }
        
        return options.Type.ToLowerInvariant() switch
        {
            "token" => new TokenAuthMethodInfo(options.Token.Token),
            "userpass" => new UserPassAuthMethodInfo(options.UserPass.Username, options.UserPass.Password),
            _ => throw new VaultAuthTypeNotSupportedException($"Vault authentication type: '{options.Type}' " +
                                                              "is not supported.", options.Type)
        };
    }
    
    public static IHttpClientBuilder AddVaultCertificatesHandler(this IHttpClientBuilder builder,
        IConfiguration configuration)
    {
        var section = configuration.GetSection(SectionName);
        var options = section.BindOptions<VaultOptions>();
        if (!options.Enabled || !options.PKI.Enabled || !options.PKI.HttpHandler.Enabled)
        {
            return builder;
        }

        var certificateName = options.PKI.HttpHandler.Certificate;
        if (string.IsNullOrWhiteSpace(certificateName))
        {
            throw new VaultException("PKI HTTP handler certificate name is empty.");
        }

        var certificate = CertificatesStore.Get(certificateName);
        if (certificate is null)
        {
            throw new VaultException($"PKI HTTP handler certificate: '{certificateName}' was not found.");
        }
        
        return builder.ConfigurePrimaryHttpMessageHandler(() =>
        {
            var handler = new HttpClientHandler();
            handler.ClientCertificates.Add(certificate);
            return handler;
        });
    }
}