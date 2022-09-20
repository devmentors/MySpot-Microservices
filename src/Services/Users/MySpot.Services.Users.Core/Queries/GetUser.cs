using Micro.Abstractions;
using MySpot.Services.Users.Core.DTO;

namespace MySpot.Services.Users.Core.Queries;

public class GetUser : IQuery<UserDetailsDto?>
{
    public Guid UserId { get; set; }
}