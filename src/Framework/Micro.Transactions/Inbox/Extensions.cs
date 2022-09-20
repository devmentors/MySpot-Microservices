using Micro.Handlers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Micro.Transactions.Inbox;

public static class Extensions
{
    public static IServiceCollection AddInbox<T>(this IServiceCollection services, IConfiguration configuration)
        where T : DbContext
    {
        var section = configuration.GetSection("inbox");
        var inboxOptions = section.BindOptions<InboxOptions>();
        services.Configure<InboxOptions>(section);
        services.AddScoped<IInbox, EfInbox<T>>();
        
        if (!inboxOptions.Enabled)
        {
            return services;
        }
        
        services.AddHostedService<InboxCleaner>();
        services.TryDecorate(typeof(ICommandHandler<>), typeof(InboxCommandHandlerDecorator<>));
        services.TryDecorate(typeof(IEventHandler<>), typeof(InboxEventHandlerDecorator<>));

        return services;
    }
}