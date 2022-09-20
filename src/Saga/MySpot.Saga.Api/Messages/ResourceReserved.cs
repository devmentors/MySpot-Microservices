using Micro.Abstractions;
using Micro.Attributes;

namespace MySpot.Saga.Api.Messages;

[Message("availability", "resource_reserved", "saga.resource_reserved")]
public record ResourceReserved(Guid ResourceId, DateTimeOffset Date) : IEvent;