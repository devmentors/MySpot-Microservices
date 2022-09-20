namespace MySpot.Services.Availability.Application.DTO;

public class ReservationDto
{
    public DateTimeOffset Date { get; set; }
    public int Capacity { get; set; }
    public int Priority { get; set; }
}