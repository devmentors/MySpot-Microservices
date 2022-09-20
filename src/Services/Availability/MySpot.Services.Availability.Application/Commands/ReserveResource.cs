using Micro.Abstractions;
using Micro.Attributes;

namespace MySpot.Services.Availability.Application.Commands;

[Message("availability", "reserve_resource", "availability.reserve_resource")]
public record ReserveResource(Guid ResourceId, Guid ReservationId, int Capacity, DateTimeOffset Date, int Priority) : ICommand;