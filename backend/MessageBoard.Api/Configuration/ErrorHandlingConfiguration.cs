using MessageBoard.Api.ErrorHandling;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Diagnostics;

namespace MessageBoard.Api.Configuration;

public static class ErrorHandlingConfiguration
{
    public static IServiceCollection AddApiErrorHandling(this IServiceCollection services)
    {
        services.AddProblemDetails();
        services.AddExceptionHandler<ApiExceptionHandler>();

        // Настраиваем ответ при невалидном ModelState (model binding и JSON-десериализация).
        // [ApiController] автоматически возвращает 400 до вызова action контроллера.
        // Формируем ValidationProblemDetails и скрываем технические детали ошибок парсинга.
        services.Configure<ApiBehaviorOptions>(options =>
        {
            options.InvalidModelStateResponseFactory = context =>
            {
                var errors = context.ModelState
                    .Where(entry => entry.Value is { Errors.Count: > 0 })
                    .ToDictionary(
                        entry => entry.Key,
                        entry => entry.Value!.Errors
                            .Select(error =>
                                IsParsingError(entry.Key, error)
                                    ? "Invalid value."
                                    : error.ErrorMessage)
                            .ToArray());

                var problem = new ValidationProblemDetails(errors)
                {
                    Status = StatusCodes.Status400BadRequest,
                    Title = "One or more validation errors occurred."
                };

                problem.Extensions["traceId"] = 
                    Activity.Current?.Id ?? context.HttpContext.TraceIdentifier;

                return new BadRequestObjectResult(problem);
            };
        });

        return services;
    }


    // Ошибки JSON-десериализации обычно имеют ключ вида "$.parentId".
    // Наличие Exception также может указывать на ошибку преобразования данных.
    // Такие ошибки заменяем на "Invalid value.", чтобы не раскрывать технические детали.
    private static bool IsParsingError(string key, ModelError error)
    {
        return key.StartsWith("$", StringComparison.Ordinal) 
            || error.Exception is not null;
    }


}