namespace MySpot.Services.Availability.Application.DTO;

public class ResourceDto
{
    public Guid Id { get; set; }
    public int Capacity { get; set; }
    public List<string> Tags { get; set; } = new();
}