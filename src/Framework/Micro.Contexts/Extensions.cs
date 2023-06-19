using Micro.Contexts.Accessors;
using Micro.Contexts.Providers;
using Microsoft.Extensions.DependencyInjection;

namespace Micro.Contexts;

public static class Extensions
{
    public static IServiceCollection AddContexts(this IServiceCollection services)
    {
        services.AddSingleton<IContextProvider, ContextProvider>();
        services.AddSingleton<IContextAccessor, ContextAccessor>();

        return services;
    }
}