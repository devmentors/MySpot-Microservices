using Micro.API.AsyncApi;
using MySpot.Services.Mapping.Api.ParkingSpotAvailabilityMappings.Commands;
using MySpot.Services.Mapping.Api.ParkingSpotAvailabilityMappings.Events;
using Saunter.Attributes;

namespace MySpot.Services.Mapping.Api;

internal abstract class AsyncApi : IAsyncApi
{
    [Channel(nameof(add_resource), BindingsRef = "availability")]
    [SubscribeOperation(typeof(AddResource), Summary = "Add resource", OperationId = nameof(add_resource))]
    internal abstract void add_resource();
    
    [Channel(nameof(delete_resource), BindingsRef = "availability")]
    [SubscribeOperation(typeof(DeleteResource), Summary = "Delete resource", OperationId = nameof(delete_resource))]
    internal abstract void delete_resource();
    
    [Channel(nameof(parking_spot_created), BindingsRef = "parking-spots")]
    [PublishOperation(typeof(ParkingSpotCreated), Summary = "Parking spot has been created", OperationId = nameof(parking_spot_created))]
    internal abstract void parking_spot_created();
    
    [Channel(nameof(parking_spot_deleted), BindingsRef = "parking-spots")]
    [PublishOperation(typeof(ParkingSpotDeleted), Summary = "Parking spot has been deleted", OperationId = nameof(parking_spot_deleted))]
    internal abstract void parking_spot_deleted();
}