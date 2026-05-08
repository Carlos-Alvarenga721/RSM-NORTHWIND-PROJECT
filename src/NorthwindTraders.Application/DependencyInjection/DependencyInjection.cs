using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using NorthwindTraders.Application.DTOs.Orders;
using NorthwindTraders.Application.UseCases.AddressValidation.ValidateAddress;
using NorthwindTraders.Application.UseCases.Customers.GetCustomersLookup;
using NorthwindTraders.Application.UseCases.Employees.GetEmployeesLookup;
using NorthwindTraders.Application.UseCases.Orders.CreateOrder;
using NorthwindTraders.Application.UseCases.Orders.DeleteOrder;
using NorthwindTraders.Application.UseCases.Orders.GenerateOrderPdf;
using NorthwindTraders.Application.UseCases.Orders.GetOrderById;
using NorthwindTraders.Application.UseCases.Orders.GetOrders;
using NorthwindTraders.Application.UseCases.Orders.UpdateOrder;
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
        services.AddScoped<GetOrdersUseCase>();
        services.AddScoped<GetOrderByIdUseCase>();
        services.AddScoped<CreateOrderUseCase>();
        services.AddScoped<UpdateOrderUseCase>();
        services.AddScoped<DeleteOrderUseCase>();
        services.AddScoped<GenerateOrderPdfUseCase>();
        services.AddScoped<ValidateAddressUseCase>();
        services.AddScoped<IValidator<CreateOrderRequest>, CreateOrderRequestValidator>();
        services.AddScoped<IValidator<UpdateOrderRequest>, UpdateOrderRequestValidator>();

        return services;
    }
}
