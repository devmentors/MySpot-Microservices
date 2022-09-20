namespace MySpot.Services.Notifications.Api.Entities;

public class User
{
    public Guid Id { get; private set; }
    public string Email { get; private set; }

    public User(Guid id, string email)
    {
        Id = id;
        Email = email;
    }
}