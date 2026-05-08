namespace NorthwindTraders.Application.DTOs.Orders;

public sealed record OrderDetailResponse(
    int ProductId,
    string ProductName,
    decimal UnitPrice,
    short Quantity,
    float Discount,
    decimal LineTotal);
