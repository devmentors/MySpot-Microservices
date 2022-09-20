using Micro.Framework;
using Micro.Handlers;
using Micro.Messaging;
using MySpot.Services.Reservations.Application;
using MySpot.Services.Reservations.Application.Commands;
using MySpot.Services.Reservations.Application.Events.External;
using MySpot.Services.Reservations.Application.Queries;
using MySpot.Services.Reservations.Core;
using MySpot.Services.Reservations.Infrastructure;

var builder = WebApplication
    .CreateBuilder(args)
    .AddMicroFramework();

builder.Services
    .AddCore()
    .AddApplication()
    .AddInfrastructure(builder.Configuration);

var app = builder.Build();

app.MapGet("/", (AppInfo appInfo) => appInfo).WithTags("API").WithName("Info");

app.MapGet("/ping", () => "pong").WithTags("API").WithName("Pong");

app.MapGet("/reservations/weekly", async (DateTime? date, IDispatcher dispatcher, HttpContext context) =>
{
    var reservations = await dispatcher.QueryAsync(new GetWeeklyReservations {Date = date, UserId = UserId(context)});
    return reservations is null ? Results.NotFound() : Results.Ok(reservations);
}).RequireAuthorization().WithTags("Reservations").WithName("Get weekly reservations");

app.MapPost("/reservations", async (MakeReservation command, IDispatcher dispatcher, HttpContext context) =>
{
    await dispatcher.SendAsync(command with {UserId = UserId(context)});
    return Results.Accepted();
}).RequireAuthorization().WithTags("Reservations").WithName("Make reservation");

app.MapDelete("/reservations/{id:guid}", async (Guid id, IDispatcher dispatcher, HttpContext context) =>
{
    await dispatcher.SendAsync(new RemoveReservation(id, UserId(context)));
    return Results.NoContent();
}).RequireAuthorization().WithTags("Reservations").WithName("Remove reservation");

app.UseMicroFramework()
    .Subscribe()
    .Command<MakeReservation>()
    .Command<RemoveReservation>()
    .Event<SignedUp>();

app.Run();

static Guid UserId(HttpContext context)
    => string.IsNullOrWhiteSpace(context.User.Identity?.Name) ? Guid.Empty : Guid.Parse(context.User.Identity.Name);
