using Micro.Messaging.Brokers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Micro.Transactions.Outbox;

public static class Extensions
{
    public static IServiceCollection AddOutbox<T>(this IServiceCollection services, IConfiguration configuration)
        where T : DbContext
    {
        var section = configuration.GetSection("outbox");
        var outboxOptions = section.BindOptions<OutboxOptions>();
        services.Configure<OutboxOptions>(section);
        services.AddScoped<IOutbox, EfOutbox<T>>();
        
        if (!outboxOptions.Enabled)
        {
            return services;
        }
        
        services.AddTransient<IMessageBroker, OutboxMessageBroker>();
        services.AddHostedService<OutboxSender>();
        services.AddHostedService<OutboxCleaner>();

        return services;
    }
}