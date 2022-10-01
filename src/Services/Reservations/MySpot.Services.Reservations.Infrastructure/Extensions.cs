using Micro.DAL.Postgres;
using Micro.Messaging.Exceptions;
using Micro.Messaging.RabbitMQ;
using Micro.Transactions;
using Micro.Transactions.Inbox;
using Micro.Transactions.Outbox;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MySpot.Services.Reservations.Core.Repository;
using MySpot.Services.Reservations.Infrastructure.DAL;
using MySpot.Services.Reservations.Infrastructure.DAL.Repositories;
using MySpot.Services.Reservations.Infrastructure.Messaging;

namespace MySpot.Services.Reservations.Infrastructure;

public static class Extensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        => services
            .AddScoped<IUserRepository, UserRepository>()
            .AddScoped<IWeeklyReservationsRepository, WeeklyReservationsRepository>()
            .AddPostgres<ReservationsDbContext>(configuration)
            .AddSingleton<IMessagingExceptionPolicyResolver, MessagingExceptionPolicyResolver>()
            .AddOutbox<ReservationsDbContext>(configuration)
            .AddInbox<ReservationsDbContext>(configuration)
            .AddMessagingErrorHandlingDecorators()
            .AddTransactionalDecorators()
            .AddOutboxInstantSenderDecorators();
}