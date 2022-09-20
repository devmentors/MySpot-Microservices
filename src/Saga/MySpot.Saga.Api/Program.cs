using Chronicle;
using Micro.Framework;
using Micro.Messaging;
using MySpot.Saga.Api.Messages;

var builder = WebApplication
    .CreateBuilder(args)
    .AddMicroFramework();

builder.Services
    .AddChronicle();

var app = builder.Build();

app.MapGet("/", (AppInfo appInfo) => appInfo).WithTags("API").WithName("Info");

app.MapGet("/ping", () => "pong").WithTags("API").WithName("Pong");

app.UseMicroFramework()
    .Subscribe()
    .Event<ResourceReserved>()
    .Event<ResourceReservationFailed>()
    .Event<ParkingSpotReserved>()
    .Event<ParkingSpotReservationRemoved>();

app.Run();
