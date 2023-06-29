using Micro.API.AsyncApi;
using Micro.API.CORS;
using Micro.API.Exceptions;
using Micro.API.Networking;
using Micro.API.Swagger;
using Micro.Auth;
using Micro.Contexts;
using Micro.Dispatchers;
using Micro.HTTP;
using Micro.HTTP.LoadBalancing;
using Micro.HTTP.ServiceDiscovery;
using Micro.Messaging;
using Micro.Messaging.Azure.ServiceBus;
using Micro.Messaging.RabbitMQ;
using Micro.Messaging.RabbitMQ.Streams;
using Micro.Observability;
using Micro.Observability.Logging;
using Micro.Security;
using Micro.Security.Vault;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace Micro.Framework;

public static class Extensions
{
    public static WebApplicationBuilder AddMicroFramework(this WebApplicationBuilder builder)
    {
        builder.AddVault();
        
        var appOptions = builder.Configuration.GetSection("app").BindOptions<AppOptions>();
        var appInfo = new AppInfo(appOptions.Name, appOptions.Version);
        builder.Services.AddSingleton(appInfo);
        
        RenderLogo(appOptions);

        builder
            .AddLogging()
            .Services
            .AddErrorHandling()
            .AddHandlers(appOptions.Project)
            .AddDispatchers()
            .AddContexts()
            .AddMemoryCache()
            .AddHttpContextAccessor()
            .AddMicro(builder.Configuration)
            .AddAuth(builder.Configuration)
            .AddCorsPolicy(builder.Configuration)
            .AddSwaggerDocs(builder.Configuration)
            .AddAsyncApiDocs(builder.Configuration)
            .AddHeadersForwarding(builder.Configuration)
            .AddMessaging(builder.Configuration)
            .AddAzureServiceBus(builder.Configuration)
            .AddRabbitMQ(builder.Configuration)
            .AddRabbitMQStreams(builder.Configuration)
            .AddConsul(builder.Configuration)
            .AddFabio(builder.Configuration)
            .AddSecurity(builder.Configuration)
            .AddLogger(builder.Configuration)
            .AddObservability(builder.Configuration);

        builder.Services
            .AddHttpClient(builder.Configuration)
            .AddVaultCertificatesHandler(builder.Configuration);
        // .AddConsulHandler()
        // .AddFabioHandler();

        // builder.Services
        //     .AddMessagingMetricsDecorators()
        //     .AddMessagingTracingDecorators();

        return builder;
    }

    public static WebApplication UseMicroFramework(this WebApplication app)
    {
        Observability.Azure.Extensions.UseObservability(app
                .UseHeadersForwarding()
                .UseCorsPolicy()
                .UseErrorHandling()
                .UseSwaggerDocs()
                .UseAuthentication()
                .UseRouting())
            .UseAuthorization()
            .UseContextLogger()
            .UseSerilogRequestLogging()
            .UseEndpoints(endpoints => endpoints.MapAsyncApiDocs(app.Configuration));

        return app;
    }

    private static void RenderLogo(AppOptions app)
    {
        if (string.IsNullOrWhiteSpace(app.Name))
        {
            return;
        }

        Console.WriteLine(Figgle.FiggleFonts.Slant.Render($"{app.Name} {app.Version}"));
    }
}