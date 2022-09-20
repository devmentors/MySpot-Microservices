using MySpot.Services.Availability.Core.Entities;

namespace MySpot.Services.Availability.Core.Events;

public class ResourceDeleted : IDomainEvent
{
    public Resource Resource { get; }

    public ResourceDeleted(Resource resource)
        => Resource = resource;
}