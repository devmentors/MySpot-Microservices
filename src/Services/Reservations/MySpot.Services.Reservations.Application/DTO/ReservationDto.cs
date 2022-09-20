namespace MySpot.Services.Reservations.Application.DTO;

public class ReservationDto
{
    public Guid Id { get; set; }
    public Guid ParkingSpotId { get; set; }
    public int Capacity { get; set;}
    public string LicensePlate { get; set; } = string.Empty;
    public DateTime Date { get; set; } 
    public string State { get; set; } = string.Empty;
    public string? Note { get; set; }
}