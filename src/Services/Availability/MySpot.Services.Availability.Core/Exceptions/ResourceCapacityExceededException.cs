using Micro.Exceptions;
using MySpot.Services.Availability.Core.ValueObjects;

namespace MySpot.Services.Availability.Core.Exceptions;

public class ResourceCapacityExceededException : CustomException
{
    private readonly Capacity _capacity;
    public Guid ResourceId { get; }

    public ResourceCapacityExceededException(Guid resourceId, Capacity capacity) 
        : base($"Resource with ID: {resourceId} exceeded capacity of {capacity}")
    {
        _capacity = capacity;
        ResourceId = resourceId;
    }
}