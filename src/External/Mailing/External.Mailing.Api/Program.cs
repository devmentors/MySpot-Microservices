using External.Mailing.Api;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();
var random = new Random();

app.MapGet("/", () => "External Mailing API");

app.MapPost("send-email", async (SendEmail request, HttpContext context, ILogger<Program> logger) =>
{
    if (string.IsNullOrWhiteSpace(request.Receiver) || string.IsNullOrWhiteSpace(request.Title) ||
        string.IsNullOrWhiteSpace(request.Body))
    {
        context.Response.StatusCode = StatusCodes.Status400BadRequest;
        await context.Response.WriteAsync("Invalid request.");
        return;
    }

    logger.LogInformation($"Sending an email to: '{request.Receiver}'...");
    var willSucceed = random.Next(1, 11) % 10 != 0;
    if (willSucceed)
    {
        await Task.Delay(TimeSpan.FromSeconds(random.Next(1, 6)));
        logger.LogInformation($"Sent an email to: '{request.Receiver}'.");
        return;
    }
    
    await Task.Delay(TimeSpan.FromSeconds(random.Next(1, 4)));
    logger.LogError($"There was an error when sending an email: '{request.Receiver}'.");
    
    context.Response.StatusCode = random.Next(1, 4) switch
    {
        1 => StatusCodes.Status400BadRequest,
        2 => StatusCodes.Status500InternalServerError,
        3 => StatusCodes.Status503ServiceUnavailable,
        _ => StatusCodes.Status400BadRequest
    };
});

app.Run();

namespace External.Mailing.Api
{
    record SendEmail(string Receiver, string Title, string Body);
}