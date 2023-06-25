namespace Micro.DAL.Mongo;

public class MongoOptions
{
    public bool Enabled { get; set; }
    public string ConnectionString { get; set; } = null!;
    public string Database { get; set; } = null!;
}