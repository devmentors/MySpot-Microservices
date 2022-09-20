using Consul;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Micro.HTTP.ServiceDiscovery;

public static class Extensions
{
    public static IServiceCollection AddConsul(this IServiceCollection services, IConfiguration configuration)
    {
        var section = configuration.GetSection("consul");
        var options = section.BindOptions<ConsulOptions>();
        services.Configure<ConsulOptions>(section);
        if (!options.Enabled)
        {
            return services;
        }

        if (string.IsNullOrWhiteSpace(options.Url))
        {
            throw new ArgumentException("Consul URL cannot be empty.", nameof(options.Url));
        }

        services.AddTransient<ConsulHttpHandler>();
        services.AddHostedService<ConsulRegistrationService>();
        services.AddSingleton<IServiceDiscoveryRegistration, DefaultServiceDiscoveryRegistration>();
        services.AddSingleton<IConsulClient>(new ConsulClient(consulConfig =>
        {
            consulConfig.Address = new Uri(options.Url);
        }));

        return services;
    }

    public static IHttpClientBuilder AddConsulHandler(this IHttpClientBuilder builder)
        => builder.AddHttpMessageHandler<ConsulHttpHandler>();
}