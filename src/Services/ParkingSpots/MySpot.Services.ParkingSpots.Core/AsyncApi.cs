using Micro.API.AsyncApi;
using MySpot.Services.ParkingSpots.Core.Events;
using Saunter.Attributes;

namespace MySpot.Services.ParkingSpots.Core;

internal abstract class AsyncApi : IAsyncApi
{
    [Channel(nameof(parking_spot_created), BindingsRef = "parking-spots")]
    [SubscribeOperation(typeof(ParkingSpotCreated), Summary = "Parking spot has been created", OperationId = nameof(parking_spot_created))]
    internal abstract void parking_spot_created();
    
    [Channel(nameof(parking_spot_deleted), BindingsRef = "parking-spots")]
    [SubscribeOperation(typeof(ParkingSpotDeleted), Summary = "Parking spot has been deleted", OperationId = nameof(parking_spot_deleted))]
    internal abstract void parking_spot_deleted();
}