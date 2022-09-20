using System.Diagnostics.Metrics;

namespace Micro.Observability.Metrics;

public class ObservableValue<T> where T : struct
{
    private readonly ObservableGauge<T> _metric;

    private T _value;

    public ObservableValue(IMetrics metrics, string name)
    {
        _metric = metrics.ObservableGauge(name, () => new Measurement<T>(_value));
    }

    public void Update(T value) => _value = value;
}