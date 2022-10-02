using System.Diagnostics.CodeAnalysis;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using PactNet;
using PactNet.Verifier;
using Xunit.Abstractions;

namespace Micro.Testing;

[ExcludeFromCodeCoverage]
public sealed class EndpointContract : IDisposable
{
    private readonly PactBrokerPublisher _publisher;
    public IPactBuilderV3 Pact { get; }
    public PactVerifier Verifier { get; }
    public JsonSerializerSettings SerializerSettings { get; }
    public string Consumer { get; }
    public string Provider { get; }
    public int Port { get; }
    public string PactsDirectory { get; }
    public Uri PactBrokerUrl { get; }
    public string ApiKey { get; }

    public EndpointContract(string consumer, string provider, ITestOutputHelper output, int port = 9000,
        string pactsDirectory = "", string pactBrokerUrl = "http://localhost:9292", string apiKey = "")
    {
        Consumer = consumer;
        Provider = $"{provider}_endpoints";
        Port = port;
        PactsDirectory = string.IsNullOrWhiteSpace(pactsDirectory)
            ? Path.Join("..", "..", "..", "..", "..", "pacts", "endpoints")
            : pactsDirectory;
        
        PactBrokerUrl = new Uri(pactBrokerUrl);
        ApiKey = apiKey;

        SerializerSettings = new JsonSerializerSettings
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver(),
            DefaultValueHandling = DefaultValueHandling.Ignore,
            NullValueHandling = NullValueHandling.Ignore,
            Formatting = Formatting.Indented
        };
        
        var config = new PactConfig
        {
            PactDir = PactsDirectory,
            Outputters = new[] {new XUnitOutput(output)},
            DefaultJsonSettings = SerializerSettings
        };

        Pact = PactNet.Pact.V3(Consumer, Provider, config).WithHttpInteractions(Port);
        Verifier = new PactVerifier(new PactVerifierConfig
        {
            Outputters = new[] {new XUnitOutput(output)}
        });

        _publisher = new PactBrokerPublisher(Consumer, Provider, output, pactBrokerUrl, apiKey);
    }

    public async Task PublishToPactBrokerAsync(string version)
    {
        var fileInfo = GetPactFile();
        if (!fileInfo.Exists)
        {
            throw new InvalidOperationException("Pact was not found.");
        }
        
        var json = await File.ReadAllTextAsync(fileInfo.FullName);
        await _publisher.PublishAsync(json, version);
    }

    public FileInfo GetPactFile() => GetPactFile($"{Consumer}-{Provider}");

    public FileInfo GetPactFile(string pactName)
    {
        var path = Path.Join(PactsDirectory, $"{pactName}.json");
        var fileInfo = new FileInfo(path);
        if (!fileInfo.Exists)
        {
            throw new InvalidOperationException($"Pact: '{path}' was not found.");
        }

        return fileInfo;
    }

    public void Dispose()
    {
        Verifier.Dispose();
    }
}