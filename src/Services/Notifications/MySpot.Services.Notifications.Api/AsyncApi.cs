using Micro.API.AsyncApi;
using MySpot.Services.Notifications.Api.Commands;
using MySpot.Services.Notifications.Api.Events.External;
using Saunter.Attributes;

namespace MySpot.Services.Notifications.Api;

internal abstract class AsyncApi : IAsyncApi
{
    [Channel(nameof(send_email), BindingsRef = "notifications")]
    [SubscribeOperation(typeof(SendEmail), Summary = "Send an email", OperationId = nameof(send_email))]
    internal abstract void send_email();
    
    [Channel(nameof(signed_up), BindingsRef = "users")]
    [PublishOperation(typeof(SignedUp), Summary = "User has been created", OperationId = nameof(signed_up))]
    internal abstract void signed_up();
}