using Micro.Exceptions;

namespace MySpot.Services.Availability.Core.Exceptions;

public class MissingResourceTagsException : CustomException
{
    public MissingResourceTagsException() : base("Resource tags are missing.")
    {
    }
}