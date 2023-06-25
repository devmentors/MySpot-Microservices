using Micro.DAL.Postgres;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MySpot.Services.Reservations.Core.Repository;
using MySpot.Services.Reservations.Infrastructure.DAL;
using MySpot.Services.Reservations.Infrastructure.DAL.Repositories;

namespace MySpot.Services.Reservations.Infrastructure;

public static class Extensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        => services
            .AddScoped<IUserRepository, UserRepository>()
            .AddScoped<IWeeklyReservationsRepository, WeeklyReservationsRepository>()
            .AddPostgres<ReservationsDbContext>(configuration);
}