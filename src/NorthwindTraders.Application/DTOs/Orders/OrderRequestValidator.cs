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

        RuleFor(request => request.OrderDate)
            .NotNull()
            .WithMessage("OrderDate is required.");

        RuleFor(request => request.RequiredDate)
            .Must((request, requiredDate) => OrderDateValidationRules.IsOnOrAfter(requiredDate, request.OrderDate))
            .WithMessage("RequiredDate cannot be before OrderDate.");

        RuleFor(request => request.ShippedDate)
            .Must((request, shippedDate) => OrderDateValidationRules.IsOnOrAfter(shippedDate, request.OrderDate))
            .WithMessage("ShippedDate cannot be before OrderDate.");

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

        RuleFor(request => request.ShippingValidation!)
            .SetValidator(new OrderShippingValidationRequestValidator())
            .When(request => request.ShippingValidation is not null);
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

        RuleFor(request => request.OrderDate)
            .NotNull()
            .WithMessage("OrderDate is required.");

        RuleFor(request => request.RequiredDate)
            .Must((request, requiredDate) => OrderDateValidationRules.IsOnOrAfter(requiredDate, request.OrderDate))
            .WithMessage("RequiredDate cannot be before OrderDate.");

        RuleFor(request => request.ShippedDate)
            .Must((request, shippedDate) => OrderDateValidationRules.IsOnOrAfter(shippedDate, request.OrderDate))
            .WithMessage("ShippedDate cannot be before OrderDate.");

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

        RuleFor(request => request.ShippingValidation!)
            .SetValidator(new OrderShippingValidationRequestValidator())
            .When(request => request.ShippingValidation is not null);
    }
}

public sealed class OrderShippingValidationRequestValidator : AbstractValidator<OrderShippingValidationRequest>
{
    private static readonly string[] AllowedStatuses =
    [
        "Validated",
        "NeedsReview",
        "ValidationUnavailable"
    ];

    public OrderShippingValidationRequestValidator()
    {
        RuleFor(validation => validation.ValidationStatus)
            .NotEmpty()
            .Must(status => AllowedStatuses.Contains(status))
            .WithMessage("Shipping validation status must be Validated, NeedsReview, or ValidationUnavailable.");

        RuleFor(validation => validation.OriginalAddress).MaximumLength(300);
        RuleFor(validation => validation.FormattedAddress).MaximumLength(300);
        RuleFor(validation => validation.GooglePlaceId).MaximumLength(200);
        RuleFor(validation => validation.ValidationMessage).MaximumLength(500);
        RuleFor(validation => validation.ValidationGranularity).MaximumLength(50);
        RuleFor(validation => validation.GeocodeGranularity).MaximumLength(50);

        RuleFor(validation => validation.Latitude)
            .InclusiveBetween(-90, 90)
            .When(validation => validation.Latitude.HasValue);

        RuleFor(validation => validation.Longitude)
            .InclusiveBetween(-180, 180)
            .When(validation => validation.Longitude.HasValue);
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

internal static class OrderDateValidationRules
{
    public static bool IsOnOrAfter(DateTime? value, DateTime? minimum)
    {
        return !value.HasValue || !minimum.HasValue || value.Value.Date >= minimum.Value.Date;
    }
}
