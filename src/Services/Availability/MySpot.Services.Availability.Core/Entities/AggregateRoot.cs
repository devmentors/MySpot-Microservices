using MySpot.Services.Availability.Core.Events;

namespace MySpot.Services.Availability.Core.Entities;

public abstract class AggregateRoot
{
    private bool _versionIncremented;
    private readonly List<IDomainEvent> _events = new();
    public IEnumerable<IDomainEvent> Events => _events;
    public AggregateId Id { get; protected set; } = null!;
    public int Version { get; protected set; }

    protected void AddEvent(IDomainEvent @event)
    {
        if (!_events.Any())
        {
            IncrementVersion();
        }

        _events.Add(@event);
    }

    public void ClearEvents() => _events.Clear();
    
    protected void IncrementVersion()
    {
        if (_versionIncremented)
        {
            return;
        }

        Version++;
        _versionIncremented = true;
    }
}