using Micro.Abstractions;
using Micro.Attributes;

namespace MySpot.Saga.Api.Messages;

[Message("reservations", "parking_spot_reservation_removed", "saga.parking_spot_reservation_removed")]
public record ParkingSpotReservationRemoved(Guid ReservationId, Guid ParkingSpotId, DateTimeOffset Date) : IEvent;