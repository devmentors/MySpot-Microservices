using Micro.Abstractions;
using Micro.Attributes;

namespace MySpot.Services.Users.Core.Events;

[Message("users", "signed_in")]
public record SignedIn(Guid UserId) : IEvent;