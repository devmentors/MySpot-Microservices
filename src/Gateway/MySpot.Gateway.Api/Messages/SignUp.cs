using Micro.Abstractions;
using Micro.Attributes;

namespace MySpot.Gateway.Api.Messages;

[Message("users", "sign_up")]
public record SignUp(string Email, string Password, string Role) : ICommand
{
    public Guid UserId { get; init; } = Guid.NewGuid();
}