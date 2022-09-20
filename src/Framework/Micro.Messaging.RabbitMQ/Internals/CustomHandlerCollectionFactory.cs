using System.Collections.Concurrent;
using EasyNetQ.Consumer;
using EasyNetQ.Topology;

namespace Micro.Messaging.RabbitMQ.Internals;

internal sealed class CustomHandlerCollectionFactory : IHandlerCollectionFactory
{
    private readonly ConcurrentDictionary<string, IHandlerCollection> _handlerCollections = new();

    public IHandlerCollection CreateHandlerCollection(in Queue queue)
    {
        if (_handlerCollections.TryGetValue(queue.Name, out var handlerCollection))
        {
            return handlerCollection;
        }

        handlerCollection = new HandlerCollection();
        _handlerCollections.TryAdd(queue.Name, handlerCollection);

        return handlerCollection;
    }
}