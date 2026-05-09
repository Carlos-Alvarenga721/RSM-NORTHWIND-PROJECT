namespace NorthwindTraders.Application.DTOs.Reports;

public sealed record ReportOrderRow(
    int OrderId,
    string? CustomerName,
    string? EmployeeName,
    DateTime? OrderDate,
    DateTime? ShippedDate,
    string? ShipRegion,
    string? ShipCountry,
    decimal OrderTotal);
