using Micro.API.CORS;
using Micro.API.Exceptions;
using Micro.API.Networking;
using Micro.API.Swagger;
using Micro.Auth;
using Micro.Contexts;
using Micro.DAL.Mongo;
using Micro.DAL.Redis;
using Micro.Dispatchers;
using Micro.HTTP;
using Micro.Observability.Logging;
using Micro.Security;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace Micro.Framework;

public static class Extensions
{
    public static WebApplicationBuilder AddMicroFramework(this WebApplicationBuilder builder)
    {
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
            .AddHeadersForwarding(builder.Configuration)
            .AddSecurity(builder.Configuration)
            .AddLogger(builder.Configuration)
            .AddHttpClient(builder.Configuration)
            .AddMongo(builder.Configuration)
            .AddRedis(builder.Configuration);

        return builder;
    }

    public static WebApplication UseMicroFramework(this WebApplication app)
    {
        app
            .UseHeadersForwarding()
            .UseCorsPolicy()
            .UseErrorHandling()
            .UseSwaggerDocs()
            .UseAuthentication()
            .UseRouting()
            .UseAuthorization()
            .UseContextLogger()
            .UseSerilogRequestLogging();

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