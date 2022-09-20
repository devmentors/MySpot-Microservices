namespace MySpot.Services.ParkingSpots.Core.Clients.DTO;

internal record AddResourceDto(Guid ResourceId, int Capacity, IEnumerable<string> Tags);