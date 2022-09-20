using Micro.Serialization;
using Microsoft.Extensions.Options;
using VaultSharp;

namespace Micro.Security.Vault.Internals;

internal sealed class KeyValueSecrets : IKeyValueSecrets
{
    private readonly IVaultClient _client;
    private readonly VaultOptions _options;
    private readonly IJsonSerializer _jsonSerializer;

    public KeyValueSecrets(IVaultClient client, IOptions<VaultOptions> options, IJsonSerializer jsonSerializer)
    {
        _client = client;
        _options = options.Value;
        _jsonSerializer = jsonSerializer;
    }

    public async Task<T> GetAsync<T>(string path)
        => _jsonSerializer.Deserialize<T>(_jsonSerializer.Serialize(await GetAsync(path)))!;

    public async Task<IDictionary<string, object>> GetAsync(string path)
    {
        if (string.IsNullOrWhiteSpace(path))
        {
            throw new VaultException("Vault KV secret path can not be empty.");
        }

        try
        {
            switch (_options.KV.EngineVersion)
            {
                case 1:
                    var secretV1 = await _client.V1.Secrets.KeyValue.V1.ReadSecretAsync(path,
                        _options.KV.MountPoint);
                    return secretV1.Data;
                case 2:
                    var secretV2 = await _client.V1.Secrets.KeyValue.V2.ReadSecretAsync(path,
                        _options.KV.Version, _options.KV.MountPoint);
                    return secretV2.Data.Data;
                default:
                    throw new VaultException($"Invalid KV engine version: {_options.KV.EngineVersion}.");
            }
        }
        catch (Exception exception)
        {
            throw new VaultException($"Getting Vault secret for path: '{path}' caused an error. " +
                                     $"{exception.Message}", exception);
        }
    }
}