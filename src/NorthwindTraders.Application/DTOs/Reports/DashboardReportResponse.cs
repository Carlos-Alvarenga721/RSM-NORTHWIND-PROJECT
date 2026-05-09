namespace NorthwindTraders.Application.DTOs.Reports;

public sealed record DashboardReportResponse(
    int OrderCount,
    int RegionCount,
    decimal TotalSales,
    OrdersReportResponse OrdersReport);
