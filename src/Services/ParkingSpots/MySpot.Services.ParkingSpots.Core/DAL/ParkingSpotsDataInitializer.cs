using Micro.DAL.Postgres;
using Micro.Messaging.Brokers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MySpot.Services.ParkingSpots.Core.Entities;
using MySpot.Services.ParkingSpots.Core.Events;

namespace MySpot.Services.ParkingSpots.Core.DAL;

internal sealed class ParkingSpotsDataInitializer : IDataInitializer
{
    private readonly ParkingSpotsDbContext _dbContext;
    private readonly IMessageBroker _messageBroker;
    private readonly ILogger<ParkingSpotsDataInitializer> _logger;

    public ParkingSpotsDataInitializer(ParkingSpotsDbContext dbContext, IMessageBroker messageBroker,
        ILogger<ParkingSpotsDataInitializer> logger)
    {
        _dbContext = dbContext;
        _messageBroker = messageBroker;
        _logger = logger;
    }

    public async Task InitAsync()
    {
        if (await _dbContext.ParkingSpots.AnyAsync())
        {
            return;
        }

        await AddParkingSpotsAsync();
        await _dbContext.SaveChangesAsync();
    }

    private async Task AddParkingSpotsAsync()
    {
        var parkingSpots = Enumerable.Range(1, 10).Select(i => new ParkingSpot
        {
            Id = Guid.NewGuid(),
            Name = $"P{i}",
            DisplayOrder = i
        }).ToList();

        await _dbContext.ParkingSpots.AddRangeAsync(parkingSpots);
        _logger.LogInformation("Initialized parking spots.");

        foreach (var parkingSpot in parkingSpots)
        {
            await _messageBroker.SendAsync(new ParkingSpotCreated(parkingSpot.Id));
        }
    }
}