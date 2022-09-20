using Micro.Abstractions;

namespace Micro.Messaging.Exceptions;

public record FailedMessage(IMessage Message);