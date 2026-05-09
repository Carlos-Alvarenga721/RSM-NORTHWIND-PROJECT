using NorthwindTraders.Application.DTOs.Reports;

namespace NorthwindTraders.Application.Abstractions.Persistence;

public interface IReportRepository
{
    Task<OrdersReportResponse> GetOrdersReportAsync(
        ReportFilterRequest filters,
        CancellationToken cancellationToken = default);
}
