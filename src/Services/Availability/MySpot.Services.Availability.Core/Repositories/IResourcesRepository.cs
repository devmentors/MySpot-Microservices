using MySpot.Services.Availability.Core.Entities;

namespace MySpot.Services.Availability.Core.Repositories;

public interface IResourcesRepository
{
    Task<Resource?> GetAsync(AggregateId id);
    Task<bool> ExistsAsync(AggregateId id);
    Task AddAsync(Resource resource);
    Task UpdateAsync(Resource resource);
    Task DeleteAsync(Resource resource);
}