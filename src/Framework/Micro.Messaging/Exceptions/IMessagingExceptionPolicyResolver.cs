using Micro.Abstractions;

namespace Micro.Messaging.Exceptions;

public interface IMessagingExceptionPolicyResolver
{
    MessageExceptionPolicy? Resolve(IMessage message, Exception exception);
}