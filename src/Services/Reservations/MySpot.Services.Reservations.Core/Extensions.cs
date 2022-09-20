using Microsoft.Extensions.DependencyInjection;
using MySpot.Services.Reservations.Core.Factories;

namespace MySpot.Services.Reservations.Core;

public static class Extensions
{
    public static IServiceCollection AddCore(this IServiceCollection services)
        => services.AddTransient<IWeeklyReservationsFactory, WeeklyReservationsFactory>();
}