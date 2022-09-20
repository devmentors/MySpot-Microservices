namespace Micro.Contexts.HTTP;

internal sealed class ContextHttpHandler : DelegatingHandler
{
    private const string CorrelationIdKey = "correlation-id";
    private readonly IContextProvider _contextProvider;

    public ContextHttpHandler(IContextProvider contextProvider)
    {
        _contextProvider = contextProvider;
    }
    
    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var context = _contextProvider.Current();
        request.Headers.TryAddWithoutValidation(CorrelationIdKey, context.CorrelationId);
        return await base.SendAsync(request, cancellationToken);
    }
}