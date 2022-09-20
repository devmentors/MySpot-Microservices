using Micro.Abstractions;
using Micro.Attributes;

namespace MySpot.Saga.Api.Messages;

[Message("reservations", "parking_spot_reserved", "saga.parking_spot_reserved")]
public record ParkingSpotReserved(Guid ReservationId, Guid ParkingSpotId, Guid UserId, DateTimeOffset Date, int Capacity) : IEvent;