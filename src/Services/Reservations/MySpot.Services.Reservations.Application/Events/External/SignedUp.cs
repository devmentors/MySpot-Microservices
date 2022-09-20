using Micro.Abstractions;
using Micro.Attributes;

namespace MySpot.Services.Reservations.Application.Events.External;

[Message("users", "signed_up", "reservations.signed_up")]
public record SignedUp(Guid UserId, string JobTitle) : IEvent;