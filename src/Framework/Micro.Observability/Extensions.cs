using Micro.Observability.Metrics;
using Micro.Observability.Tracing;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Micro.Observability;

public static class Extensions
{
    public static IServiceCollection AddObservability(this IServiceCollection services, IConfiguration configuration)
    {
        services
            .AddOpenTelemetry()
            .AddMetrics(services, configuration)
            .AddTracing(services, configuration);

        return services;
    }

    public static IApplicationBuilder UseObservability(this IApplicationBuilder app)
        => app.UseMetrics();
}