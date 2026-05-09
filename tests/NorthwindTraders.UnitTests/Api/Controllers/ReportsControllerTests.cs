using FluentAssertions;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NorthwindTraders.Api.Controllers;
using NorthwindTraders.Application.Abstractions.Persistence;
using NorthwindTraders.Application.Abstractions.Services;
using NorthwindTraders.Application.DTOs.Reports;
using NorthwindTraders.Application.UseCases.Reports.ExportOrdersReport;
using NorthwindTraders.Application.UseCases.Reports.GetDashboardReport;
using NorthwindTraders.Application.UseCases.Reports.GetOrdersReport;

namespace NorthwindTraders.UnitTests.Api.Controllers;

public sealed class ReportsControllerTests
{
    [Fact]
    public async Task GetOrdersAsync_ShouldReturnOkWithReport()
    {
        var filters = new ReportFilterRequest(1997, null, null, null);
        var report = CreateReport();
        var reportRepository = new Mock<IReportRepository>();
        reportRepository
            .Setup(repository => repository.GetOrdersReportAsync(filters, It.IsAny<CancellationToken>()))
            .ReturnsAsync(report);

        var controller = CreateController(reportRepository.Object);

        var result = await controller.GetOrdersAsync(filters, CancellationToken.None);

        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        okResult.Value.Should().BeEquivalentTo(report);
    }

    [Fact]
    public async Task ExportOrdersToExcelAsync_ShouldReturnExcelFile()
    {
        var filters = new ReportFilterRequest(null, null, null, null);
        var report = CreateReport();
        var file = new byte[] { 1, 2, 3 };
        var reportRepository = new Mock<IReportRepository>();
        reportRepository
            .Setup(repository => repository.GetOrdersReportAsync(filters, It.IsAny<CancellationToken>()))
            .ReturnsAsync(report);
        var exportService = new Mock<IOrdersReportExportService>();
        exportService
            .Setup(service => service.GenerateExcel(report, filters))
            .Returns(file);

        var controller = CreateController(reportRepository.Object, exportService.Object);

        var result = await controller.ExportOrdersToExcelAsync(filters, CancellationToken.None);

        var fileResult = result.Should().BeOfType<FileContentResult>().Subject;
        fileResult.FileContents.Should().BeEquivalentTo(file);
        fileResult.ContentType.Should().Be("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
        fileResult.FileDownloadName.Should().Be("northwind-orders-report.xlsx");
    }

    private static ReportsController CreateController(
        IReportRepository reportRepository,
        IOrdersReportExportService? exportService = null)
    {
        var validator = new InlineValidator<ReportFilterRequest>();
        var exportServiceMock = exportService ?? Mock.Of<IOrdersReportExportService>();

        return new ReportsController(
            new GetOrdersReportUseCase(reportRepository, validator),
            new GetDashboardReportUseCase(reportRepository, validator),
            new ExportOrdersReportUseCase(reportRepository, exportServiceMock, validator));
    }

    private static OrdersReportResponse CreateReport()
    {
        return new OrdersReportResponse(
            new List<OrdersOverTimePoint>
            {
                new("Jan 1997", 1, 47.38m)
            },
            new List<ShipmentsByRegionPoint>
            {
                new("WA", 1)
            },
            new List<ReportOrderRow>
            {
                new(10248, "Vins et alcools Chevalier", "Steven Buchanan", DateTime.Today, null, "WA", "USA", 47.38m)
            });
    }
}
