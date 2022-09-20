namespace MySpot.Services.Availability.Core.ValueObjects;

public record ReservationId
{
    public Guid Value { get; }

    public ReservationId(Guid value)
    {
        Value = value;
    }
    
    public static implicit operator ReservationId(Guid value) => new(value);
    public static implicit operator Guid(ReservationId id) => id.Value;
}