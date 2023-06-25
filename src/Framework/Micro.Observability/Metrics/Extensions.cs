﻿using System.Reflection;
using Micro.Messaging.Brokers;
using Micro.Observability.Metrics.Decorators;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using OpenTelemetry;
using OpenTelemetry.Metrics;

namespace Micro.Observability.Metrics;

internal static class Extensions
{
    private const string ConsoleExporter = "console";
    private const string PrometheusExporter = "prometheus";

    public static OpenTelemetryBuilder AddMetrics(this OpenTelemetryBuilder openTelemetry,
        IServiceCollection services,
        IConfiguration configuration)
    {
        var section = configuration.GetSection("metrics");
        var options = section.BindOptions<MetricsOptions>();
        services.Configure<MetricsOptions>(section);

        if (!options.Enabled)
        {
            return openTelemetry;
        }

        return openTelemetry
            .WithMetrics(builder =>
            {
                builder.AddAspNetCoreInstrumentation();
                builder.AddHttpClientInstrumentation();
                builder.AddRuntimeInstrumentation();

                foreach (var attribute in GetMeterAttributes())
                {
                    if (attribute is not null)
                    {
                        builder.AddMeter(attribute.Key);
                    }
                }

                switch (options.Exporter.ToLowerInvariant())
                {
                    case ConsoleExporter:
                    {
                        builder.AddConsoleExporter();
                        break;
                    }
                    case PrometheusExporter:
                    {
                        var endpoint = options.Endpoint;
                        builder.AddPrometheusExporter(prometheus => { prometheus.ScrapeEndpointPath = endpoint; });
                        break;
                    }
                }
            });
    }

    public static IApplicationBuilder UseMetrics(this IApplicationBuilder app)
    {
        var metricsOptions = app.ApplicationServices.GetRequiredService<IOptions<MetricsOptions>>().Value;
        if (!metricsOptions.Enabled)
        {
            return app;
        }

        if (metricsOptions.Exporter.ToLowerInvariant() is not PrometheusExporter)
        {
            return app;
        }

        app.UseOpenTelemetryPrometheusScrapingEndpoint();

        return app;
    }

    private static IEnumerable<MeterAttribute?> GetMeterAttributes()
        => AppDomain.CurrentDomain.GetAssemblies()
            .Where(x => !x.IsDynamic)
            .SelectMany(x => x.GetTypes())
            .Where(x => x.IsClass && x.GetCustomAttribute<MeterAttribute>() is not null)
            .Select(x => x.GetCustomAttribute<MeterAttribute>())
            .Where(x => x is not null);
    
    public static IServiceCollection AddMessagingMetricsDecorators(this IServiceCollection services)
    {
        services.TryDecorate<IMessageBroker, MessageBrokerMetricsDecorator>();

        return services;
    }
}