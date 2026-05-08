using Microsoft.Extensions.DependencyInjection;
using NorthwindTraders.Application.UseCases.Customers.GetCustomersLookup;
using NorthwindTraders.Application.UseCases.Employees.GetEmployeesLookup;
using NorthwindTraders.Application.UseCases.Products.GetProductsLookup;
using NorthwindTraders.Application.UseCases.Shippers.GetShippersLookup;

namespace NorthwindTraders.Application.DependencyInjection;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<GetCustomersLookupUseCase>();
        services.AddScoped<GetEmployeesLookupUseCase>();
        services.AddScoped<GetShippersLookupUseCase>();
        services.AddScoped<GetProductsLookupUseCase>();

        return services;
    }
}
