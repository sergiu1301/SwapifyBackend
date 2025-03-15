using Newtonsoft.Json;
using System.Dynamic;
using System.Net.Mime;
using Swapify.Infrastructure.Exceptions;
using Validation;

namespace Swapify.Host.Middlewares;

public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionMiddleware> _logger;

    private readonly Dictionary<ErrorTypes, int> _errorTypeToHttpStatusCodeMap = new()
    {
        { ErrorTypes.ClientError, StatusCodes.Status400BadRequest },
        { ErrorTypes.ServerError, StatusCodes.Status500InternalServerError },
        { ErrorTypes.AuthenticationRequired, StatusCodes.Status401Unauthorized },
        { ErrorTypes.ResourceNotFound, StatusCodes.Status404NotFound },
        { ErrorTypes.ResourceAlreadyExists, StatusCodes.Status409Conflict },
        { ErrorTypes.AccessForbidden, StatusCodes.Status403Forbidden },
        { ErrorTypes.NotAllowed, StatusCodes.Status405MethodNotAllowed },
        { ErrorTypes.ValidationError, StatusCodes.Status400BadRequest },
        { ErrorTypes.Conflict, StatusCodes.Status409Conflict},
        { ErrorTypes.TooManyRequests, StatusCodes.Status429TooManyRequests },
        { ErrorTypes.FailedDependency, StatusCodes.Status424FailedDependency }
    };

    public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task Invoke(HttpContext context)
    {
        Requires.NotNull(context, nameof(context));

        if (context.Request.IsApiCall() || context.Request.IsAuthEndpoint())
        {
            await ProcessExceptions(context);
        }
        else
        {
            await _next(context);
        }
    }

    private async Task ProcessExceptions(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (BaseException ex)
        {
            await HandleGeneralException(context, ex);
        }
        catch (Exception ex)
        {
            await HandleApiInternalException(context, ex);
        }
    }

    private Task HandleApiInternalException(HttpContext context, Exception ex)
    {
        _logger.LogError(ex, "An error occurred while processing an API request.");

        dynamic errorResponse = CreateErrorResponse(StatusCodes.Status500InternalServerError.ToString(), "Internal server error.");
        return HandleException(context, StatusCodes.Status500InternalServerError, errorResponse);
    }

    private Task HandleGeneralException(HttpContext context, BaseException ex)
    {
        if (ex.ErrorType == ErrorTypes.ServerError)
        {
            _logger.LogError(ex, "An error occurred while processing an API request");
        }
        else
        {
            _logger.LogWarning(ex, "A warning occurred while processing an API request");
        }

        int httpStatusCode = GetHttpCodeByErrorType(ex.ErrorType);
        dynamic errorResponse = CreateErrorResponse(ex.ErrorCode, ex.Message);
        return HandleException(context, httpStatusCode, errorResponse);
    }

    private dynamic CreateErrorResponse(string errorCode, string message)
    {
        dynamic errorResponse = new ExpandoObject();

        errorResponse.ErrorCode = errorCode;
        errorResponse.Message = message;

        return errorResponse;
    }

    private Task HandleException(HttpContext context, int httpStatusCode, object errorResponse)
    {
        context.Response.ContentType = MediaTypeNames.Application.Json;
        context.Response.StatusCode = httpStatusCode;

        return context.Response.WriteAsync(JsonConvert.SerializeObject(errorResponse));
    }

    private int GetHttpCodeByErrorType(ErrorTypes errorType)
    {
        if (!_errorTypeToHttpStatusCodeMap.TryGetValue(errorType, out int statusCode))
        {
            statusCode = StatusCodes.Status500InternalServerError;
        }
        return statusCode;
    }
}