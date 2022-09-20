using Micro.Abstractions;

namespace Micro.Messaging.Subscribers;

public interface IMessageSubscriber
{
    IMessageSubscriber Message<T>(Func<IServiceProvider, T, CancellationToken, Task> handler) where T : class, IMessage;
    IMessageSubscriber Command<T>() where T : class, ICommand;
    IMessageSubscriber Event<T>() where T : class, IEvent;
}