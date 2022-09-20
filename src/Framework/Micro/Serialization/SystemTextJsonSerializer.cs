using System.Text.Json;
using System.Text.Json.Serialization;

namespace Micro.Serialization;

public sealed class SystemTextJsonSerializer : IJsonSerializer
{
    private readonly JsonSerializerOptions _options = new()
    {
        PropertyNameCaseInsensitive = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        Converters = {new JsonStringEnumConverter(JsonNamingPolicy.CamelCase)}
    };

    public string Serialize<T>(T value) => JsonSerializer.Serialize(value, _options);
    public T? Deserialize<T>(string value) => JsonSerializer.Deserialize<T>(value, _options);
    public object? Deserialize(string value, Type type) => JsonSerializer.Deserialize(value, type, _options);
}