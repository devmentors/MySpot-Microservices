using Microsoft.Extensions.DependencyInjection;
using MySpot.Services.Reservations.Core.DomainServices;
using MySpot.Services.Reservations.Core.Policies;

namespace MySpot.Services.Reservations.Application;

public static class Extensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        var assemblies = typeof(IReservationPolicy).Assembly;
        services.AddSingleton<IWeeklyReservationsService, WeeklyReservationsService>();

        services.Scan(s => s.FromAssemblies(assemblies)
            .AddClasses(c => c.AssignableTo<IReservationPolicy>())
            .AsImplementedInterfaces()
            .WithSingletonLifetime());
        
        return services;
    }
}