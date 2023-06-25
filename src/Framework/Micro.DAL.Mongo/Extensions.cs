using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;

namespace Micro.DAL.Mongo;

public static class Extensions
{
    public static IServiceCollection AddMongo(this IServiceCollection services, IConfiguration configuration)
    {
        var section = configuration.GetSection("mongo");
        var options = section.BindOptions<MongoOptions>();
        services.Configure<MongoOptions>(section);
        if (!section.Exists() || !options.Enabled)
        {
            return services;
        }

        var mongoClient = new MongoClient(options.ConnectionString);
        var database = mongoClient.GetDatabase(options.Database);
        services.AddSingleton<IMongoClient>(mongoClient);
        services.AddSingleton(database);

        RegisterConventions();

        return services;
    }
    
    private static void RegisterConventions()
    {
        BsonSerializer.RegisterSerializer(typeof(decimal), new DecimalSerializer(BsonType.Decimal128));
        BsonSerializer.RegisterSerializer(typeof(decimal?),
            new NullableSerializer<decimal>(new DecimalSerializer(BsonType.Decimal128)));
        ConventionRegistry.Register("convey", new ConventionPack
        {
            new CamelCaseElementNameConvention(),
            new IgnoreExtraElementsConvention(true),
            new EnumRepresentationConvention(BsonType.String),
        }, _ => true);
    }
}