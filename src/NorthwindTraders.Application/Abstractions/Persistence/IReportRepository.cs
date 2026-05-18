using NorthwindTraders.Application.DTOs.Reports;

namespace NorthwindTraders.Application.Abstractions.Persistence;

/// <summary>
/// Provides the filtered reporting projection without exposing EF Core queries to Application use cases.
/// </summary>
public interface IReportRepository
{
    Task<OrdersReportResponse> GetOrdersReportAsync(
        ReportFilterRequest filters,
        CancellationToken cancellationToken = default);
}
