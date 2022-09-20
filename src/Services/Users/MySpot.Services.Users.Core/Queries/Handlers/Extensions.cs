using MySpot.Services.Users.Core.DTO;
using MySpot.Services.Users.Core.Entities;

namespace MySpot.Services.Users.Core.Queries.Handlers;

internal static class Extensions
{
        
    public static UserDto AsDto(this User user)
        => user.Map<UserDto>();

    public static UserDetailsDto AsDetailsDto(this User user)
    {
        var dto = user.Map<UserDetailsDto>();
        dto.Permissions = user.Role.Permissions;

        return dto;
    }

    private static T Map<T>(this User user) where T : UserDto, new()
        => new()
        {
            UserId = user.Id,
            Email = user.Email,
            Role = user.Role.Name,
            JobTitle = user.JobTitle,
            CreatedAt = user.CreatedAt
        };
}