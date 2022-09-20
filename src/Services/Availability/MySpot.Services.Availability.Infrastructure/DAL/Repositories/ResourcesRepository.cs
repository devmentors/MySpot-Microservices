using Microsoft.EntityFrameworkCore;
using MySpot.Services.Availability.Core.Entities;
using MySpot.Services.Availability.Core.Repositories;

namespace MySpot.Services.Availability.Infrastructure.DAL.Repositories;

internal sealed class ResourcesRepository : IResourcesRepository
{
    private readonly DbSet<Resource> _resources;
    private readonly AvailabilityDbContext _context;

    public ResourcesRepository(AvailabilityDbContext context)
    {
        _resources = context.Resources;
        _context = context;
    }

    public Task<Resource?> GetAsync(AggregateId id)
        => _resources.Include(r => r.Reservations).SingleOrDefaultAsync(r => r.Id == id);

    public Task<bool> ExistsAsync(AggregateId id)
        => _resources.AnyAsync(r => r.Id == id);

    public async Task AddAsync(Resource resource)
    {
        await _resources.AddAsync(resource);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Resource resource)
    {
        _resources.Update(resource);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Resource resource)
    {
        _resources.Remove(resource);
        await _context.SaveChangesAsync();
    }
}