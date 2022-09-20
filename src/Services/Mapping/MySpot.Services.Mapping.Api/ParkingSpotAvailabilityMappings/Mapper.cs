using Micro.Handlers;
using Micro.Messaging.Brokers;
using MySpot.Services.Mapping.Api.ParkingSpotAvailabilityMappings.Commands;
using MySpot.Services.Mapping.Api.ParkingSpotAvailabilityMappings.Events;

namespace MySpot.Services.Mapping.Api.ParkingSpotAvailabilityMappings;

internal sealed class Mapper : IEventHandler<ParkingSpotCreated>, IEventHandler<ParkingSpotDeleted>
{
    private const int ParkingSpotCapacity = 2;
    private readonly IMessageBroker _messageBroker;

    public Mapper(IMessageBroker messageBroker)
        => _messageBroker = messageBroker;

    public async Task HandleAsync(ParkingSpotCreated @event, CancellationToken cancellationToken = default)
    {
        var tags = new[] {"parking_spot"};
        await _messageBroker.SendAsync(new AddResource(@event.ParkingSpotId, ParkingSpotCapacity, tags) , cancellationToken);
    }

    public async Task HandleAsync(ParkingSpotDeleted @event, CancellationToken cancellationToken = default)
    {
        await _messageBroker.SendAsync(new DeleteResource(@event.ParkingSpotId) , cancellationToken);
    }
}