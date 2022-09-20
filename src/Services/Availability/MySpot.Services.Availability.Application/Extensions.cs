using Microsoft.Extensions.DependencyInjection;

namespace MySpot.Services.Availability.Application;

public static class Extensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        return services;
    }
}