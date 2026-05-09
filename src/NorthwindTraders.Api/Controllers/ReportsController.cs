using Microsoft.AspNetCore.Mvc;
using NorthwindTraders.Application.DTOs.Reports;
using NorthwindTraders.Application.UseCases.Reports.ExportOrdersReport;
using NorthwindTraders.Application.UseCases.Reports.GetDashboardReport;
using NorthwindTraders.Application.UseCases.Reports.GetOrdersReport;

namespace NorthwindTraders.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class ReportsController(
    GetOrdersReportUseCase getOrdersReportUseCase,
    GetDashboardReportUseCase getDashboardReportUseCase,
    ExportOrdersReportUseCase exportOrdersReportUseCase) : ControllerBase
{
    [HttpGet("dashboard")]
    public async Task<IActionResult> GetDashboardAsync(
        [FromQuery] ReportFilterRequest filters,
        CancellationToken cancellationToken)
    {
        var dashboard = await getDashboardReportUseCase.ExecuteAsync(filters, cancellationToken);

        return Ok(dashboard);
    }

    [HttpGet("orders")]
    public async Task<IActionResult> GetOrdersAsync(
        [FromQuery] ReportFilterRequest filters,
        CancellationToken cancellationToken)
    {
        var report = await getOrdersReportUseCase.ExecuteAsync(filters, cancellationToken);

        return Ok(report);
    }

    [HttpGet("orders/export/excel")]
    public async Task<IActionResult> ExportOrdersToExcelAsync(
        [FromQuery] ReportFilterRequest filters,
        CancellationToken cancellationToken)
    {
        var file = await exportOrdersReportUseCase.ExecuteExcelAsync(filters, cancellationToken);

        return File(
            file,
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            "northwind-orders-report.xlsx");
    }

    [HttpGet("orders/export/pdf")]
    public async Task<IActionResult> ExportOrdersToPdfAsync(
        [FromQuery] ReportFilterRequest filters,
        CancellationToken cancellationToken)
    {
        var file = await exportOrdersReportUseCase.ExecutePdfAsync(filters, cancellationToken);

        return File(file, "application/pdf", "northwind-orders-report.pdf");
    }
}
