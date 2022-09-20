namespace MySpot.Services.Users.Core.DTO;

public class UserDto
{
    public Guid UserId { get; set; }
    public string Email { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public string JobTitle { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}