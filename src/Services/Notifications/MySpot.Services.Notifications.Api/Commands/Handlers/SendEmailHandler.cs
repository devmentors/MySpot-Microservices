using Micro.Handlers;
using Microsoft.EntityFrameworkCore;
using MySpot.Services.Notifications.Api.Clients;
using MySpot.Services.Notifications.Api.DAL;
using MySpot.Services.Notifications.Api.Exceptions;

namespace MySpot.Services.Notifications.Api.Commands.Handlers;

internal sealed class SendEmailHandler : ICommandHandler<SendEmail>
{
    private readonly NotificationsDbContext _dbContext;
    private readonly IEmailApiClient _emailApiClient;
    private readonly ILogger<SendEmailHandler> _logger;

    public SendEmailHandler(NotificationsDbContext dbContext, IEmailApiClient emailApiClient,
        ILogger<SendEmailHandler> logger)
    {
        _dbContext = dbContext;
        _emailApiClient = emailApiClient;
        _logger = logger;
    }

    public async Task HandleAsync(SendEmail command, CancellationToken cancellationToken = default)
    {
        var (userId, templateName) = command;
        var user = await _dbContext.Users.SingleOrDefaultAsync(x => x.Id == userId, cancellationToken);
        if (user is null)
        {
            throw new UserNotFoundException(userId);
        }

        var template = await _dbContext.Templates.SingleOrDefaultAsync(x => x.Name == templateName, cancellationToken);
        if (template is null)
        {
            throw new TemplateNotFoundException(templateName);
        }

        _logger.LogInformation($"Sending an email using template: '{template.Name}' to user: '{user.Email}'...");
        await _emailApiClient.SendAsync(user.Email, template.Title, template.Body);
        _logger.LogInformation($"Sent an email using template: '{template.Name}' to user: '{user.Email}'.");
    }
}