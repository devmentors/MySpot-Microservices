using Micro.Serialization;
using Micro.Time;
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
        => services
            .Configure<AppOptions>(configuration.GetSection("app"))
            .AddSingleton<IClock, UtcClock>()
            .AddSingleton<IJsonSerializer, SystemTextJsonSerializer>();
}