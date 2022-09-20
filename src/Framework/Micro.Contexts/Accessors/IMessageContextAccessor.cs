namespace Micro.Contexts.Accessors;

public interface IMessageContextAccessor
{
    MessageContext? MessageContext { get; set; }
}