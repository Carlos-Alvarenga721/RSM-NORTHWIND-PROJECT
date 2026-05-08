namespace NorthwindTraders.Application.DTOs.AddressValidation;

public sealed record AddressValidationRequest(
    string? AddressLine,
    string? City,
    string? Region,
    string? PostalCode,
    string? Country);
