using System.Diagnostics.CodeAnalysis;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;

namespace Micro.Testing;

[ExcludeFromCodeCoverage]
public class TestApp<T> : WebApplicationFactory<T> where T : class
{
    private readonly TestAuthenticator _authenticator;

    public HttpClient Client { get; }
    public Uri? ServerUrl { get; }

    public void Authenticate(string userId)
    {
        var jwt = _authenticator.GenerateJsonWebToken(userId);
        Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwt.AccessToken);
    }

    public TestApp(Action<IServiceCollection>? services = null, string environment = "test", string serverUrl = "")
    {
        _authenticator = new TestAuthenticator();
        
        Client = WithWebHostBuilder(builder =>
        {
            if (services is not null)
            {
                builder.ConfigureServices(services);
            }
            
            if (!string.IsNullOrWhiteSpace(environment))
            {
                builder.UseEnvironment(environment);
            }

            if (!string.IsNullOrWhiteSpace(serverUrl))
            {
                builder.UseUrls(serverUrl);
            }
        }).CreateClient();

        if (string.IsNullOrWhiteSpace(serverUrl))
        {
            return;
        }
        
        ServerUrl = new Uri(serverUrl);
        Client.BaseAddress = ServerUrl;
    }
}