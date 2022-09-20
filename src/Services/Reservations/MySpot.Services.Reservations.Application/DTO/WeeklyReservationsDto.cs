namespace MySpot.Services.Reservations.Application.DTO;

public class WeeklyReservationsDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public DateTime From { get; set; }
    public DateTime To { get; set; }
    public List<ReservationDto> Reservations { get; set; } = new();
}