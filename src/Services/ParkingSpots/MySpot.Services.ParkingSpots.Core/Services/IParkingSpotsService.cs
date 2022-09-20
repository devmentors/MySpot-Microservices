using MySpot.Services.ParkingSpots.Core.Entities;

namespace MySpot.Services.ParkingSpots.Core.Services;

public interface IParkingSpotsService
{
    Task<IEnumerable<ParkingSpot>> GetAllAsync();
    Task AddAsync(ParkingSpot parkingSpot);
    Task UpdateAsync(ParkingSpot parkingSpot);
    Task DeleteAsync(Guid parkingSpotId);
}