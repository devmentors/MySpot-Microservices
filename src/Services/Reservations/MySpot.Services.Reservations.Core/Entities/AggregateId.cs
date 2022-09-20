using MySpot.Services.Reservations.Core.Exception;

namespace MySpot.Services.Reservations.Core.Entities;

public record AggregateId
{
    public Guid Value { get; }

    public AggregateId() : this(Guid.NewGuid())
    {
    }

    public AggregateId(Guid value)
    {
        if (value == Guid.Empty)
        {
            throw new InvalidAggregateIdException(value);
        }

        Value = value;
    }

    public static AggregateId Create()
        => new(Guid.NewGuid());
    
    public static implicit operator Guid(AggregateId id)
        => id.Value;

    public static implicit operator AggregateId(Guid id)
        => new (id);

    public override string ToString() => Value.ToString();
}