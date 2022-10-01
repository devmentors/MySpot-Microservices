using Micro.API.AsyncApi;
using MySpot.Services.Notifications.Api.Commands;
using Saunter.Attributes;

namespace MySpot.Services.Notifications.Api;

internal abstract class AsyncApi : IAsyncApi
{
    [Channel(nameof(send_email), BindingsRef = "notifications")]
    [SubscribeOperation(typeof(SendEmail), Summary = "Send an email", OperationId = nameof(send_email))]
    internal abstract void send_email();
}