using Micro.Framework;
using MySpot.Services.ParkingSpots.Core;
using MySpot.Services.ParkingSpots.Core.Entities;
using MySpot.Services.ParkingSpots.Core.Services;

var builder = WebApplication
    .CreateBuilder(args)
    .AddMicroFramework();

builder.Services.AddCore(builder.Configuration);

var app = builder.Build();

app.MapGet("/", (AppInfo appInfo) => appInfo).WithTags("API").WithName("Info");

app.MapGet("/ping", () => "pong").WithTags("API").WithName("Pong");

app.MapGet("/parking-spots", (IParkingSpotsService service) => service.GetAllAsync())
    .WithTags("Parking spots").WithName("Get parking spots");

app.MapPost("/parking-spots", async (ParkingSpot parkingSpot, IParkingSpotsService service) =>
{
    parkingSpot.Id = Guid.NewGuid();
    await service.AddAsync(parkingSpot);
    return Results.NoContent();
}).WithTags("Parking spots").WithName("Add parking spot");

app.MapPut("/parking-spots/{id:guid}", async (Guid id, ParkingSpot parkingSpot, IParkingSpotsService service) =>
{
    parkingSpot.Id = id;
    await service.UpdateAsync(parkingSpot);
    return Results.NoContent();
}).WithTags("Parking spots").WithName("Update parking spot");

app.MapDelete("/parking-spots/{id:guid}", async (Guid id, IParkingSpotsService service) =>
{
    await service.DeleteAsync(id);
    return Results.NoContent();
}).WithTags("Parking spots").WithName("Delete parking spot");

app.UseMicroFramework().Run();
