using NorthwindTraders.Application.Abstractions.Persistence;
using NorthwindTraders.Application.Abstractions.Services;
using NorthwindTraders.Application.Common.Exceptions;

namespace NorthwindTraders.Application.UseCases.Orders.GenerateOrderPdf;

public sealed class GenerateOrderPdfUseCase(
    IOrderRepository orderRepository,
    IOrderPdfReportService orderPdfReportService)
{
    public async Task<byte[]> ExecuteAsync(int orderId, CancellationToken cancellationToken = default)
    {
        var order = await orderRepository.GetByIdAsync(orderId, cancellationToken)
            ?? throw new NotFoundException($"Order {orderId} was not found.");

        return orderPdfReportService.GenerateOrderDetailsPdf(order);
    }
}
