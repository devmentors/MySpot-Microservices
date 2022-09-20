using Micro.Handlers;
using Micro.Messaging.Brokers;
using MySpot.Services.Availability.Application.Events;
using MySpot.Services.Availability.Application.Exceptions;
using MySpot.Services.Availability.Core.Entities;
using MySpot.Services.Availability.Core.Repositories;

namespace MySpot.Services.Availability.Application.Commands.Handlers;

internal sealed class ReserveResourceHandler : ICommandHandler<ReserveResource>
{
    private readonly IResourcesRepository _repository;
    private readonly IMessageBroker _messageBroker;

    public ReserveResourceHandler(IResourcesRepository repository, IMessageBroker messageBroker)
    {
        _repository = repository;
        _messageBroker = messageBroker;
    }

    public async Task HandleAsync(ReserveResource command, CancellationToken cancellationToken = default)
    {
        var (resourceId, reservationId, capacity, date, priority) = command;
        var resource = await _repository.GetAsync(resourceId);
        if (resource is null)
        {
            throw new ResourceNotFoundException(resourceId);
        }

        var reservation = new Reservation(reservationId, capacity, date, priority);
        resource.AddReservation(reservation);
        await _repository.UpdateAsync(resource);
        await _messageBroker.SendAsync(new ResourceReserved(resourceId, date), cancellationToken);
    }
}