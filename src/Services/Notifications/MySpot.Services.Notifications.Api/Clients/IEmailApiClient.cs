namespace MySpot.Services.Notifications.Api.Clients;

public interface IEmailApiClient
{
    Task SendAsync(string receiver, string title, string body);
}