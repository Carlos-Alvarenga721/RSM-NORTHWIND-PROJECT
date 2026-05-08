namespace NorthwindTraders.Application.DTOs.Orders;

public sealed record OrderDetailRequest(
    int ProductId,
    decimal UnitPrice,
    short Quantity,
    float Discount);
