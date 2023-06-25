using Microsoft.EntityFrameworkCore;
using MySpot.Services.ParkingSpots.Core.DAL;
using MySpot.Services.ParkingSpots.Core.Entities;
using MySpot.Services.ParkingSpots.Core.Exceptions;

namespace MySpot.Services.ParkingSpots.Core.Services;

internal sealed class ParkingSpotsService : IParkingSpotsService
{
    private const int ParkingSpotCapacity = 2;
    private readonly DbSet<ParkingSpot> _parkingSpots;
    private readonly ParkingSpotsDbContext _context;

    public ParkingSpotsService(ParkingSpotsDbContext context)
    {
        _context = context;
        _parkingSpots = context.ParkingSpots;
    }

    public async Task<IEnumerable<ParkingSpot>> GetAllAsync()
        => await _parkingSpots.OrderBy(x => x.DisplayOrder).ToListAsync();

    public async Task AddAsync(ParkingSpot parkingSpot)
    {
        await _parkingSpots.AddAsync(parkingSpot);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(ParkingSpot parkingSpot)
    {
        var existingParkingSpot = await _parkingSpots.SingleOrDefaultAsync(x => x.Id == parkingSpot.Id);
        if (existingParkingSpot is null)
        {
            throw new ParkingSpotNotFoundException(parkingSpot.Id);
        }

        existingParkingSpot.Name = parkingSpot.Name;
        existingParkingSpot.DisplayOrder = parkingSpot.DisplayOrder;
        _parkingSpots.Update(parkingSpot);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid parkingSpotId)
    {
        var parkingSpot = await _parkingSpots.SingleOrDefaultAsync(x => x.Id == parkingSpotId);
        if (parkingSpot is null)
        {
            throw new ParkingSpotNotFoundException(parkingSpotId);
        }

        _parkingSpots.Remove(parkingSpot);
        await _context.SaveChangesAsync();
    }
}