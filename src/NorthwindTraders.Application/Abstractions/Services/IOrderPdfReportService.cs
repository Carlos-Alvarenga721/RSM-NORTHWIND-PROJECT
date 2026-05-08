using NorthwindTraders.Application.DTOs.Orders;

namespace NorthwindTraders.Application.Abstractions.Services;

public interface IOrderPdfReportService
{
    byte[] GenerateOrderDetailsPdf(OrderResponse order);
}
