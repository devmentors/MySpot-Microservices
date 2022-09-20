using EasyNetQ;
using EasyNetQ.Consumer;
using Humanizer;
using Micro.Contexts.Accessors;
using Micro.Handlers;
using Micro.Messaging.Clients;
using Micro.Messaging.RabbitMQ.Exceptions;
using Micro.Messaging.RabbitMQ.Internals;
using Micro.Messaging.Subscribers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using IMessageHandler = Micro.Messaging.RabbitMQ.Internals.IMessageHandler;
using MessageHandler = Micro.Messaging.RabbitMQ.Internals.MessageHandler;

namespace Micro.Messaging.RabbitMQ;

public static class Extensions
{
    public static IServiceCollection AddRabbitMQ(this IServiceCollection services, IConfiguration configuration)
    {
        var section = configuration.GetSection("rabbitmq");
        var options = section.BindOptions<RabbitMqOptions>();
        services.Configure<RabbitMqOptions>(section);
        
        if (!options.Enabled)
        {
            return services;
        }
        
        var contextAccessor = new ContextAccessor();
        var messageContextAccessor = new MessageContextAccessor();
        var messageTypeRegistry = new MessageTypeRegistry();
        
        var bus = RabbitHutch.CreateBus(options.ConnectionString,
            register =>
            {
                register.EnableNewtonsoftJson(new JsonSerializerSettings
                {
                    TypeNameHandling = TypeNameHandling.None,
                    Converters = new List<JsonConverter>
                    {
                        new StringEnumConverter(new CamelCaseNamingStrategy())
                    },
                    ContractResolver = new CamelCasePropertyNamesContractResolver()
                });
                register.Register(typeof(IConventions), typeof(CustomConventions));
                register.Register(typeof(IMessageSerializationStrategy), typeof(CustomMessageSerializationStrategy));
                register.Register(typeof(IHandlerCollectionFactory), typeof(CustomHandlerCollectionFactory));
                register.Register(typeof(IMessageTypeRegistry), messageTypeRegistry);
                register.Register(typeof(IContextAccessor), contextAccessor);
                register.Register(typeof(IMessageContextAccessor), messageContextAccessor);
            });
        
        services.AddSingleton(bus);
        services.AddSingleton<IMessageBrokerClient, RabbitMqBrokerClient>();
        services.AddSingleton<IMessageSubscriber, RabbitMqMessageSubscriber>();
        services.AddSingleton<IMessageHandler, MessageHandler>();
        services.AddSingleton<IMessageTypeRegistry>(messageTypeRegistry);
        services.AddSingleton<IContextAccessor>(contextAccessor);
        services.AddSingleton<IMessageContextAccessor>(messageContextAccessor);

        return services;
    }
    
    public static IServiceCollection AddMessagingErrorHandlingDecorators(this IServiceCollection services)
    {
        services.TryDecorate(typeof(ICommandHandler<>), typeof(MessagingErrorHandlingCommandHandlerDecorator<>));
        services.TryDecorate(typeof(IEventHandler<>), typeof(MessagingErrorHandlingEventHandlerDecorator<>));

        return services;
    }

    internal static string ToMessageKey(this string messageType) => messageType.Underscore();
}