namespace MySpot.Services.Notifications.Api.Entities;

public class Template
{
    public Guid Id { get; private set; }
    public string Name { get; private set; }
    public string Title { get; private set; }
    public string Body { get; private set; }

    public Template(Guid id, string name, string title, string body)
    {
        Id = id;
        Name = name;
        Title = title;
        Body = body;
    }
}