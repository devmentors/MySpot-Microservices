using Micro.Abstractions;
using Micro.Attributes;

namespace MySpot.Services.Reservations.Application.Commands;

[Message("reservations", "make_reservation", "reservations.make_reservation")]
public record MakeReservation(Guid UserId, Guid ParkingSpotId, int Capacity, string LicensePlate, DateTimeOffset Date,
    string? Note = null) : ICommand;