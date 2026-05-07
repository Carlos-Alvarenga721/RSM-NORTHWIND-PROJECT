namespace NorthwindTraders.Application.DTOs.Customers;

public sealed record CustomerLookupDto(
    string CustomerId,
    string CompanyName,
    string? ContactName,
    string? City,
    string? Region,
    string? Country);
