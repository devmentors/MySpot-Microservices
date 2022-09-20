using Micro.Framework;
using Micro.Handlers;
using Micro.Messaging;
using MySpot.Services.Users.Core;
using MySpot.Services.Users.Core.Commands;
using MySpot.Services.Users.Core.Queries;
using MySpot.Services.Users.Core.Services;

var builder = WebApplication
    .CreateBuilder(args)
    .AddMicroFramework();

builder.Services.AddCore(builder.Configuration);

var app = builder.Build();

app.MapGet("/", (AppInfo appInfo) => appInfo).WithTags("API").WithName("Info");

app.MapGet("/ping", () => "pong").WithTags("API").WithName("Pong");

app.MapGet("/users/{id:guid}", async (Guid id, IDispatcher dispatcher) =>
{
    var user = await dispatcher.QueryAsync(new GetUser {UserId = id});
    return user is null ? Results.NotFound() : Results.Ok(user);
}).WithTags("Users").WithName("Get user");

app.MapGet("/me", async (IDispatcher dispatcher, HttpContext context) =>
{
    var user = await dispatcher.QueryAsync(new GetUser {UserId = UserId(context)});
    return user is null ? Results.NotFound() : Results.Ok(user);
}).RequireAuthorization().WithTags("Account").WithName("Get account");

app.MapPost("/sign-up", async (SignUp command, IDispatcher dispatcher) =>
{
    await dispatcher.SendAsync(command with {UserId = Guid.NewGuid()});
    return Results.NoContent();
}).WithTags("Account").WithName("Sign up");

app.MapPost("/sign-in", async (SignIn command, IDispatcher dispatcher, ITokenStorage storage) =>
{
    await dispatcher.SendAsync(command);
    var jwt = storage.Get();
    return Results.Ok(jwt);
}).WithTags("Account").WithName("Sign in");


app.UseMicroFramework()
    .Subscribe()
    .Command<SignUp>();

app.Run();

static Guid UserId(HttpContext context)
    => string.IsNullOrWhiteSpace(context.User.Identity?.Name) ? Guid.Empty : Guid.Parse(context.User.Identity.Name);
