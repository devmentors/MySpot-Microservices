using Micro.Exceptions;

namespace MySpot.Services.Availability.Core.Exceptions;

public sealed class PastDateException : CustomException
{
    public DateTimeOffset Date { get; }

    public PastDateException(DateTimeOffset date) : base($"Cannot set past date : {date.Date}")
        => Date = date;
}