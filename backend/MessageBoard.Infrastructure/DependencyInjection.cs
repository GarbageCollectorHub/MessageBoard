using MessageBoard.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MessageBoard.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<MessageBoardDbContext>(options =>
        {
            var connectionString = configuration.GetConnectionString("MessageBoard");

            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new InvalidOperationException("Database connection string 'ConnectionStrings:MessageBoard' is not configured.");
            }

            options.UseSqlServer(connectionString);
        });

        return services;
    }
}