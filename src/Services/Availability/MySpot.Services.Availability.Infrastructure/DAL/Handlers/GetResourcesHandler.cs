using Micro.Handlers;
using Microsoft.EntityFrameworkCore;
using MySpot.Services.Availability.Application.DTO;
using MySpot.Services.Availability.Application.Queries;
using MySpot.Services.Availability.Core.Entities;

namespace MySpot.Services.Availability.Infrastructure.DAL.Handlers;

internal sealed class GetResourcesHandler : IQueryHandler<GetResources, IEnumerable<ResourceDto>>
{
    private readonly DbSet<Resource> _resources;

    public GetResourcesHandler(AvailabilityDbContext context)
    {
        _resources = context.Resources;
    }

    public async Task<IEnumerable<ResourceDto>> HandleAsync(GetResources query,
        CancellationToken cancellationToken = default)
    {
        return await _resources
            .AsNoTracking()
            .Select(x => x.AsDto())
            .ToListAsync(cancellationToken);
    }
}