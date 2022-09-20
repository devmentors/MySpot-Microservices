using Micro.Abstractions;
using MySpot.Services.Availability.Application.DTO;

namespace MySpot.Services.Availability.Application.Queries;

public class GetResources : IQuery<IEnumerable<ResourceDto>>
{
}