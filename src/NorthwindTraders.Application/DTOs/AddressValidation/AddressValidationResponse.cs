namespace NorthwindTraders.Application.DTOs.AddressValidation;

public sealed record AddressValidationResponse(
    string? OriginalAddress,
    string? FormattedAddress,
    double? Latitude,
    double? Longitude,
    string ValidationStatus,
    string? GooglePlaceId,
    string? ValidationMessage,
    string? ValidationGranularity,
    string? GeocodeGranularity);
