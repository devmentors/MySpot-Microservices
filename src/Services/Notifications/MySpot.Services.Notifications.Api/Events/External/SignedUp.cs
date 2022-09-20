using Micro.Abstractions;
using Micro.Attributes;

namespace MySpot.Services.Notifications.Api.Events.External;

[Message("users", "signed_up", "notifications.signed_up")]
public record SignedUp(Guid UserId, string Email) : IEvent;