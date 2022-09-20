namespace MySpot.Services.Users.Core.DTO;

public class UserDetailsDto : UserDto
{
    public IEnumerable<string>? Permissions { get; set; }
}