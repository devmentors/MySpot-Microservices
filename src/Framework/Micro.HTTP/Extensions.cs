using System.Security.Cryptography.X509Certificates;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Polly;
using Polly.Extensions.Http;

namespace Micro.HTTP;

public static class Extensions
{
    public static IServiceCollection AddHttpClient(this IServiceCollection services, IConfiguration configuration)
    {
        var httpClientSection = configuration.GetSection("httpClient");
        var httpClientOptions = httpClientSection.BindOptions<HttpClientOptions>();
        services.Configure<HttpClientOptions>(httpClientSection);

        var builder = services
            .AddHttpClient(httpClientOptions.Name)
            .AddTransientHttpErrorPolicy(_ => HttpPolicyExtensions.HandleTransientHttpError()
                .WaitAndRetryAsync(httpClientOptions.Resiliency.Retries, retry =>
                    httpClientOptions.Resiliency.Exponential
                        ? TimeSpan.FromSeconds(Math.Pow(2, retry))
                        : httpClientOptions.Resiliency.RetryInterval ?? TimeSpan.FromSeconds(2)));

        var certificateLocation = httpClientOptions.Certificate?.Location;
        if (httpClientOptions.Certificate is not null && !string.IsNullOrWhiteSpace(certificateLocation))
        {
            var certificate = new X509Certificate2(certificateLocation, httpClientOptions.Certificate.Password);
            builder.ConfigurePrimaryHttpMessageHandler(() =>
            {
                var handler = new HttpClientHandler();
                handler.ClientCertificates.Add(certificate);
                return handler;
            });
        }

        return services;
    }
}