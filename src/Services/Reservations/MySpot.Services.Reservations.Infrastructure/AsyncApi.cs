using Micro.API.AsyncApi;
using MySpot.Services.Reservations.Application.Commands;
using MySpot.Services.Reservations.Application.Events;
using Saunter.Attributes;

namespace MySpot.Services.Reservations.Infrastructure;

internal abstract class AsyncApi : IAsyncApi
{
    [Channel(nameof(parking_spot_reserved), BindingsRef = "reservations")]
    [SubscribeOperation(typeof(ParkingSpotReserved), Summary = "Parking spot has been reserved", OperationId = nameof(parking_spot_reserved))]
    internal abstract void parking_spot_reserved();
    
    [Channel(nameof(parking_spot_reservation_failed), BindingsRef = "reservations")]
    [SubscribeOperation(typeof(ParkingSpotReservationFailed), Summary = "Parking spot reservation has failed", OperationId = nameof(parking_spot_reservation_failed))]
    internal abstract void parking_spot_reservation_failed();
        
    [Channel(nameof(parking_spot_reservation_removed), BindingsRef = "reservations")]
    [SubscribeOperation(typeof(ParkingSpotReservationRemoved), Summary = "Parking spot reservation has been removed", OperationId = nameof(parking_spot_reservation_removed))]
    internal abstract void parking_spot_reservation_removed();
    
    [Channel(nameof(make_reservation), BindingsRef = "reservations")]
    [PublishOperation(typeof(MakeReservation), Summary = "Make reservation", OperationId = nameof(make_reservation))]
    internal abstract void make_reservation();
    
    [Channel(nameof(remove_reservation), BindingsRef = "reservations")]
    [PublishOperation(typeof(RemoveReservation), Summary = "Remove reservation", OperationId = nameof(remove_reservation))]
    internal abstract void remove_reservation();
}