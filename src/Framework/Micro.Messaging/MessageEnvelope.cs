using Micro.Abstractions;
using Micro.Contexts;

namespace Micro.Messaging;

public record MessageEnvelope<T>(T Message, MessageContext Context) where T : IMessage;