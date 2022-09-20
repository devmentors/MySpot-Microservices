using System.Diagnostics;
using Micro.Contexts.Accessors;
using Microsoft.AspNetCore.Http;

namespace Micro.Contexts.Providers;

internal sealed class ContextProvider : IContextProvider
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IContextAccessor _contextAccessor;

    public ContextProvider(IHttpContextAccessor httpContextAccessor, IContextAccessor contextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
        _contextAccessor = contextAccessor;
    }

    public IContext Current()
    {
        if (_contextAccessor.Context is not null)
        {
            return _contextAccessor.Context;
        }

        IContext context;
        var httpContext = _httpContextAccessor.HttpContext;
        if (httpContext is not null)
        {
            var traceId = httpContext.TraceIdentifier;
            var correlationId = httpContext.GetCorrelationId() ?? Guid.NewGuid().ToString("N");
            var userId = httpContext.User.Identity?.Name;
            context = new Context(Activity.Current?.Id ?? ActivityTraceId.CreateRandom().ToString(),
                traceId, correlationId, string.Empty, string.Empty, userId);
        }
        else
        {
            context = new Context(Activity.Current?.Id ?? ActivityTraceId.CreateRandom().ToString(),
                string.Empty, Guid.NewGuid().ToString("N"));
        }

        _contextAccessor.Context = context;

        return context;
    }
}