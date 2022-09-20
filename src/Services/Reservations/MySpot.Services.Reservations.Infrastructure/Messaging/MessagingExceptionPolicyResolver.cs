using Micro.Abstractions;
using Micro.Exceptions;
using Micro.Messaging.Exceptions;
using MySpot.Services.Reservations.Application.Commands;
using MySpot.Services.Reservations.Application.Events;

namespace MySpot.Services.Reservations.Infrastructure.Messaging;

internal sealed class MessagingExceptionPolicyResolver : IMessagingExceptionPolicyResolver
{
    public MessageExceptionPolicy? Resolve(IMessage message, Exception exception)
        => message switch
        {
            MakeReservation m => exception switch
            {
                CustomException ex => new MessageExceptionPolicy(false, true,
                    broker => broker.SendAsync(new ParkingSpotReservationFailed(m.ParkingSpotId, m.UserId, m.Date,
                        ex.Message, "cannot_make_reservation"))),
                _ => new MessageExceptionPolicy(false, true)
            },
            _ => null
        };
}