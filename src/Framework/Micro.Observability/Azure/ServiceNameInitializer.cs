using Microsoft.ApplicationInsights.Channel;
using Microsoft.ApplicationInsights.Extensibility;

namespace Micro.Observability.Azure;

internal sealed class ServiceNameInitializer : ITelemetryInitializer
{
    private readonly string _name;

    public ServiceNameInitializer(string name)
    {
        _name = name;
    }
    
    public void Initialize(ITelemetry telemetry)
    {
        telemetry.Context.Cloud.RoleInstance = _name;
        telemetry.Context.Cloud.RoleName = _name;
    }
}