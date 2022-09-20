using Micro.Exceptions;

namespace MySpot.Services.Availability.Application.Exceptions;

public class ResourceAlreadyExistsException : CustomException
{
    public Guid Id { get; }

    public ResourceAlreadyExistsException(Guid id) : base($"Resource with id: {id} already exists.")
        => Id = id;
}