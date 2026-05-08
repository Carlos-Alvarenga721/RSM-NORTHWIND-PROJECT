using FluentValidation;
using NorthwindTraders.Application.Abstractions.Persistence;
using NorthwindTraders.Domain.Orders;

namespace NorthwindTraders.Application.DTOs.Orders;

public sealed class CreateOrderRequestValidator : AbstractValidator<CreateOrderRequest>
{
    public CreateOrderRequestValidator(
        ICustomerRepository customerRepository,
        IEmployeeRepository employeeRepository,
        IShipperRepository shipperRepository,
        IProductRepository productRepository)
    {
        RuleFor(request => request.CustomerId)
            .NotEmpty()
            .MaximumLength(OrderBusinessRules.CustomerIdMaxLength)
            .MustAsync(customerRepository.ExistsAsync)
            .WithMessage("CustomerId must reference an existing customer.");

        RuleFor(request => request.EmployeeId)
            .GreaterThan(0)
            .MustAsync(employeeRepository.ExistsAsync)
            .WithMessage("EmployeeId must reference an existing employee.");

        RuleFor(request => request.ShipVia)
            .MustAsync(async (value, cancellationToken) =>
                value is null || await shipperRepository.ExistsAsync(value.Value, cancellationToken))
            .WithMessage("ShipVia must reference an existing shipper.");

        RuleFor(request => request.Freight)
            .GreaterThanOrEqualTo(OrderBusinessRules.MinimumFreight)
            .When(request => request.Freight.HasValue);

        RuleFor(request => request.ShipName).MaximumLength(OrderBusinessRules.ShipNameMaxLength);
        RuleFor(request => request.ShipAddress).MaximumLength(OrderBusinessRules.ShipAddressMaxLength);
        RuleFor(request => request.ShipCity).MaximumLength(OrderBusinessRules.ShipCityMaxLength);
        RuleFor(request => request.ShipRegion).MaximumLength(OrderBusinessRules.ShipRegionMaxLength);
        RuleFor(request => request.ShipPostalCode).MaximumLength(OrderBusinessRules.ShipPostalCodeMaxLength);
        RuleFor(request => request.ShipCountry).MaximumLength(OrderBusinessRules.ShipCountryMaxLength);

        RuleFor(request => request.Details)
            .NotNull()
            .Must(orderDetails => orderDetails is not null && orderDetails.Count > 0)
            .WithMessage("An order must have at least one order detail.")
            .Must(orderDetails => orderDetails is not null
                && orderDetails.Select(detail => detail.ProductId).Distinct().Count() == orderDetails.Count)
            .WithMessage("An order cannot contain duplicate products.");

        RuleForEach(request => request.Details)
            .SetValidator(new OrderDetailRequestValidator(productRepository));
    }
}

public sealed class UpdateOrderRequestValidator : AbstractValidator<UpdateOrderRequest>
{
    public UpdateOrderRequestValidator(
        ICustomerRepository customerRepository,
        IEmployeeRepository employeeRepository,
        IShipperRepository shipperRepository,
        IProductRepository productRepository)
    {
        RuleFor(request => request.CustomerId)
            .NotEmpty()
            .MaximumLength(OrderBusinessRules.CustomerIdMaxLength)
            .MustAsync(customerRepository.ExistsAsync)
            .WithMessage("CustomerId must reference an existing customer.");

        RuleFor(request => request.EmployeeId)
            .GreaterThan(0)
            .MustAsync(employeeRepository.ExistsAsync)
            .WithMessage("EmployeeId must reference an existing employee.");

        RuleFor(request => request.ShipVia)
            .MustAsync(async (value, cancellationToken) =>
                value is null || await shipperRepository.ExistsAsync(value.Value, cancellationToken))
            .WithMessage("ShipVia must reference an existing shipper.");

        RuleFor(request => request.Freight)
            .GreaterThanOrEqualTo(OrderBusinessRules.MinimumFreight)
            .When(request => request.Freight.HasValue);

        RuleFor(request => request.ShipName).MaximumLength(OrderBusinessRules.ShipNameMaxLength);
        RuleFor(request => request.ShipAddress).MaximumLength(OrderBusinessRules.ShipAddressMaxLength);
        RuleFor(request => request.ShipCity).MaximumLength(OrderBusinessRules.ShipCityMaxLength);
        RuleFor(request => request.ShipRegion).MaximumLength(OrderBusinessRules.ShipRegionMaxLength);
        RuleFor(request => request.ShipPostalCode).MaximumLength(OrderBusinessRules.ShipPostalCodeMaxLength);
        RuleFor(request => request.ShipCountry).MaximumLength(OrderBusinessRules.ShipCountryMaxLength);

        RuleFor(request => request.Details)
            .NotNull()
            .Must(orderDetails => orderDetails is not null && orderDetails.Count > 0)
            .WithMessage("An order must have at least one order detail.")
            .Must(orderDetails => orderDetails is not null
                && orderDetails.Select(detail => detail.ProductId).Distinct().Count() == orderDetails.Count)
            .WithMessage("An order cannot contain duplicate products.");

        RuleForEach(request => request.Details)
            .SetValidator(new OrderDetailRequestValidator(productRepository));
    }
}

public sealed class OrderDetailRequestValidator : AbstractValidator<OrderDetailRequest>
{
    public OrderDetailRequestValidator(IProductRepository productRepository)
    {
        RuleFor(detail => detail.ProductId)
            .GreaterThan(0)
            .MustAsync(productRepository.ExistsAsync)
            .WithMessage("ProductId must reference an existing product.");

        RuleFor(detail => detail.UnitPrice)
            .GreaterThanOrEqualTo(OrderBusinessRules.MinimumUnitPrice);

        RuleFor(detail => detail.Quantity)
            .GreaterThanOrEqualTo(OrderBusinessRules.MinimumQuantity);

        RuleFor(detail => detail.Discount)
            .InclusiveBetween(OrderBusinessRules.MinimumDiscount, OrderBusinessRules.MaximumDiscount);
    }
}
