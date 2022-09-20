using Micro.Framework;
using Micro.Messaging;
using MySpot.Services.Mapping.Api.ParkingSpotAvailabilityMappings.Events;

var builder = WebApplication
    .CreateBuilder(args)
    .AddMicroFramework();

var app = builder.Build();

app.MapGet("/", (AppInfo appInfo) => appInfo).WithTags("API").WithName("Info");

app.MapGet("/ping", () => "pong").WithTags("API").WithName("Pong");

app.Subscribe()
    .Event<ParkingSpotCreated>()
    .Event<ParkingSpotDeleted>();

app.UseMicroFramework().Run();

