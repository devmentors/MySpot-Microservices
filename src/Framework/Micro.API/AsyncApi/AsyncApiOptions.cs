namespace Micro.API.AsyncApi;

public sealed class AsyncApiOptions
{
    public bool Enabled { get; set; }
    public string Title { get; set; } = "Async API";
    public string Version { get; set; } = "1.0";
    public string Description { get; set; } = "Async API description";
    public Dictionary<string, Server> Servers { get; set; } = new();
    public Dictionary<string, Binding> Bindings { get; set; } = new();

    public class Server
    {
        public string Url { get; set; } = string.Empty;
        public string Protocol { get; set; } = string.Empty;
    }

    public class Binding
    {
        public AmqpBinding? Amqp { get; set; }
    }

    public class AmqpBinding
    {
        public string Type { get; set; } = string.Empty;
        public AmqpExchange? Exchange { get; set; }
        public AmqpQueue? Queue { get; set; }
    }

    public class AmqpExchange
    {
        public string Name { get; set; } = string.Empty;
        public string VirtualHost { get; set; } = "/";
    }

    public class AmqpQueue
    {
        public string Name { get; set; } = string.Empty;
        public string VirtualHost { get; set; } = "/";
    }
}
