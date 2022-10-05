namespace MySpot.Services.Reservations.Infrastructure.DAL.ReadModels;

internal sealed class ReservationReadModel
{
    public Guid Id { get; set; }
    public Guid ParkingSpotId { get; set; }
    public int Capacity { get; set;}
    public DateTimeOffset Date { get; set; }
    public string State { get; set; }
    
    public WeeklyReservationsReadModel WeeklyReservations { get; set; }
}