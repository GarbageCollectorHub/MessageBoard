using System.Diagnostics;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace MessageBoard.Api.ErrorHandling;

public class ApiExceptionHandler : IExceptionHandler
{
    private readonly ILogger<ApiExceptionHandler> _logger;

    public ApiExceptionHandler(ILogger<ApiExceptionHandler> logger)
    {
        _logger = logger;
    }

    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        var statusCode = exception is BadHttpRequestException requestException
            ? requestException.StatusCode
            : StatusCodes.Status500InternalServerError;

        var traceId = Activity.Current?.Id ?? httpContext.TraceIdentifier;

        if (statusCode >= StatusCodes.Status500InternalServerError)
        {
            _logger.LogError(
                exception,
                "Request {Method} {Path} failed. TraceId: {TraceId}",
                httpContext.Request.Method,
                httpContext.Request.Path.Value,
                traceId);
        }

        var problem = new ProblemDetails
        {
            Status = statusCode,
            Title = statusCode >= StatusCodes.Status500InternalServerError
                ? "Server error."
                : "Invalid request.",
            Detail = statusCode >= StatusCodes.Status500InternalServerError
                ? "An unexpected error occurred."
                : "The request could not be processed."
        };

        problem.Extensions["traceId"] = traceId;

        var response = Results.Problem(problem);

        await response.ExecuteAsync(httpContext);

        return true;
    }
}