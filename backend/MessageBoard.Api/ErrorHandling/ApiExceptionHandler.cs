using System.Diagnostics;
using FluentValidation;
using MessageBoard.Application.Comments.Exceptions;
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
        CancellationToken ct)
    {
        var traceId = Activity.Current?.Id ?? httpContext.TraceIdentifier;

        // Возвращаем ошибки валидации по полям для отображения на клиенте
        if (exception is ValidationException validationException)
        {
            var errors = validationException.Errors
                .GroupBy(error => error.PropertyName)
                .ToDictionary(
                    group => group.Key,
                    group => group.Select(error => error.ErrorMessage).ToArray());

            await Results.ValidationProblem(
                errors,
                extensions: new Dictionary<string, object?>
                {
                    ["traceId"] = traceId
                }
            ).ExecuteAsync(httpContext);

            return true;
        }

        var statusCode = exception switch
        {
            ParentCommentNotFoundException => StatusCodes.Status404NotFound,
            BadHttpRequestException badRequest => badRequest.StatusCode,

            _ => StatusCodes.Status500InternalServerError
        };

        if (statusCode >= StatusCodes.Status500InternalServerError)
        {
            _logger.LogError(
                exception,
                "Request {Method} {Path} failed. TraceId: {TraceId}",
                httpContext.Request.Method,
                httpContext.Request.Path.Value,
                traceId);
        }

        // Не раскрываем внутренние ошибки клиенту: детали остаются в логах
        var problem = new ProblemDetails
        {
            Status = statusCode,
            Title = exception switch
            {
                ParentCommentNotFoundException => "Parent comment not found.",
                _ when statusCode >= 500 => "Server error.",

                _ => "Invalid request."
            },
            Detail = exception switch
            {
                ParentCommentNotFoundException => "The specified parent comment does not exist.",
                _ when statusCode >= 500 => "An unexpected error occurred.",

                _ => "The request could not be processed."
            }
        };

        problem.Extensions["traceId"] = traceId;

        await Results.Problem(problem).ExecuteAsync(httpContext);

        return true;
    }
}
