using Micro.Exceptions;

namespace MySpot.Services.Availability.Application.Exceptions;

public class ResourceNotFoundException : CustomException
{
    public Guid Id { get; }

    public ResourceNotFoundException(Guid id) : base($"Resource with ID: {id} was not found.")
        => Id = id;
}