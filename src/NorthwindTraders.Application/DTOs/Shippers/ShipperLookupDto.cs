namespace NorthwindTraders.Application.DTOs.Shippers;

public sealed record ShipperLookupDto(
    int ShipperId,
    string CompanyName,
    string? Phone);
