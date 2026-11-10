using MessageBoard.Application.Comments.DTOs;
using System.Text.Json.Nodes;

namespace MessageBoard.Api.Configuration;

public static class OpenApiConfiguration
{
    public static IServiceCollection AddApiDocumentation(this IServiceCollection services)
    {

        services.AddOpenApi(options =>
        {
            options.AddSchemaTransformer((schema, context, ct) =>
            {
                if (context.JsonTypeInfo.Type == typeof(CreateCommentRequestDto))
                {
                    schema.Examples =
                    [
                        new JsonObject
                {
                    ["userName"] = "TestUser",
                    ["email"] = "test@example.com",
                    ["homePage"] = "https://test-example.com",
                    ["text"] = "Hello World!",
                    ["parentId"] = null
                }
                    ];
                }

                return Task.CompletedTask;
            });
        });

        return services;
    }


    public static WebApplication UseApiDocumentation(this WebApplication app)
    {
        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();

            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint(
                    "/openapi/v1.json",
                    "MessageBoard API v1");
            });
        }

        return app;
    }

}