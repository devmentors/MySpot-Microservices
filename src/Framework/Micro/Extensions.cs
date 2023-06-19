using System.Text.Json;
using System.Text.Json.Serialization;
using Micro.Identity;
using Micro.Serialization;
using Micro.Time;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Micro;

public static class Extensions
{
    public static T BindOptions<T>(this IConfiguration configuration, string sectionName) where T : new()
        => BindOptions<T>(configuration.GetSection(sectionName));

    public static T BindOptions<T>(this IConfigurationSection section) where T : new()
    {
        var options = new T();
        section.Bind(options);
        return options;
    }

    public static IServiceCollection AddMicro(this IServiceCollection services, IConfiguration configuration)
    {
        var section = configuration.GetSection("app");
        var options = section.BindOptions<AppOptions>();
        services.Configure<AppOptions>(section);

        return services
            .AddSingleton<IClock, UtcClock>()
            .AddSingleton<IIdGen>(new IdentityGenerator(options.GeneratorId))
            .AddSingleton<IJsonSerializer, SystemTextJsonSerializer>()
            .Configure<JsonOptions>(jsonOptions =>
            {
                jsonOptions.SerializerOptions.PropertyNameCaseInsensitive = true;
                jsonOptions.SerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
                jsonOptions.SerializerOptions.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase));
            });
    }
}