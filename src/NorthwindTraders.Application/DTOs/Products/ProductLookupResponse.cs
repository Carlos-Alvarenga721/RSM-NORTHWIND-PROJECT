namespace NorthwindTraders.Application.DTOs.Products;

public sealed record ProductLookupResponse(
    int ProductId,
    string ProductName,
    decimal? UnitPrice,
    short? UnitsInStock,
    bool Discontinued,
    int? CategoryId,
    string? CategoryName);
