using Micro.Handlers;
using Micro.Observability.Logging.Decorators;
using Micro.Observability.Logging.Middlewares;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;
using Serilog.Filters;

namespace Micro.Observability.Logging;

public static class Extensions
{
    private const string ConsoleOutputTemplate = "{Timestamp:HH:mm:ss} [{Level:u3}] {Message}{NewLine}{Exception}";
    private const string FileOutputTemplate = "{Timestamp:HH:mm:ss} [{Level:u3}] ({SourceContext}.{Method}) {Message}{NewLine}{Exception}";
    private const string AppSectionName = "app";
    private const string SerilogSectionName = "serilog";

    public static IServiceCollection AddLogger(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<SerilogOptions>(configuration.GetSection(SerilogSectionName));
        services.AddSingleton<ContextLoggingMiddleware>();
        services.TryDecorate(typeof(ICommandHandler<>), typeof(LoggingCommandHandlerDecorator<>));
        services.TryDecorate(typeof(IEventHandler<>), typeof(LoggingEventHandlerDecorator<>));

        return services;
    }

    public static IApplicationBuilder UseContextLogger(this IApplicationBuilder app)
        => app.UseMiddleware<ContextLoggingMiddleware>();
    
    public static WebApplicationBuilder AddLogging(this WebApplicationBuilder builder,
        Action<LoggerConfiguration>? configure = null,
        string loggerSectionName = SerilogSectionName, string appSectionName = AppSectionName)
    {
        builder.Host.AddLogging(configure, loggerSectionName, appSectionName);
        return builder;
    }

    private static IHostBuilder AddLogging(this IHostBuilder builder, Action<LoggerConfiguration>? configure = null,
        string loggerSectionName = SerilogSectionName, string appSectionName = AppSectionName)
        => builder.UseSerilog((context, loggerConfiguration) =>
        {
            if (string.IsNullOrWhiteSpace(loggerSectionName))
            {
                loggerSectionName = SerilogSectionName;
            }

            if (string.IsNullOrWhiteSpace(appSectionName))
            {
                appSectionName = AppSectionName;
            }

            var appOptions = context.Configuration.BindOptions<AppOptions>(appSectionName);
            var loggerOptions = context.Configuration.BindOptions<SerilogOptions>(loggerSectionName);

            Configure(loggerOptions, appOptions, loggerConfiguration, context.HostingEnvironment.EnvironmentName);
            configure?.Invoke(loggerConfiguration);
        });

    private static void Configure(SerilogOptions serilogOptions, AppOptions appOptions,
        LoggerConfiguration loggerConfiguration, string environmentName)
    {
        var level = GetLogEventLevel(serilogOptions.Level);

        loggerConfiguration.Enrich.FromLogContext()
            .MinimumLevel.Is(level)
            .Enrich.WithProperty("Environment", environmentName)
            .Enrich.WithProperty("Application", appOptions.Name)
            .Enrich.WithProperty("Version", appOptions.Version);

        foreach (var (key, value) in serilogOptions.Tags)
        {
            loggerConfiguration.Enrich.WithProperty(key, value);
        }

        foreach (var (key, value) in serilogOptions.Overrides)
        {
            var logLevel = GetLogEventLevel(value);
            loggerConfiguration.MinimumLevel.Override(key, logLevel);
        }

        serilogOptions.ExcludePaths?.ToList().ForEach(p => loggerConfiguration.Filter
            .ByExcluding(Matching.WithProperty<string>("RequestPath", n => n.EndsWith(p))));

        serilogOptions.ExcludeProperties?.ToList().ForEach(p => loggerConfiguration.Filter
            .ByExcluding(Matching.WithProperty(p)));

        Configure(loggerConfiguration, serilogOptions);
    }

    private static void Configure(LoggerConfiguration loggerConfiguration, SerilogOptions options)
    {
        var consoleOptions = options.Console;
        var fileOptions = options.File;
        var seqOptions = options.Seq;

        if (consoleOptions.Enabled)
        {
            loggerConfiguration.WriteTo.Console(outputTemplate: ConsoleOutputTemplate);
        }

        if (fileOptions.Enabled)
        {
            var path = string.IsNullOrWhiteSpace(fileOptions.Path) ? "logs/logs.txt" : fileOptions.Path;
            if (!Enum.TryParse<RollingInterval>(fileOptions.Interval, true, out var interval))
            {
                interval = RollingInterval.Day;
            }

            loggerConfiguration.WriteTo.File(path, rollingInterval: interval, outputTemplate: FileOutputTemplate);
        }

        if (seqOptions.Enabled)
        {
            loggerConfiguration.WriteTo.Seq(seqOptions.Url, apiKey: seqOptions.ApiKey);
        }
    }

    private static LogEventLevel GetLogEventLevel(string level)
        => Enum.TryParse<LogEventLevel>(level, true, out var logLevel)
            ? logLevel
            : LogEventLevel.Information;
}