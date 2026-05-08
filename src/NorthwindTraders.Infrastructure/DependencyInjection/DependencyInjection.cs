using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NorthwindTraders.Application.Abstractions.Persistence;
using NorthwindTraders.Application.Abstractions.Services;
using NorthwindTraders.Infrastructure.Persistence.DbContext;
using NorthwindTraders.Infrastructure.Persistence.Repositories;
using NorthwindTraders.Infrastructure.Persistence.UnitOfWork;
using NorthwindTraders.Infrastructure.Services.AddressValidation;
using NorthwindTraders.Infrastructure.Services.Reports;

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

        services.AddScoped<ICustomerRepository, CustomerRepository>();
        services.AddScoped<IEmployeeRepository, EmployeeRepository>();
        services.AddScoped<IShipperRepository, ShipperRepository>();
        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<IOrderRepository, OrderRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IOrderPdfReportService, QuestPdfOrderReportService>();
        services.AddHttpClient<IAddressValidationService, GoogleMapsAddressValidationService>(client =>
        {
            client.BaseAddress = new Uri("https://addressvalidation.googleapis.com/");
        });

        return services;
    }
}
