using FluentValidation;
using NorthwindTraders.Application.Abstractions.Persistence;
using NorthwindTraders.Application.Abstractions.Services;
using NorthwindTraders.Application.DTOs.Reports;

namespace NorthwindTraders.Application.UseCases.Reports.ExportOrdersReport;

public sealed class ExportOrdersReportUseCase(
    IReportRepository reportRepository,
    IOrdersReportExportService exportService,
    IValidator<ReportFilterRequest> validator)
{
    public async Task<byte[]> ExecuteExcelAsync(
        ReportFilterRequest filters,
        CancellationToken cancellationToken = default)
    {
        await validator.ValidateAndThrowAsync(filters, cancellationToken);

        var report = (await reportRepository.GetOrdersReportAsync(filters, cancellationToken)
            ?? OrdersReportResponse.Empty).Normalize();

        return exportService.GenerateExcel(report, filters);
    }

    public async Task<byte[]> ExecutePdfAsync(
        ReportFilterRequest filters,
        CancellationToken cancellationToken = default)
    {
        await validator.ValidateAndThrowAsync(filters, cancellationToken);

        var report = (await reportRepository.GetOrdersReportAsync(filters, cancellationToken)
            ?? OrdersReportResponse.Empty).Normalize();

        return exportService.GeneratePdf(report, filters);
    }
}
