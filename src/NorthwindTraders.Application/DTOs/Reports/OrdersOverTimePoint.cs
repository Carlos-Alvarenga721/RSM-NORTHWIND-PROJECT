namespace NorthwindTraders.Application.DTOs.Reports;

public sealed record OrdersOverTimePoint(
    string Label,
    int OrderCount,
    decimal TotalSales);
