using Chronicle;
using Micro.Messaging.Brokers;
using MySpot.Saga.Api.Messages;

namespace MySpot.Saga.Api.Sagas;

internal sealed class ReservationSagaData
{
    public Guid ReservationId { get; set; }
    public Guid UserId { get; set; }
}

internal sealed class ReservationSaga : Saga<ReservationSagaData>,
    ISagaStartAction<ParkingSpotReserved>,
    ISagaAction<ResourceReserved>,
    ISagaAction<ResourceReservationFailed>,
    ISagaAction<ParkingSpotReservationRemoved>
{
    private readonly IMessageBroker _messageBroker;

    public ReservationSaga(IMessageBroker messageBroker) => _messageBroker = messageBroker;

    public override SagaId ResolveId(object message, ISagaContext context)
        => message switch
        {
            ParkingSpotReserved m => $"{m.ParkingSpotId}:{m.Date:d}",
            ResourceReserved m => $"{m.ResourceId}:{m.Date:d}",
            ResourceReservationFailed m => $"{m.ResourceId}:{m.Date:d}",
            ParkingSpotReservationRemoved m => $"{m.ParkingSpotId}:{m.Date:d}",
            _ => throw new InvalidOperationException("Unsupported message.")
        };

    public async Task HandleAsync(ParkingSpotReserved message, ISagaContext context)
    {
        Data.ReservationId = message.ReservationId;
        Data.UserId = message.UserId;
        await _messageBroker.SendAsync(new ReserveResource(message.ParkingSpotId, message.ReservationId,
            message.Capacity, message.Date, 1));
    }

    public Task CompensateAsync(ParkingSpotReserved message, ISagaContext context)
        => Task.CompletedTask;

    public Task HandleAsync(ResourceReserved message, ISagaContext context)
        => CompleteAsync();

    public Task CompensateAsync(ResourceReserved message, ISagaContext context)
        => Task.CompletedTask;

    public Task HandleAsync(ResourceReservationFailed message, ISagaContext context)
        => _messageBroker.SendAsync(new RemoveReservation(Data.ReservationId, Data.UserId));

    public Task CompensateAsync(ResourceReservationFailed message, ISagaContext context)
        => Task.CompletedTask;

    public Task HandleAsync(ParkingSpotReservationRemoved message, ISagaContext context)
        => CompleteAsync();

    public Task CompensateAsync(ParkingSpotReservationRemoved message, ISagaContext context)
        => Task.CompletedTask;
}