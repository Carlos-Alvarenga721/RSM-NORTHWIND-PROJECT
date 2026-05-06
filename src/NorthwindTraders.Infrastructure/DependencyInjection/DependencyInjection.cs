using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NorthwindTraders.Infrastructure.Persistence.DbContext;

namespace NorthwindTraders.Infrastructure.DependencyInjection;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("NorthwindDatabase");

        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException(
                "Connection string 'NorthwindDatabase' is missing. Configure it with user-secrets or environment variables.");
        }

        services.AddDbContext<NorthwindDbContext>(options =>
            options.UseSqlServer(connectionString));

        return services;
    }
}
