using Micro.Exceptions;

namespace MySpot.Services.Availability.Core.Exceptions;

public class InvalidResourceTagsException : CustomException
{
    public InvalidResourceTagsException() : base("Resource tags are invalid.")
    {
    }
}