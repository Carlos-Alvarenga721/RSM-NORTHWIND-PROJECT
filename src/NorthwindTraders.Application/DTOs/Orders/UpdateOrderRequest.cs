namespace NorthwindTraders.Application.DTOs.Orders;

public sealed record UpdateOrderRequest(
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
    IReadOnlyList<OrderDetailRequest> Details);
