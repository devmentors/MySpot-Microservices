using Chronicle;
using Micro.Framework;
using Micro.Messaging;

var builder = WebApplication
    .CreateBuilder(args)
    .AddMicroFramework();

builder.Services
    .AddChronicle();

var app = builder.Build();

app.MapGet("/", (AppInfo appInfo) => appInfo).WithTags("API").WithName("Info");

app.MapGet("/ping", () => "pong").WithTags("API").WithName("Pong");

app.UseMicroFramework()
    .Subscribe();

app.Run();
