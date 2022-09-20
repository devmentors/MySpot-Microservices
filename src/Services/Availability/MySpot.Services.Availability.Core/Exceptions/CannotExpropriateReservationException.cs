using Micro.Exceptions;
using MySpot.Services.Availability.Core.ValueObjects;

namespace MySpot.Services.Availability.Core.Exceptions;

public class CannotExpropriateReservationException : CustomException
{
    public Guid ResourceId { get; }
    public Date Date { get; }

    public CannotExpropriateReservationException(Guid resourceId, Date dateTime)
        : base($"Cannot expropriate resource {resourceId} reservation at {dateTime}")
        => (ResourceId, Date) = (resourceId, dateTime);
}