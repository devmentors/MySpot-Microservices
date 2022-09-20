using MySpot.Services.Reservations.Core.Types;
using MySpot.Services.Reservations.Core.ValueObjects;

namespace MySpot.Services.Reservations.Core.Entities;

public class User
{
    public UserId Id { get; private set; }
    public JobTitle JobTitle { get; private set; }

    public User(UserId id, JobTitle jobTitle)
    {
        Id = id;
        JobTitle = jobTitle;
    }
}