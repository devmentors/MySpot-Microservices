using Micro.Exceptions;

namespace MySpot.Services.Users.Core.Exceptions;

internal class MissingPasswordException : CustomException
{
    public MissingPasswordException() : base($"Invalid password")
    {
    }
}