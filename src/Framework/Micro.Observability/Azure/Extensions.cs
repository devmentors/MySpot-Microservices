using Azure.Monitor.OpenTelemetry.AspNetCore;
using Micro.Observability.Metrics;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Micro.Observability.Azure;

public static class Extensions
{
    private const string AppSectionName = "app";
    
    public static IServiceCollection AddApplicationInsights(this IServiceCollection services, IConfiguration configuration)
    {
        var section = configuration.GetSection("applicationInsights");
        var options = section.BindOptions<ApplicationInsightsOptions>();
        services.Configure<ApplicationInsightsOptions>(section);
        if (!options.Enabled)
        {
            return services;
        }
        
        services.AddApplicationInsightsTelemetry();
        var appOptions = configuration.BindOptions<AppOptions>(AppSectionName);
        services.Configure<TelemetryConfiguration>(x => x.TelemetryInitializers.Add(new ServiceNameInitializer(appOptions.Name)));
        services.Configure<AzureMonitorOptions>(x => x.ConnectionString = options.ConnectionString);
        
        return services;
    }

    public static IApplicationBuilder UseObservability(this IApplicationBuilder app)
        => app.UseMetrics();
}