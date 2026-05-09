using FluentValidation;
using NorthwindTraders.Application.Abstractions.Persistence;
using NorthwindTraders.Application.DTOs.Reports;

namespace NorthwindTraders.Application.UseCases.Reports.GetDashboardReport;

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

        return new DashboardReportResponse(
            report.Orders.Count,
            report.ShipmentsByRegion.Count,
            report.Orders.Sum(order => order.OrderTotal),
            report);
    }
}
