namespace NorthwindTraders.Application.DTOs.Orders;

public sealed record OrderResponse(
    int OrderId,
    string CustomerId,
    string? CustomerName,
    int? EmployeeId,
    string? EmployeeName,
    DateTime? OrderDate,
    DateTime? RequiredDate,
    DateTime? ShippedDate,
    int? ShipVia,
    string? ShipperName,
    decimal Freight,
    string? ShipName,
    string? ShipAddress,
    string? ShipCity,
    string? ShipRegion,
    string? ShipPostalCode,
    string? ShipCountry,
    OrderShippingValidationResponse? ShippingValidation,
    decimal OrderTotal,
    IReadOnlyList<OrderDetailResponse> Details);
