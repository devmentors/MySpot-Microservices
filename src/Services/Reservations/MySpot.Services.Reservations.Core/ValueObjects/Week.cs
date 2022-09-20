namespace MySpot.Services.Reservations.Core.ValueObjects;

public sealed record Week
{
    public Date From { get; }
    public Date To { get; }

    public Week(DateTimeOffset value)
    {
        var dayOfWeekNumber = value.DayOfWeek is DayOfWeek.Sunday ? 7 : (int) value.DayOfWeek;
        var pastDays = -1 * dayOfWeekNumber;
        var remainingDays = 7 + pastDays;
        From = new Date(value.AddDays(pastDays));
        To = new Date(value.AddDays(remainingDays));
    }

    public override string ToString() => $"{From} -> {To}";
}