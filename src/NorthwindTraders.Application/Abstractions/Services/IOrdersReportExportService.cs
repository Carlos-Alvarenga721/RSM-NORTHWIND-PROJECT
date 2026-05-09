using NorthwindTraders.Application.DTOs.Reports;

namespace NorthwindTraders.Application.Abstractions.Services;

public interface IOrdersReportExportService
{
    byte[] GenerateExcel(OrdersReportResponse report, ReportFilterRequest filters);

    byte[] GeneratePdf(OrdersReportResponse report, ReportFilterRequest filters);
}
