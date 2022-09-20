namespace MySpot.Services.Availability.Application.DTO;

public class ResourceDetailsDto : ResourceDto
{
    public List<ReservationDto> Reservations { get; set; } = new();
}