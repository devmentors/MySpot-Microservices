using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;

namespace Micro.DAL.Redis;

public static class Extensions
{
    public static IServiceCollection AddRedis(this IServiceCollection services, IConfiguration configuration)
    {
        var section = configuration.GetSection("redis");
        var options = section.BindOptions<RedisOptions>();
        services.Configure<RedisOptions>(section);
        if (!section.Exists() || !options.Enabled)
        {
            return services;
        }

        services.AddStackExchangeRedisCache(o =>
        {
            o.Configuration = options.ConnectionString;
            o.InstanceName = options.GetInstance();
        });

        var redisConnectionMultiplexer = ConnectionMultiplexer.Connect(options.ConnectionString);
        services.AddSingleton<IConnectionMultiplexer>(redisConnectionMultiplexer);
        services.AddTransient(ctx => ctx.GetRequiredService<IConnectionMultiplexer>().GetDatabase());

        return services;
    }
}