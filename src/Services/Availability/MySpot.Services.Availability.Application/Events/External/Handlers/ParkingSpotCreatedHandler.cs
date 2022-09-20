using Micro.Handlers;
using Micro.Messaging.Brokers;
using MySpot.Services.Availability.Application.Exceptions;
using MySpot.Services.Availability.Core.Entities;
using MySpot.Services.Availability.Core.Repositories;
using MySpot.Services.Availability.Core.ValueObjects;

namespace MySpot.Services.Availability.Application.Events.External.Handlers;

internal sealed class ParkingSpotCreatedHandler : IEventHandler<ParkingSpotCreated>
{
    private readonly IResourcesRepository _resourcesRepository;
    private readonly IMessageBroker _messageBroker;

    public ParkingSpotCreatedHandler(IResourcesRepository resourcesRepository, IMessageBroker messageBroker)
    {
        _resourcesRepository = resourcesRepository;
        _messageBroker = messageBroker;
    }
    
    public async Task HandleAsync(ParkingSpotCreated @event, CancellationToken cancellationToken = default)
    {
        var resourceId = @event.ParkingSpotId;
        if (await _resourcesRepository.ExistsAsync(resourceId))
        {
            throw new ResourceAlreadyExistsException(resourceId);
        }

        var resource = Resource.Create(resourceId, 2, new []{new Tag("parking_spot")});
        await _resourcesRepository.AddAsync(resource);
        await _messageBroker.SendAsync(new ResourceAdded(resourceId), cancellationToken);
    }
}