using Micro.Handlers;
using MySpot.Services.Notifications.Api.DAL;
using MySpot.Services.Notifications.Api.Entities;

namespace MySpot.Services.Notifications.Api.Events.External.Handlers;

internal sealed class SignedUpHandler : IEventHandler<SignedUp>
{
    private readonly NotificationsDbContext _dbContext;
    private readonly ILogger<SignedUpHandler> _logger;

    public SignedUpHandler(NotificationsDbContext dbContext, ILogger<SignedUpHandler> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }
    
    public async Task HandleAsync(SignedUp @event, CancellationToken cancellationToken = default)
    {
        await _dbContext.Users.AddAsync(new User(@event.UserId, @event.Email), cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
        _logger.LogInformation($"Added the user with ID: '{@event.UserId}', email: '{@event.Email}'.");
    }
}