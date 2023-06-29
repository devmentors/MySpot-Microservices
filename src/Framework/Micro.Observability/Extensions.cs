using Azure.Monitor.OpenTelemetry.AspNetCore;
using Micro.Observability.Azure;
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
        var appInsightsOptions = configuration.BindOptions<ApplicationInsightsOptions>("applicationInsights");
        var builder = services
            .AddApplicationInsights(configuration)
            .AddOpenTelemetry()
            .AddMetrics(services, configuration)
            .AddTracing(services, configuration);

        if (!appInsightsOptions.Enabled)
        {
            return services;
        }

        if (string.IsNullOrWhiteSpace(appInsightsOptions.ConnectionString))
        {
            throw new ArgumentException("Application Insights connection string is empty.");
        }

        builder.UseAzureMonitor();

        return services;
    }

    public static IApplicationBuilder UseObservability(this IApplicationBuilder app)
        => app.UseMetrics();
}