namespace MySpot.Services.Reservations.Application.Clients.DTO;

public record ResourceDto(Guid Id, int Capacity, List<string> Tags);