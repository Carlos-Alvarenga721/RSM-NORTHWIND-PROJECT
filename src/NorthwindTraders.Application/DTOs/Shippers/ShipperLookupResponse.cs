namespace NorthwindTraders.Application.DTOs.Shippers;

public sealed record ShipperLookupResponse(
    int ShipperId,
    string CompanyName,
    string? Phone);
