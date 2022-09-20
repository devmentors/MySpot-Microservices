using Micro.Abstractions;

namespace Micro.Messaging.Brokers;

public interface IMessageBroker
{
    Task SendAsync<T>(T message, CancellationToken cancellationToken = default) where T : IMessage;
}