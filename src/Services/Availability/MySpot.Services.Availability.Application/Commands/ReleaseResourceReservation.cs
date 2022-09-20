using Micro.Abstractions;
using Micro.Attributes;

namespace MySpot.Services.Availability.Application.Commands;

[Message("availability", "release_resource_reservation", "availability.release_resource_reservation")]
public record ReleaseResourceReservation(Guid ResourceId, DateTimeOffset Date) : ICommand;