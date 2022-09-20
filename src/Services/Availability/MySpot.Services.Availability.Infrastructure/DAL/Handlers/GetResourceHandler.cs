using Micro.Handlers;
using Microsoft.EntityFrameworkCore;
using MySpot.Services.Availability.Application.DTO;
using MySpot.Services.Availability.Application.Queries;
using MySpot.Services.Availability.Core.Entities;

namespace MySpot.Services.Availability.Infrastructure.DAL.Handlers;

internal sealed class GetResourceHandler : IQueryHandler<GetResource, ResourceDetailsDto?>
{
    private readonly DbSet<Resource> _resources;

    public GetResourceHandler(AvailabilityDbContext context)
    {
        _resources = context.Resources;
    }

    public Task<ResourceDetailsDto?> HandleAsync(GetResource query, CancellationToken cancellationToken = default)
    {
        return _resources
            .AsNoTracking()
            .Where(x => x.Id.Equals(query.ResourceId))
            .Include(x => x.Reservations)
            .Select(x => x.AsDetailsDto())
            .SingleOrDefaultAsync(cancellationToken);
    }
}