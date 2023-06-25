namespace Micro.DAL.Redis;

public class RedisOptions
{
    public bool Enabled { get; set; }
    public string ConnectionString { get; set; } = string.Empty;
    public string Instance { get; set; } = string.Empty;

    public string GetInstance()
        => string.IsNullOrWhiteSpace(Instance) ? string.Empty : Instance.EndsWith(":") ? Instance : $"{Instance}:";
}