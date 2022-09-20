namespace MySpot.Services.Users.Core.Entities;

public class User
{
    public Guid Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string JobTitle { get; set; } = string.Empty;
    public Role Role { get; set; } = new();
    public string RoleId { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}