namespace NorthwindTraders.Application.DTOs.Customers;

public sealed record CustomerLookupResponse(
    string CustomerId,
    string CompanyName,
    string? ContactName,
    string? City,
    string? Country);
