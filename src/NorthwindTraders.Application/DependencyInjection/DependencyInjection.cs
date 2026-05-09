using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using NorthwindTraders.Application.DTOs.Orders;
using NorthwindTraders.Application.DTOs.Reports;
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
using NorthwindTraders.Application.UseCases.Reports.ExportOrdersReport;
using NorthwindTraders.Application.UseCases.Reports.GetDashboardReport;
using NorthwindTraders.Application.UseCases.Reports.GetOrdersReport;
using NorthwindTraders.Application.UseCases.Shippers.GetShippersLookup;
using NorthwindTraders.Application.DTOs.AddressValidation;
using NorthwindTraders.Application.Validators;

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
        services.AddScoped<GetOrdersReportUseCase>();
        services.AddScoped<GetDashboardReportUseCase>();
        services.AddScoped<ExportOrdersReportUseCase>();
        services.AddScoped<IValidator<CreateOrderRequest>, CreateOrderRequestValidator>();
        services.AddScoped<IValidator<UpdateOrderRequest>, UpdateOrderRequestValidator>();
        services.AddScoped<IValidator<ReportFilterRequest>, ReportFilterRequestValidator>();
        services.AddScoped<IValidator<AddressValidationRequest>, AddressValidationRequestValidator>();

        return services;
    }
}
