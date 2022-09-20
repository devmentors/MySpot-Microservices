using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Saunter;
using Saunter.AsyncApiSchema.v2;
using Saunter.AsyncApiSchema.v2.Bindings;
using Saunter.AsyncApiSchema.v2.Bindings.Amqp;

namespace Micro.API.AsyncApi;

public static class Extensions
{
    private const string SectionName = "asyncApi";
    
    public static IServiceCollection AddAsyncApiDocs(this IServiceCollection services, IConfiguration configuration)
    {
        var section = configuration.GetSection(SectionName);
        var options = section.BindOptions<AsyncApiOptions>();
        services.Configure<AsyncApiOptions>(section);

        if (!options.Enabled)
        {
            return services;
        }

        var asyncApiType = AppDomain.CurrentDomain.GetAssemblies().SelectMany(x => x.GetTypes())
            .FirstOrDefault(x => x.IsAssignableTo(typeof(IAsyncApi)));

        services.AddAsyncApiSchemaGeneration(asyncApi =>
        {
            if (asyncApiType is not null)
            {
                asyncApi.AssemblyMarkerTypes = new[] {asyncApiType};
            }

            asyncApi.AsyncApi = new AsyncApiDocument
            {
                Info = new Info(options.Title, options.Version)
                {
                    Description = options.Description
                },
                Servers = options.Servers.ToDictionary(x =>
                    x.Key, x => new Server(x.Value.Url, x.Value.Protocol)),
                Components =
                {
                    ChannelBindings = options.Bindings.ToDictionary(x => x.Key, x => new ChannelBindings
                    {
                        Amqp = x.Value.Amqp is null
                            ? null
                            : new AmqpChannelBinding
                            {
                                Is = Enum.TryParse<AmqpChannelBindingIs>(x.Value.Amqp.Type, true, out var type)
                                    ? type
                                    : null,
                                Exchange = x.Value.Amqp.Exchange is null
                                    ? null
                                    : new AmqpChannelBindingExchange
                                    {
                                        Name = x.Value.Amqp.Exchange.Name,
                                        VirtualHost = string.IsNullOrWhiteSpace(x.Value.Amqp.Exchange.VirtualHost)
                                            ? "/"
                                            : x.Value.Amqp.Exchange.VirtualHost
                                    },
                                Queue = x.Value.Amqp.Queue is null
                                    ? null
                                    : new AmqpChannelBindingQueue()
                                    {
                                        Name = x.Value.Amqp.Queue.Name,
                                        VirtualHost = string.IsNullOrWhiteSpace(x.Value.Amqp.Queue.VirtualHost)
                                            ? "/"
                                            : x.Value.Amqp.Queue.VirtualHost
                                    }
                            }
                    })
                }
            };
        });

        return services;
    }

    public static IEndpointRouteBuilder MapAsyncApiDocs(this IEndpointRouteBuilder endpoints,
        IConfiguration configuration)
    {
        var section = configuration.GetSection(SectionName);
        var options = section.BindOptions<AsyncApiOptions>();
        if (options.Enabled)
        {
            endpoints.MapAsyncApiDocuments();
            endpoints.MapAsyncApiUi();
        }
        
        return endpoints;
    }
}