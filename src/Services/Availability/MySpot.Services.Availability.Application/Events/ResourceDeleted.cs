using Micro.Abstractions;
using Micro.Attributes;

namespace MySpot.Services.Availability.Application.Events;

[Message("availability", "resource_deleted")]
public record ResourceDeleted(Guid ResourceId) : IEvent;