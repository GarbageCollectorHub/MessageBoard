using FluentValidation;
using MessageBoard.Application.Comments.Validation;
using Microsoft.Extensions.DependencyInjection;

namespace MessageBoard.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddValidatorsFromAssemblyContaining<CreateCommentRequestDtoValidator>();

        return services;
    }
}