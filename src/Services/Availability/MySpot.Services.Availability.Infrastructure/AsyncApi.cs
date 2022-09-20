using Micro.API.AsyncApi;
using MySpot.Services.Availability.Application.Commands;
using MySpot.Services.Availability.Application.Events;
using Saunter.Attributes;

namespace MySpot.Services.Availability.Infrastructure;

internal abstract class AsyncApi : IAsyncApi
{
    [Channel(nameof(resource_added), BindingsRef = "availability")]
    [SubscribeOperation(typeof(ResourceAdded), Summary = "Resource has been created", OperationId = nameof(resource_added))]
    internal abstract void resource_added();
    
    [Channel(nameof(resource_deleted), BindingsRef = "availability")]
    [SubscribeOperation(typeof(ResourceDeleted), Summary = "Resource has been deleted", OperationId = nameof(resource_deleted))]
    internal abstract void resource_deleted();
    
    [Channel(nameof(resource_reservation_canceled), BindingsRef = "availability")]
    [SubscribeOperation(typeof(ResourceReservationCanceled), Summary = "Resource reservation has been canceled", OperationId = nameof(resource_reservation_canceled))]
    internal abstract void resource_reservation_canceled();
    
    [Channel(nameof(resource_reservation_released), BindingsRef = "availability")]
    [SubscribeOperation(typeof(ResourceReservationReleased), Summary = "Resource reservation has been released", OperationId = nameof(resource_reservation_released))]
    internal abstract void resource_reservation_released();
    
    [Channel(nameof(resource_reserved), BindingsRef = "availability")]
    [SubscribeOperation(typeof(ResourceReserved), Summary = "Resource has been reserved", OperationId = nameof(resource_reserved))]
    internal abstract void resource_reserved();
    
    [Channel(nameof(resource_reservation_failed), BindingsRef = "availability")]
    [SubscribeOperation(typeof(ResourceReservationFailed), Summary = "Resource reservation has failed", OperationId = nameof(resource_reservation_failed))]
    internal abstract void resource_reservation_failed();
    
    [Channel(nameof(add_resource), BindingsRef = "availability")]
    [PublishOperation(typeof(AddResource), Summary = "Add resource", OperationId = nameof(add_resource))]
    internal abstract void add_resource();
    
    [Channel(nameof(delete_resource), BindingsRef = "availability")]
    [PublishOperation(typeof(DeleteResource), Summary = "Delete resource", OperationId = nameof(delete_resource))]
    internal abstract void delete_resource();
    
    [Channel(nameof(release_resource_reservation), BindingsRef = "availability")]
    [PublishOperation(typeof(ReleaseResourceReservation), Summary = "Release resource reservation", OperationId = nameof(release_resource_reservation))]
    internal abstract void release_resource_reservation();
    
    [Channel(nameof(reserve_resource), BindingsRef = "availability")]
    [PublishOperation(typeof(ReserveResource), Summary = "Reserve resource", OperationId = nameof(reserve_resource))]
    internal abstract void reserve_resource();
}