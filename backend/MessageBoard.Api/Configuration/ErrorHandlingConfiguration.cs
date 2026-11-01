using MessageBoard.Api.ErrorHandling;

namespace MessageBoard.Api.Configuration;

public static class ErrorHandlingConfiguration
{
    public static IServiceCollection AddApiErrorHandling(this IServiceCollection services)
    {
        services.AddProblemDetails();
        services.AddExceptionHandler<ApiExceptionHandler>();

        return services;
    }
}