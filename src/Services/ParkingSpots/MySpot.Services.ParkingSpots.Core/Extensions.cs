using Micro.DAL.Postgres;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MySpot.Services.ParkingSpots.Core.DAL;
using MySpot.Services.ParkingSpots.Core.Services;

namespace MySpot.Services.ParkingSpots.Core;

public static class Extensions
{
    public static IServiceCollection AddCore(this IServiceCollection services, IConfiguration configuration)
    {
        return services
            .AddScoped<IParkingSpotsService, ParkingSpotsService>()
            .AddPostgres<ParkingSpotsDbContext>(configuration)
            .AddInitializer<ParkingSpotsDataInitializer>();
    }
}