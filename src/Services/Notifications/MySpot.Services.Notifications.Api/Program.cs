using Micro.DAL.Postgres;
using Micro.Framework;
using Micro.Handlers;
using Micro.Messaging;
using MySpot.Services.Notifications.Api.Clients;
using MySpot.Services.Notifications.Api.Commands;
using MySpot.Services.Notifications.Api.DAL;
using MySpot.Services.Notifications.Api.Events.External;

var builder = WebApplication
    .CreateBuilder(args)
    .AddMicroFramework();

builder.Services
    .AddPostgres<NotificationsDbContext>(builder.Configuration)
    .AddInitializer<NotificationsDataInitializer>()
    .AddSingleton<IEmailApiClient, EmailApiClient>();

var app = builder.Build();

app.MapGet("/", (AppInfo appInfo) => appInfo).WithTags("API").WithName("Info");

app.MapGet("/ping", () => "pong").WithTags("API").WithName("Pong");

app.MapPost("/emails/send", async (SendEmail command, IDispatcher dispatcher) =>
{
    await dispatcher.SendAsync(command);
    return Results.NoContent();
}).WithTags("Emails").WithName("Send email");;

app.Subscribe()
    .Command<SendEmail>()
    .Event<SignedUp>();

app.UseMicroFramework().Run();
