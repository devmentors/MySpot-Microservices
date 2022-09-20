using Micro.Handlers;
using Microsoft.Extensions.Logging;
using MySpot.Services.Reservations.Core.Entities;
using MySpot.Services.Reservations.Core.Repository;
using MySpot.Services.Reservations.Core.ValueObjects;

namespace MySpot.Services.Reservations.Application.Events.External.Handlers;

internal sealed class SignedUpHandler : IEventHandler<SignedUp>
{
    private readonly ILogger<SignedUpHandler> _logger;
    private readonly IUserRepository _userRepository;

    public SignedUpHandler(IUserRepository userRepository, ILogger<SignedUpHandler> logger)
    {
        _userRepository = userRepository;
        _logger = logger;
    }
    
    public async Task HandleAsync(SignedUp @event, CancellationToken cancellationToken = default)
    {
        var jobTitle = @event.JobTitle switch
        {
            "employee" => JobTitle.Employee,
            "manager" => JobTitle.Manager,
            "boss" => JobTitle.Boss,
            _ => JobTitle.None
        };
        
        await _userRepository.AddAsync(new User(@event.UserId, jobTitle), cancellationToken);
        _logger.LogInformation($"Added the user with ID: '{@event.UserId}'.");
    }
}