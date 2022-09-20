using Micro.Abstractions;
using Micro.Attributes;

namespace MySpot.Services.Notifications.Api.Commands;

[Message("notifications", "send_email", "notifications.send_email")]
public record SendEmail(Guid UserId, string Template) : ICommand;