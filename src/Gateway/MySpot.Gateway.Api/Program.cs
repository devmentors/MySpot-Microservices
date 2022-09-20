using Micro.Contexts;
using Micro.Framework;
using Micro.Messaging.Brokers;
using MySpot.Gateway.Api.Messages;
using Yarp.ReverseProxy.Transforms;

var builder = WebApplication
    .CreateBuilder(args)
    .AddMicroFramework();

builder.Host
    .ConfigureAppConfiguration(cfg => cfg.AddJsonFile("yarp.json", false));

builder.Services
    .AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetRequiredSection("reverseProxy"))
    .AddTransforms(builderContext =>
    {
        builderContext.AddRequestTransform(transformContext =>
        {
            var correlationId = transformContext.HttpContext.GetCorrelationId() ?? Guid.NewGuid().ToString("N");
            transformContext.ProxyRequest.Headers.Add("correlation-id", correlationId);
            return ValueTask.CompletedTask;
        });
    });

var app = builder.Build();

app.MapGet("/", (AppInfo appInfo) => appInfo).WithTags("API").WithName("Info");

app.MapGet("/ping", () => "pong").WithTags("API").WithName("Pong");

app.MapPost("/async/account/sign-up", async (SignUp command, IMessageBroker messageBroker) =>
{
    await messageBroker.SendAsync(command);
    return Results.Accepted();
});

app.UseMicroFramework()
    .UseEndpoints(x => x.MapReverseProxy());

app.Run();
