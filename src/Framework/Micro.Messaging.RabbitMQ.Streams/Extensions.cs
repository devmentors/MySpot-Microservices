using Micro.Messaging.RabbitMQ.Streams.Publishers;
using Micro.Messaging.RabbitMQ.Streams.Subscribers;
using Micro.Messaging.Streams;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Micro.Messaging.RabbitMQ.Streams;

public static class Extensions
{
    private const string SectionName = "rabbitmq:streams";

    public static IServiceCollection AddRabbitMQStreams(this IServiceCollection services, IConfiguration configuration)
    {
        var section = configuration.GetSection(SectionName);
        var options = section.BindOptions<RabbitMQStreamsOptions>();
        services.Configure<RabbitMQStreamsOptions>(section);

        if (!options.Enabled)
        {
            return services;
        }

        services.AddSingleton<RabbitStreamManager>();
        services.AddHostedService<RabbitStreamInitializer>();
        services.AddSingleton<IStreamPublisher, RabbitMQStreamPublisher>();
        services.AddSingleton<IStreamSubscriber, RabbitMQStreamSubscriber>();

        return services;
    }
}