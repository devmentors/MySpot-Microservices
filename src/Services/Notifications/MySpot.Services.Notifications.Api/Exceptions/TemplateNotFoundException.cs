using Micro.Exceptions;

namespace MySpot.Services.Notifications.Api.Exceptions;

internal sealed class TemplateNotFoundException : CustomException
{
    public string Name { get; }

    public TemplateNotFoundException(string name) : base($"Template: '{name}' was not found.")
    {
        Name = name;
    }
}