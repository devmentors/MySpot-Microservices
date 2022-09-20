using Chronicle;
using Micro.Handlers;
using MySpot.Saga.Api.Messages;

namespace MySpot.Saga.Api;

internal sealed class MessagesHandler :
    IEventHandler<ParkingSpotReserved>,
    IEventHandler<ParkingSpotReservationRemoved>,
    IEventHandler<ResourceReservationFailed>,
    IEventHandler<ResourceReserved>
{
    private readonly ISagaCoordinator _sagaCoordinator;

    public MessagesHandler(ISagaCoordinator sagaCoordinator)
    {
        _sagaCoordinator = sagaCoordinator;
    }

    public Task HandleAsync(ParkingSpotReserved @event, CancellationToken cancellationToken = default)
        => ProcessAsync(@event);

    public Task HandleAsync(ResourceReservationFailed @event, CancellationToken cancellationToken = default)
        => ProcessAsync(@event);

    public Task HandleAsync(ParkingSpotReservationRemoved @event, CancellationToken cancellationToken = default)
        => ProcessAsync(@event);

    public Task HandleAsync(ResourceReserved @event, CancellationToken cancellationToken = default)
        => ProcessAsync(@event);

    private Task ProcessAsync<T>(T message) where T : class =>
        _sagaCoordinator.ProcessAsync(message, SagaContext.Empty);
}