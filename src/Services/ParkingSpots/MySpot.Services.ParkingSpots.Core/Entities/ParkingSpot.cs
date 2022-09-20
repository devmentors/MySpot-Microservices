using System.ComponentModel.DataAnnotations;

namespace MySpot.Services.ParkingSpots.Core.Entities;

public class ParkingSpot
{
    public Guid Id { get; set; }

    [Required]
    [StringLength(50, MinimumLength = 1)]
    public string Name { get; set; } = string.Empty;

    [Range(1,10000)]
    public int DisplayOrder { get; set; }
}