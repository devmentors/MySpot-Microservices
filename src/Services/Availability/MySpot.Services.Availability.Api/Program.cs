using Micro.Framework;
using Micro.Handlers;
using Micro.Messaging;
using MySpot.Services.Availability.Application;
using MySpot.Services.Availability.Application.Commands;
using MySpot.Services.Availability.Application.Queries;
using MySpot.Services.Availability.Infrastructure;

var builder = WebApplication
    .CreateBuilder(args)
    .AddMicroFramework();

builder.Services
    .AddApplication()
    .AddInfrastructure(builder.Configuration);

var app = builder.Build();

app.MapGet("/", (AppInfo appInfo) => appInfo).WithTags("API").WithName("Info");

app.MapGet("/ping", () => "pong").WithTags("API").WithName("Pong");

app.MapGet("/resources", async (IDispatcher dispatcher)
        => Results.Ok(await dispatcher.QueryAsync(new GetResources())))
    .WithTags("Resources").WithName("Get resources");

app.MapGet("/resources/{id:guid}", async (Guid id, IDispatcher dispatcher) =>
{
    var resource = await dispatcher.QueryAsync(new GetResource {ResourceId = id});
    return resource is null ? Results.NotFound() : Results.Ok(resource);
}).WithTags("Resources").WithName("Get resource");

app.MapPost("/resources", async (AddResource command, IDispatcher dispatcher) =>
{
    await dispatcher.SendAsync(command);
    return Results.CreatedAtRoute("Get resource", new  {id = command.ResourceId});
}).WithTags("Resources").WithName("Add resource");

app.Subscribe()
    .Command<AddResource>()
    .Command<DeleteResource>()
    .Command<ReserveResource>()
    .Command<ReleaseResourceReservation>();

app.UseMicroFramework().Run();
