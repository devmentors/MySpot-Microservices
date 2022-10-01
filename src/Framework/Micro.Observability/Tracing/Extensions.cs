using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OpenTelemetry;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace Micro.Observability.Tracing;

public static class Extensions
{
    private const string ConsoleExporter = "console";
    private const string JaegerExporter = "jaeger";

    public static IServiceCollection AddTracing(this IServiceCollection services,
        IConfiguration configuration)
    {
        var tracingSection = configuration.GetSection("tracing");
        var tracingOptions = tracingSection.BindOptions<TracingOptions>();
        services.Configure<TracingOptions>(tracingSection);

        if (!tracingOptions.Enabled)
        {
            return services;
        }

        var appName = configuration.BindOptions<AppOptions>("app").Name;
        if (string.IsNullOrWhiteSpace(appName))
        {
            throw new InvalidOperationException("Application name cannot be empty when using the tracing.");
        }

        services.AddOpenTelemetryTracing(builder =>
        {
            builder.SetResourceBuilder(ResourceBuilder.CreateDefault()
                    .AddTelemetrySdk()
                    .AddEnvironmentVariableDetector()
                    .AddService(appName))
                .AddSource(appName)
                .AddAspNetCoreInstrumentation()
                .AddHttpClientInstrumentation()
                .AddSqlClientInstrumentation();

            switch (tracingOptions.Exporter.ToLowerInvariant())
            {
                case ConsoleExporter:
                {
                    builder.AddConsoleExporter();
                    break;
                }
                case JaegerExporter:
                {
                    var jaegerOptions = tracingOptions.Jaeger;
                    builder.AddJaegerExporter(jaeger =>
                    {
                        jaeger.AgentHost = jaegerOptions.AgentHost;
                        jaeger.AgentPort = jaegerOptions.AgentPort;
                        jaeger.MaxPayloadSizeInBytes = jaegerOptions.MaxPayloadSizeInBytes;
                        if (!Enum.TryParse<ExportProcessorType>(jaegerOptions.ExportProcessorType, true,
                                out var exportProcessorType))
                        {
                            exportProcessorType = ExportProcessorType.Batch;
                        }

                        jaeger.ExportProcessorType = exportProcessorType;
                    });
                    break;
                }
            }
        });

        return services;
    }
}