using Micro.Abstractions;
using Micro.Messaging.Exceptions;
using MySpot.Services.Availability.Application.Commands;
using MySpot.Services.Availability.Application.Events;
using MySpot.Services.Availability.Application.Exceptions;
using MySpot.Services.Availability.Core.Exceptions;

namespace MySpot.Services.Availability.Infrastructure.Messaging;

internal sealed class MessagingExceptionPolicyResolver : IMessagingExceptionPolicyResolver
{
    public MessageExceptionPolicy? Resolve(IMessage message, Exception exception)
        => message switch
        {
            ReserveResource m => exception switch
            {
                ResourceNotFoundException ex => new MessageExceptionPolicy(false, true,
                    broker => broker.SendAsync(new ResourceReservationFailed(m.ResourceId, m.Date,
                        ex.Message, "resource_not_found"))),
                CannotExpropriateReservationException ex => new MessageExceptionPolicy(false, true,
                    broker => broker.SendAsync(new ResourceReservationFailed(m.ResourceId, m.Date,
                        ex.Message, "cannot_expropriate_reservation"))),
                ResourceCapacityExceededException ex => new MessageExceptionPolicy(false, true,
                    broker => broker.SendAsync(new ResourceReservationFailed(m.ResourceId, m.Date,
                        ex.Message, "resource_capacity_exceeded"))),
                _ => new MessageExceptionPolicy(false, true)
            },
            _ => null
        };
}