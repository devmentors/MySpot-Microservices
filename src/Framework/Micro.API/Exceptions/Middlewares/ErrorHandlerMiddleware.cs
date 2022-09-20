using System.Net;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Micro.API.Exceptions.Middlewares;

internal sealed class ErrorHandlerMiddleware : IMiddleware
{
    private readonly IExceptionToResponseMapper _mapper;
    private readonly ILogger<ErrorHandlerMiddleware> _logger;

    public ErrorHandlerMiddleware(IExceptionToResponseMapper mapper, ILogger<ErrorHandlerMiddleware> logger)
    {
        _mapper = mapper;
        _logger = logger;
    }
        
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try
        {
            await next(context);
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, exception.Message);
            await HandleErrorAsync(context, exception);
        }
    }

    private async Task HandleErrorAsync(HttpContext context, Exception exception)
    {
        var errorResponse = _mapper.Map(exception);
        context.Response.StatusCode = (int) (errorResponse?.StatusCode ?? HttpStatusCode.InternalServerError);
        var response = errorResponse?.Response;
        if (response is null)
        {
            return;
        }
            
        await context.Response.WriteAsJsonAsync(response);
    }
}