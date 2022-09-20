using Micro.Exceptions;

namespace MySpot.Services.Users.Core.Exceptions;

internal class RoleNotFoundException : CustomException
{
    public RoleNotFoundException(string role) : base($"Role: '{role}' was not found.")
    {
    }
}