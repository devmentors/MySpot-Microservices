using Micro.Framework;
using Micro.Messaging.Brokers;
using MySpot.Gateway.Api.Messages;

var builder = WebApplication
    .CreateBuilder(args)
    .AddMicroFramework();

builder.Configuration.AddJsonFile("yarp.json", false);

builder.Services
    .AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetRequiredSection("reverseProxy"));

var app = builder.Build();

app.MapGet("/", (AppInfo appInfo) => appInfo).WithTags("API").WithName("Info");

app.MapGet("/ping", () => "pong").WithTags("API").WithName("Pong");

app.MapPost("/async/account/sign-up", async (SignUp command, IMessageBroker messageBroker) =>
{
    await messageBroker.SendAsync(command);
    return Results.Accepted();
});

app.UseMicroFramework()
    .MapReverseProxy();

app.Run();
