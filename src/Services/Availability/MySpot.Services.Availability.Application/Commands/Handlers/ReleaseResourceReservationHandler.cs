using Micro.Handlers;
using Micro.Messaging.Brokers;
using MySpot.Services.Availability.Application.Events;
using MySpot.Services.Availability.Application.Exceptions;
using MySpot.Services.Availability.Core.Repositories;
using MySpot.Services.Availability.Core.ValueObjects;

namespace MySpot.Services.Availability.Application.Commands.Handlers;

internal sealed class ReleaseResourceReservationHandler : ICommandHandler<ReleaseResourceReservation>
{
    private readonly IResourcesRepository _repository;
    private readonly IMessageBroker _messageBroker;

    public ReleaseResourceReservationHandler(IResourcesRepository repository, IMessageBroker messageBroker)
    {
        _repository = repository;
        _messageBroker = messageBroker;
    }
        
    public async Task HandleAsync(ReleaseResourceReservation command, CancellationToken cancellationToken = default)
    {
        var (resourceId, date) = command;
        var resource = await _repository.GetAsync(resourceId);
        if (resource is null)
        {
            throw new ResourceNotFoundException(resourceId);
        }

        var reservationDate = new Date(date);
        var reservation = resource.Reservations.FirstOrDefault(r => r.Date == reservationDate);
        if (reservation is null)
        {
            throw new ReservationNotFoundException(date);
        }
        
        resource.ReleaseReservation(reservation);
        await _repository.UpdateAsync(resource);
        await _messageBroker.SendAsync(new ResourceReservationReleased(resourceId, date), cancellationToken);
    }
}