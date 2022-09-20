using Microsoft.Extensions.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Micro.HTTP.Logging;

//Credits goes to https://www.stevejgordon.co.uk/httpclientfactory-asp-net-core-logging
internal sealed class HttpLoggingFilter : IHttpMessageHandlerBuilderFilter
{
    private readonly ILoggerFactory _loggerFactory;
    private readonly IOptions<HttpClientOptions> _options;

    public HttpLoggingFilter(ILoggerFactory loggerFactory, IOptions<HttpClientOptions> options)
    {
        _loggerFactory = loggerFactory ?? throw new ArgumentNullException(nameof(loggerFactory));
        _options = options;
    }

    public Action<HttpMessageHandlerBuilder> Configure(Action<HttpMessageHandlerBuilder> next)
    {
        if (next is null)
        {
            throw new ArgumentNullException(nameof(next));
        }

        return builder =>
        {
            next(builder);
            var logger = _loggerFactory.CreateLogger($"System.Net.Http.HttpClient.{builder.Name}.LogicalHandler");
            builder.AdditionalHandlers.Insert(0, new LoggingScopeHttpMessageHandler(logger, _options));
        };
    }
}