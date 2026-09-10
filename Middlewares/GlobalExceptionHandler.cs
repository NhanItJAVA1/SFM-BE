using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using SFM_BE.Exceptions;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SFM_BE.Middlewares;

public class GlobalExceptionHandler : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger;

    public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
    {
        _logger = logger;
    }

    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        System.Exception exception,
        CancellationToken cancellationToken)
    {
        _logger.LogError(exception, "An unhandled exception occurred.");

        var statusCode = exception is AppException appException
            ? appException.StatusCode
            : StatusCodes.Status500InternalServerError;

        var errorCode = exception is AppException appExceptionWithCode
            ? appExceptionWithCode.ErrorCode
            : "INTERNAL_SERVER_ERROR";

        var response = new
        {
            success = false,
            statusCode,
            errorCode,
            message = exception is AppException ? exception.Message : "An unexpected error occurred.",
            timestamp = DateTime.UtcNow,
            path = httpContext.Request.Path.ToString()
        };

        httpContext.Response.StatusCode = statusCode;

        await httpContext.Response.WriteAsJsonAsync(response, cancellationToken);
        return true;
    }
}
