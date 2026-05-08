namespace NorthwindTraders.Application.DTOs.Orders;

public sealed record OrderSummaryResponse(
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
    string? ShipCity,
    string? ShipCountry,
    int DetailCount,
    decimal OrderTotal);
