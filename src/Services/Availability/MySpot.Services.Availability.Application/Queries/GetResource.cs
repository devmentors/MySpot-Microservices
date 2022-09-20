using Micro.Abstractions;
using MySpot.Services.Availability.Application.DTO;

namespace MySpot.Services.Availability.Application.Queries;

public class GetResource : IQuery<ResourceDetailsDto?>
{
    public Guid ResourceId { get; set; }
}