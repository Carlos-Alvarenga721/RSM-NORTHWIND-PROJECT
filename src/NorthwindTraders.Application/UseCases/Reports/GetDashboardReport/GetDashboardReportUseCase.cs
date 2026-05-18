using FluentValidation;
using NorthwindTraders.Application.Abstractions.Persistence;
using NorthwindTraders.Application.DTOs.Reports;

namespace NorthwindTraders.Application.UseCases.Reports.GetDashboardReport;

/// <summary>
/// Builds high-level dashboard metrics from the same normalized report data used by charts and tables.
/// </summary>
public sealed class GetDashboardReportUseCase(
    IReportRepository reportRepository,
    IValidator<ReportFilterRequest> validator)
{
    public async Task<DashboardReportResponse> ExecuteAsync(
        ReportFilterRequest filters,
        CancellationToken cancellationToken = default)
    {
        await validator.ValidateAndThrowAsync(filters, cancellationToken);

        var report = (await reportRepository.GetOrdersReportAsync(filters, cancellationToken)
            ?? OrdersReportResponse.Empty).Normalize();

        // These totals are intentionally derived from filtered rows, matching what users see in the table.
        return new DashboardReportResponse(
            report.Orders.Count,
            report.ShipmentsByRegion.Count,
            report.Orders.Sum(order => order.OrderTotal),
            report);
    }
}
