using Micro.DAL.Postgres;
using Micro.Messaging.Exceptions;
using Micro.Messaging.RabbitMQ;
using Micro.Transactions;
using Micro.Transactions.Inbox;
using Micro.Transactions.Outbox;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MySpot.Services.Availability.Core.Repositories;
using MySpot.Services.Availability.Infrastructure.DAL;
using MySpot.Services.Availability.Infrastructure.DAL.Repositories;
using MySpot.Services.Availability.Infrastructure.Messaging;

namespace MySpot.Services.Availability.Infrastructure;

public static class Extensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        => services
            .AddTransient<IResourcesRepository, ResourcesRepository>()
            .AddPostgres<AvailabilityDbContext>(configuration)
            .AddSingleton<IMessagingExceptionPolicyResolver, MessagingExceptionPolicyResolver>()
            .AddOutbox<AvailabilityDbContext>(configuration)
            .AddInbox<AvailabilityDbContext>(configuration)
            .AddMessagingErrorHandlingDecorators()
            .AddTransactionalDecorators()
            .AddOutboxInstantSenderDecorators();
}