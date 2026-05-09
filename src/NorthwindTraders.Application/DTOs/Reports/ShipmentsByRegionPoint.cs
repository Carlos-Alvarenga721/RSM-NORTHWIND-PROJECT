namespace NorthwindTraders.Application.DTOs.Reports;

public sealed record ShipmentsByRegionPoint(
    string? Region,
    int ShipmentCount);
