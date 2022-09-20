using Micro.Abstractions;
using Micro.Attributes;
using Micro.Handlers;
using Micro.Transactions.Outbox;

namespace Micro.Transactions.Decorators;

[Decorator]
internal sealed class OutboxInstantSenderCommandHandlerDecorator<T> : ICommandHandler<T> where T : class, ICommand
{
    private readonly ICommandHandler<T> _handler;
    private readonly IOutbox _outbox;

    public OutboxInstantSenderCommandHandlerDecorator(ICommandHandler<T> handler, IOutbox outbox)
    {
        _handler = handler;
        _outbox = outbox;
    }

    public async Task HandleAsync(T command, CancellationToken cancellationToken = default)
    {
        await _handler.HandleAsync(command, cancellationToken);
        await _outbox.PublishAwaitingAsync(cancellationToken);
    }
}