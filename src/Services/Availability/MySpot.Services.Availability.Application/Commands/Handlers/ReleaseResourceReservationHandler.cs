using Micro.Handlers;
using MySpot.Services.Availability.Application.Exceptions;
using MySpot.Services.Availability.Core.Repositories;
using MySpot.Services.Availability.Core.ValueObjects;

namespace MySpot.Services.Availability.Application.Commands.Handlers;

internal sealed class ReleaseResourceReservationHandler : ICommandHandler<ReleaseResourceReservation>
{
    private readonly IResourcesRepository _repository;

    public ReleaseResourceReservationHandler(IResourcesRepository repository)
    {
        _repository = repository;
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
    }
}