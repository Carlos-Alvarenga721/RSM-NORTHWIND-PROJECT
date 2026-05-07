namespace NorthwindTraders.Application.DTOs.Products;

public sealed record ProductLookupDto(
    int ProductId,
    string ProductName,
    decimal? UnitPrice,
    short? UnitsInStock,
    bool Discontinued);
