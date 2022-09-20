using System.Collections.Concurrent;
using Humanizer;
using Micro.Abstractions;
using Micro.Attributes;
using Micro.Contexts;
using Micro.Handlers;

namespace Micro.Transactions.Inbox;

[Decorator]
internal sealed class InboxCommandHandlerDecorator<T> : ICommandHandler<T> where T : class, ICommand
{
    private static readonly ConcurrentDictionary<Type, string> Names = new();
    private readonly ICommandHandler<T> _handler;
    private readonly IContextProvider _contextProvider;
    private readonly IInbox _inbox;

    public InboxCommandHandlerDecorator(ICommandHandler<T> handler, IContextProvider contextProvider, IInbox inbox)
    {
        _handler = handler;
        _contextProvider = contextProvider;
        _inbox = inbox;
    }

    public Task HandleAsync(T command, CancellationToken cancellationToken = default)
    {
        var context = _contextProvider.Current();
        var messageName = Names.GetOrAdd(typeof(T), typeof(T).Name.Underscore());
        if (_inbox.Enabled && !string.IsNullOrWhiteSpace(context.MessageId))
        {
            return _inbox.HandleAsync(context.MessageId, messageName,
                () => _handler.HandleAsync(command, cancellationToken), cancellationToken);
        }

        return _handler.HandleAsync(command, cancellationToken);
    }
}