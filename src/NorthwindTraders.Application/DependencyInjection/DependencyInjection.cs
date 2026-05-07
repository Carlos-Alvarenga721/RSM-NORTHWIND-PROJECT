using Microsoft.Extensions.DependencyInjection;
using NorthwindTraders.Application.UseCases.Lookups;

namespace NorthwindTraders.Application.DependencyInjection;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<GetCustomerLookupUseCase>();
        services.AddScoped<GetEmployeeLookupUseCase>();
        services.AddScoped<GetShipperLookupUseCase>();
        services.AddScoped<GetProductLookupUseCase>();

        return services;
    }
}
