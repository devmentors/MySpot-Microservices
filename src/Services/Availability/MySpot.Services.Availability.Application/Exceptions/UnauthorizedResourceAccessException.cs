using Micro.Exceptions;

namespace MySpot.Services.Availability.Application.Exceptions;

public class UnauthorizedResourceAccessException : CustomException
{
    public Guid ResourceId { get; }
    public Guid CustomerId { get; }

    public UnauthorizedResourceAccessException(Guid resourceId, Guid customerId)
        : base($"Unauthorized access to resource: '{resourceId}' by customer: '{customerId}'")
        => (ResourceId, CustomerId) = (resourceId, customerId);
}