using MySpot.Services.Availability.Application.DTO;
using MySpot.Services.Availability.Core.Entities;

namespace MySpot.Services.Availability.Infrastructure.DAL.Handlers;

internal static class Extensions
{
    public static ResourceDto AsDto(this Resource entity)
        => entity.Map<ResourceDto>();

    public static ResourceDetailsDto AsDetailsDto(this Resource entity)
    {
        var dto = entity.Map<ResourceDetailsDto>();
        dto.Reservations = entity.Reservations.Select(x => new ReservationDto
        {
            Date = x.Date,
            Priority = x.Priority,
            Capacity = x.Capacity
        }).ToList();

        return dto;
    }

    private static T Map<T>(this Resource entity) where T: ResourceDto, new() => new()
        {
            Id = entity.Id,
            Capacity = entity.Capacity,
            Tags = entity.Tags.Select(x =>x.Value).ToList()
        };
}