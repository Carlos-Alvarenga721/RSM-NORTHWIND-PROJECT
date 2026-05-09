using FluentAssertions;
using Moq;
using NorthwindTraders.Application.Abstractions.Persistence;
using NorthwindTraders.Application.Abstractions.Services;
using NorthwindTraders.Application.DTOs.Reports;
using NorthwindTraders.Application.UseCases.Reports.ExportOrdersReport;

namespace NorthwindTraders.UnitTests.Application.Reports;

public sealed class ExportOrdersReportUseCaseTests
{
    [Fact]
    public async Task ExecuteExcelAsync_ShouldLoadReportAndGenerateExcel()
    {
        var filters = new ReportFilterRequest(null, null, null, null);
        var report = new OrdersReportResponse(
            new List<OrdersOverTimePoint>(),
            new List<ShipmentsByRegionPoint>(),
            new List<ReportOrderRow>());
        var file = new byte[] { 7, 8, 9 };
        var reportRepository = new Mock<IReportRepository>();
        reportRepository
            .Setup(repository => repository.GetOrdersReportAsync(filters, It.IsAny<CancellationToken>()))
            .ReturnsAsync(report);
        var exportService = new Mock<IOrdersReportExportService>();
        exportService
            .Setup(service => service.GenerateExcel(report, filters))
            .Returns(file);
        var validator = new ReportFilterRequestValidator();

        var useCase = new ExportOrdersReportUseCase(reportRepository.Object, exportService.Object, validator);

        var result = await useCase.ExecuteExcelAsync(filters, CancellationToken.None);

        result.Should().BeEquivalentTo(file);
        exportService.Verify(service => service.GenerateExcel(report, filters), Times.Once);
    }
}
