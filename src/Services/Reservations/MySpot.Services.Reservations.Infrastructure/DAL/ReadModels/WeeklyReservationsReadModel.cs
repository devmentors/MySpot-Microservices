namespace MySpot.Services.Reservations.Infrastructure.DAL.ReadModels;

internal sealed class WeeklyReservationsReadModel
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public DateTimeOffset To { get; set; }
    public ICollection<ReservationReadModel> Reservations { get; set; }
}