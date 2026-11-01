using Serilog;

namespace MessageBoard.Api.Configuration;

public static class LoggingConfiguration
{
    public static IServiceCollection AddApiLogging(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSerilog(logger =>
        {
            logger.ReadFrom.Configuration(configuration);
        });

        return services;
    }
}