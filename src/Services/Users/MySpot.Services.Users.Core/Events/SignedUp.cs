using Micro.Abstractions;
using Micro.Attributes;

namespace MySpot.Services.Users.Core.Events;

[Message("users", "signed_up")]
public record SignedUp(Guid UserId, string Email, string Role, string JobTitle) : IEvent;