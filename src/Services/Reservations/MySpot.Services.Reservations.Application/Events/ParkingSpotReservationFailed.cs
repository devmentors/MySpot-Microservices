using Micro.Abstractions;
using Micro.Attributes;

namespace MySpot.Services.Reservations.Application.Events;

[Message("reservations", "parking_spot_reservation_failed")]
public record ParkingSpotReservationFailed(Guid ParkingSpotId, Guid UserId, DateTimeOffset Date, string Reason, string Code) : IEvent;