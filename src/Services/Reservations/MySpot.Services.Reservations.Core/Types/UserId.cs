using MySpot.Services.Reservations.Core.Exception;

namespace MySpot.Services.Reservations.Core.Types;

public class UserId
{
    public Guid Value { get; }

    public UserId(Guid value)
    {
        if (value == Guid.Empty)
        {
            throw new InvalidEntityIdException(value);
        }

        Value = value;
    }
    
    public static implicit operator Guid(UserId userId)
        => userId.Value;
    
    public static implicit operator UserId(Guid value)
        => new(value);
   
    public static implicit operator string(UserId userId)
        => userId.Value.ToString();

    public static implicit operator UserId?(string? value)
        => string.IsNullOrWhiteSpace(value) ? null : Guid.Parse(value);
}