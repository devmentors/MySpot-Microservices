using Micro.HTTP.ServiceDiscovery;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Micro.HTTP.LoadBalancing;

public static class Extensions
{
    public static IServiceCollection AddFabio(this IServiceCollection services, IConfiguration configuration)
    {
        var section = configuration.GetSection("fabio");
        var options = section.BindOptions<FabioOptions>();
        services.Configure<FabioOptions>(section);

        if (!options.Enabled)
        {
            return services;
        }

        if (string.IsNullOrWhiteSpace(options.Url))
        {
            throw new ArgumentException("Fabio URL cannot be empty.", nameof(options.Url));
        }
        
        services.AddTransient<FabioHttpHandler>();
        services.AddSingleton<IServiceDiscoveryRegistration, FabioServiceDiscoveryRegistration>();

        return services;
    }

    public static IHttpClientBuilder AddFabioHandler(this IHttpClientBuilder builder)
        => builder.AddHttpMessageHandler<FabioHttpHandler>();
}