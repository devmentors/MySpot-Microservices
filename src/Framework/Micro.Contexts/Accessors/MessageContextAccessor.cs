namespace Micro.Contexts.Accessors;

public sealed class MessageContextAccessor : IMessageContextAccessor
{
    private static readonly AsyncLocal<MessageContextHolder> Holder = new();

    public MessageContext? MessageContext
    {
        get => Holder.Value?.Context;
        set
        {
            var holder = Holder.Value;
            if (holder != null)
            {
                holder.Context = null;
            }

            if (value is not null)
            {
                Holder.Value = new MessageContextHolder {Context = value};
            }
        }
    }

    private class MessageContextHolder
    {
        public MessageContext? Context;
    }
}