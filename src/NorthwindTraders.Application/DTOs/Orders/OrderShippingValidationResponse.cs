namespace NorthwindTraders.Application.DTOs.Orders;

public sealed record OrderShippingValidationResponse(
    string? OriginalAddress,
    string? FormattedAddress,
    double? Latitude,
    double? Longitude,
    string ValidationStatus,
    string? GooglePlaceId,
    string? ValidationMessage,
    string? ValidationGranularity,
    string? GeocodeGranularity,
    DateTime ValidatedAtUtc);
