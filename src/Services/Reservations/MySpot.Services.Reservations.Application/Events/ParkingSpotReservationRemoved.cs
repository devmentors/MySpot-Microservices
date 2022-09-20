using Micro.Abstractions;
using Micro.Attributes;

namespace MySpot.Services.Reservations.Application.Events;

[Message("reservations", "parking_spot_reservation_removed")]
public record ParkingSpotReservationRemoved(Guid ReservationId, Guid ParkingSpotId, DateTimeOffset Date) : IEvent;