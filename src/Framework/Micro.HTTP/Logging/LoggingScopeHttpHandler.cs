using System.Net;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Micro.HTTP.Logging;

internal sealed class LoggingScopeHttpMessageHandler : DelegatingHandler
{
    private readonly ILogger _logger;
    private readonly HashSet<string> _maskedUrlParts;
    private readonly string _maskTemplate;

    public LoggingScopeHttpMessageHandler(ILogger logger, IOptions<HttpClientOptions> options)
    {
        _logger = logger;
        _maskedUrlParts = new HashSet<string>(options.Value.RequestMasking.UrlParts);
        _maskTemplate = string.IsNullOrWhiteSpace(options.Value.RequestMasking.MaskTemplate)
            ? "*****"
            : options.Value.RequestMasking.MaskTemplate;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        if (request is null)
        {
            throw new ArgumentNullException(nameof(request));
        }

        using (Log.BeginRequestPipelineScope(_logger, request, _maskedUrlParts, _maskTemplate))
        {
            Log.HandleRequestPipelineStart(_logger, request, _maskedUrlParts, _maskTemplate);
            var response = await base.SendAsync(request, cancellationToken);
            Log.HandleRequestPipelineEnd(_logger, response);

            return response;
        }
    }

    private static class Log
    {
        private static class EventIds
        {
            public static readonly EventId PipelineStart = new(100, "RequestPipelineStart");
            public static readonly EventId PipelineEnd = new(101, "RequestPipelineEnd");
        }

        private static readonly Func<ILogger, HttpMethod, Uri?, IDisposable?> RequestPipelineScope =
            LoggerMessage.DefineScope<HttpMethod, Uri?>("HTTP {HttpMethod} {Uri}");

        private static readonly Action<ILogger, HttpMethod, Uri?, Exception?> RequestPipelineStart =
            LoggerMessage.Define<HttpMethod, Uri?>(LogLevel.Information, EventIds.PipelineStart,
                "Start processing HTTP request {HttpMethod} {Uri}");

        private static readonly Action<ILogger, HttpStatusCode, Exception?> RequestPipelineEnd =
            LoggerMessage.Define<HttpStatusCode>(LogLevel.Information, EventIds.PipelineEnd,
                "End processing HTTP request - {StatusCode}");

        public static IDisposable? BeginRequestPipelineScope(ILogger logger, HttpRequestMessage request,
            ISet<string> maskedRequestUrlParts, string maskTemplate)
        {
            var uri = MaskUri(request.RequestUri, maskedRequestUrlParts, maskTemplate);
            return RequestPipelineScope(logger, request.Method, uri);
        }

        public static void HandleRequestPipelineStart(ILogger logger, HttpRequestMessage request,
            ISet<string> maskedRequestUrlParts, string maskTemplate)
        {
            var uri = MaskUri(request.RequestUri, maskedRequestUrlParts, maskTemplate);
            RequestPipelineStart(logger, request.Method, uri, null);
        }

        public static void HandleRequestPipelineEnd(ILogger logger, HttpResponseMessage response)
        {
            RequestPipelineEnd(logger, response.StatusCode, null);
        }

        private static Uri? MaskUri(Uri? uri, ISet<string> maskedRequestUrlParts, string maskTemplate)
        {
            if (uri is null)
            {
                return uri;
            }
            
            if (!maskedRequestUrlParts.Any())
            {
                return uri;
            }
                
            var requestUri = uri.OriginalString;
            var hasMatch = false;
            foreach (var part in maskedRequestUrlParts)
            {
                if (string.IsNullOrWhiteSpace(part))
                {
                    continue;
                }
                    
                if (!requestUri.Contains(part))
                {
                    continue;
                }
                    
                requestUri = requestUri.Replace(part, maskTemplate);
                hasMatch = true;
            }

            return hasMatch ? new Uri(requestUri) : uri;
        }
    }
}