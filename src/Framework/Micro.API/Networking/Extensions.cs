using System.Net;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Micro.API.Networking;

public static class Extensions
{
    private const string SectionName = "networking";
    private const string HeaderName = "x-forwarded-for";
    private const string DefaultIpAddress = "::ffff:10.0.0.0";

    public static IServiceCollection AddHeadersForwarding(this IServiceCollection services, IConfiguration configuration)
    {
        var section = configuration.GetSection(SectionName);
        services.Configure<NetworkingOptions>(section);

        return services;
    }
        
    public static IApplicationBuilder UseHeadersForwarding(this IApplicationBuilder app)
    {
        var options = app.ApplicationServices.GetRequiredService<IOptions<NetworkingOptions>>().Value;
        if (!options.Enabled)
        {
            return app;
        }
        
        var knownNetworks = new List<IPNetwork>();
        if (!options.Networks.Any())
        {
            knownNetworks.Add(new IPNetwork(IPAddress.Parse(DefaultIpAddress), 8));
        }
        else
        {
            options.Networks
                .ForEach(n => knownNetworks.Add(new IPNetwork(IPAddress.Parse(n.Prefix), n.PrefixLength)));
        }

        var headersOptions = new ForwardedHeadersOptions
        {
            ForwardedHeaders = ForwardedHeaders.All,
            ForwardLimit = null
        };

        headersOptions.KnownNetworks.Clear();
        knownNetworks.ForEach(kn => headersOptions.KnownNetworks.Add(kn));

        return app.UseForwardedHeaders(headersOptions);
    }
        
    public static string GetUserIpAddress(this HttpContext? context)
    {
        if (context is null)
        {
            return string.Empty;
        }

        var ipAddress = context.Connection.RemoteIpAddress?.ToString();
        if (!context.Request.Headers.TryGetValue(HeaderName, out var forwardedFor))
        {
            return string.Empty;
        }

        var ipAddresses = forwardedFor.ToString().Split(",", StringSplitOptions.RemoveEmptyEntries);
        if (ipAddresses.Any())
        {
            ipAddress = ipAddresses[0];
        }

        return ipAddress ?? string.Empty;
    }
}