using System.Net;

namespace Micro.API.Exceptions;

public sealed record ExceptionResponse(object Response, HttpStatusCode StatusCode);