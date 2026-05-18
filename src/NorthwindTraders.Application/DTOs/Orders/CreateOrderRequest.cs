namespace NorthwindTraders.Application.DTOs.Orders;

/// <summary>
/// API contract for creating an order header, optional validated shipping metadata, and product details together.
/// </summary>
public sealed record CreateOrderRequest(
    string CustomerId,
    int EmployeeId,
    DateTime? OrderDate,
    DateTime? RequiredDate,
    DateTime? ShippedDate,
    int? ShipVia,
    decimal? Freight,
    string? ShipName,
    string? ShipAddress,
    string? ShipCity,
    string? ShipRegion,
    string? ShipPostalCode,
    string? ShipCountry,
    OrderShippingValidationRequest? ShippingValidation,
    IReadOnlyList<OrderDetailRequest> Details);
