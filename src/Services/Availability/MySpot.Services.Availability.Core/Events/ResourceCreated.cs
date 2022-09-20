using MySpot.Services.Availability.Core.Entities;

namespace MySpot.Services.Availability.Core.Events;

public class ResourceCreated : IDomainEvent
{
    public Resource Resource { get; }

    public ResourceCreated(Resource resource)
        => Resource = resource;
}