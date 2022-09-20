using System.Collections.Concurrent;
using System.Net;
using Humanizer;
using Micro.Exceptions;

namespace Micro.API.Exceptions.Mappers;

internal sealed class ExceptionToResponseMapper : IExceptionToResponseMapper
{
    private readonly ExceptionResponse _defaultResponse =
        new(new ErrorsResponse(new Error("error", "There was an error.")), HttpStatusCode.InternalServerError);

    private readonly ExceptionResponse _communicationResponse = new(
        new ErrorsResponse(new Error("internal_service_http_communication",
            "There was an internal HTTP service communication error.")), HttpStatusCode.InternalServerError);
    
    private readonly ConcurrentDictionary<Type, string> _codes = new();

    public ExceptionResponse Map(Exception exception)
        => exception switch
        {
            CustomException ex => new ExceptionResponse(new ErrorsResponse(new Error(GetErrorCode(ex), ex.Message))
                , HttpStatusCode.BadRequest),
            HttpRequestException _ => _communicationResponse,
            _ => _defaultResponse
        };

    private record Error(string Code, string Message);

    private record ErrorsResponse(params Error[] Errors);

    private string GetErrorCode(object exception)
    {
        var type = exception.GetType();
        return _codes.GetOrAdd(type, type.Name.Underscore().Replace("_exception", string.Empty));
    }
}