using MySpot.Services.Reservations.Application.Clients.DTO;

namespace MySpot.Services.Reservations.Application.Clients;

public interface IAvailabilityApiClient
{
    Task<ResourceDto?> GetResourceAsync(Guid resourceId);
}