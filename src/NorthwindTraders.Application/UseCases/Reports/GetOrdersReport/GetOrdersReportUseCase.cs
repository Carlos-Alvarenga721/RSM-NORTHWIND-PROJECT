using FluentValidation;
using NorthwindTraders.Application.Abstractions.Persistence;
using NorthwindTraders.Application.DTOs.Reports;

namespace NorthwindTraders.Application.UseCases.Reports.GetOrdersReport;

public sealed class GetOrdersReportUseCase(
    IReportRepository reportRepository,
    IValidator<ReportFilterRequest> validator)
{
    public async Task<OrdersReportResponse> ExecuteAsync(
        ReportFilterRequest filters,
        CancellationToken cancellationToken = default)
    {
        await validator.ValidateAndThrowAsync(filters, cancellationToken);

        var report = await reportRepository.GetOrdersReportAsync(filters, cancellationToken);

        return (report ?? OrdersReportResponse.Empty).Normalize();
    }
}
