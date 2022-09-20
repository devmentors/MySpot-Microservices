using Micro.API.AsyncApi;
using MySpot.Services.Users.Core.Commands;
using MySpot.Services.Users.Core.Events;
using Saunter.Attributes;

namespace MySpot.Services.Users.Core;

internal abstract class AsyncApi : IAsyncApi
{
    [Channel(nameof(signed_up), BindingsRef = "users")]
    [SubscribeOperation(typeof(SignedUp), Summary = "User has been created", OperationId = nameof(signed_up))]
    internal abstract void signed_up();
    
    [Channel(nameof(signed_in), BindingsRef = "users")]
    [SubscribeOperation(typeof(SignedIn), Summary = "User has been authenticated", OperationId = nameof(signed_in))]
    internal abstract void signed_in();
    
    [Channel(nameof(sign_up), BindingsRef = "users")]
    [PublishOperation(typeof(SignUp), Summary = "Create user account", OperationId = nameof(sign_up))]
    internal abstract void sign_up();
}