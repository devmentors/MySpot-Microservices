using Micro.API.AsyncApi;
using MySpot.Saga.Api.Messages;
using Saunter.Attributes;

namespace MySpot.Saga.Api;

internal abstract class AsyncApi : IAsyncApi
{
    [Channel(nameof(reserve_resource), BindingsRef = "availability")]
    [SubscribeOperation(typeof(ReserveResource), Summary = "Reserve resource", OperationId = nameof(reserve_resource))]
    internal abstract void reserve_resource();
    
    [Channel(nameof(remove_reservation), BindingsRef = "reservations")]
    [SubscribeOperation(typeof(RemoveReservation), Summary = "Remove reservation", OperationId = nameof(remove_reservation))]
    internal abstract void remove_reservation();
    
    [Channel(nameof(resource_reserved), BindingsRef = "availability")]
    [PublishOperation(typeof(ResourceReserved), Summary = "Resource has been reserved", OperationId = nameof(resource_reserved))]
    internal abstract void resource_reserved();
    
    [Channel(nameof(resource_reservation_failed), BindingsRef = "availability")]
    [SubscribeOperation(typeof(ResourceReservationFailed), Summary = "Resource reservation has failed", OperationId = nameof(resource_reservation_failed))]
    internal abstract void resource_reservation_failed();
    
    [Channel(nameof(parking_spot_reserved), BindingsRef = "reservations")]
    [PublishOperation(typeof(ParkingSpotReserved), Summary = "Parking spot has been reserved", OperationId = nameof(parking_spot_reserved))]
    internal abstract void parking_spot_reserved();
    
    [Channel(nameof(parking_spot_reservation_removed), BindingsRef = "reservations")]
    [PublishOperation(typeof(ParkingSpotReservationRemoved), Summary = "Parking spot reservation has been removed", OperationId = nameof(parking_spot_reservation_removed))]
    internal abstract void parking_spot_reservation_removed();
}