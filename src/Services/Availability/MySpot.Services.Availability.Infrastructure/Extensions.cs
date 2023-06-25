using Micro.DAL.Postgres;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MySpot.Services.Availability.Core.Repositories;
using MySpot.Services.Availability.Infrastructure.DAL;
using MySpot.Services.Availability.Infrastructure.DAL.Repositories;

namespace MySpot.Services.Availability.Infrastructure;

public static class Extensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        => services
            .AddTransient<IResourcesRepository, ResourcesRepository>()
            .AddPostgres<AvailabilityDbContext>(configuration);
}