using System.Diagnostics.CodeAnalysis;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using PactNet;
using PactNet.Verifier;
using PactNet.Verifier.Messaging;
using Xunit.Abstractions;

namespace Micro.Testing;

[ExcludeFromCodeCoverage]
public sealed class MessageContract : IDisposable
{
    private readonly PactBrokerPublisher _publisher;
    public IMessagePactBuilderV3 Pact { get; }
    public PactVerifier Verifier { get; }
    public JsonSerializerSettings SerializerSettings { get; }
    public string Consumer { get; }
    public string Provider { get; }
    public string PactsDirectory { get; }
    public Uri PactBrokerUrl { get; }
    public string ApiKey { get; }

    public MessageContract(string consumer, string provider, ITestOutputHelper output, string pactsDirectory = "",
        string pactBrokerUrl = "http://localhost:9292", string apiKey = "")
    {
        Consumer = consumer;
        Provider = provider;
        PactsDirectory = string.IsNullOrWhiteSpace(pactsDirectory)
            ? Path.Join("..", "..", "..", "..", "..", "pacts", "messages")
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

        Pact = PactNet.Pact.V3(consumer, provider, config).WithMessageInteractions();
        Verifier = new PactVerifier(new PactVerifierConfig
        {
            Outputters = new[] {new XUnitOutput(output)}
        });
        
        _publisher = new PactBrokerPublisher(consumer, provider, output, pactBrokerUrl, apiKey);
    }

    public IPactVerifierMessagingProvider MessagingProvider()
        => Verifier.MessagingProvider(Provider, SerializerSettings);
    
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