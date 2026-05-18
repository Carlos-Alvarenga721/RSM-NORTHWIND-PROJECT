using FluentValidation;
using NorthwindTraders.Application.Abstractions.Persistence;
using NorthwindTraders.Application.DTOs.Reports;

namespace NorthwindTraders.Application.UseCases.Reports.GetOrdersReport;

/// <summary>
/// Loads the filtered report model that powers the reporting dashboard and export workflow.
/// </summary>
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

        // Empty reports still return stable collections so frontend charts and tables can render predictably.
        return (report ?? OrdersReportResponse.Empty).Normalize();
    }
}
