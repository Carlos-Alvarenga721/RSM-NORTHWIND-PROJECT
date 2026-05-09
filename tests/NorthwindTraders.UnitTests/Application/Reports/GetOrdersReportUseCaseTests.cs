using FluentAssertions;
using Moq;
using NorthwindTraders.Application.Abstractions.Persistence;
using NorthwindTraders.Application.DTOs.Reports;
using NorthwindTraders.Application.UseCases.Reports.GetOrdersReport;

namespace NorthwindTraders.UnitTests.Application.Reports;

public sealed class GetOrdersReportUseCaseTests
{
    [Fact]
    public async Task ExecuteAsync_ShouldValidateAndReturnReport()
    {
        var filters = new ReportFilterRequest(1997, 1, null, "WA");
        var report = new OrdersReportResponse(
            new List<OrdersOverTimePoint>(),
            new List<ShipmentsByRegionPoint>(),
            new List<ReportOrderRow>());
        var reportRepository = new Mock<IReportRepository>();
        reportRepository
            .Setup(repository => repository.GetOrdersReportAsync(filters, It.IsAny<CancellationToken>()))
            .ReturnsAsync(report);
        var validator = new ReportFilterRequestValidator();

        var useCase = new GetOrdersReportUseCase(reportRepository.Object, validator);

        var result = await useCase.ExecuteAsync(filters, CancellationToken.None);

        result.Should().BeSameAs(report);
        reportRepository.Verify(
            repository => repository.GetOrdersReportAsync(filters, It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_ShouldReturnEmptyCollectionsWhenRepositoryReturnsNullCollections()
    {
        var filters = new ReportFilterRequest(null, null, null, null);
        var report = new OrdersReportResponse(null!, null!, null!);
        var reportRepository = new Mock<IReportRepository>();
        reportRepository
            .Setup(repository => repository.GetOrdersReportAsync(filters, It.IsAny<CancellationToken>()))
            .ReturnsAsync(report);
        var validator = new ReportFilterRequestValidator();

        var useCase = new GetOrdersReportUseCase(reportRepository.Object, validator);

        var result = await useCase.ExecuteAsync(filters, CancellationToken.None);

        result.OrdersOverTime.Should().BeEmpty();
        result.ShipmentsByRegion.Should().BeEmpty();
        result.Orders.Should().BeEmpty();
    }
}
