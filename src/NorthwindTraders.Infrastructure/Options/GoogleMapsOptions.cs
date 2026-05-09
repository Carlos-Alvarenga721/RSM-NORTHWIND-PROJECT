namespace NorthwindTraders.Infrastructure.Options;

public sealed class GoogleMapsOptions
{
    public const string SectionName = "GoogleMaps";
    public const string DefaultAddressValidationBaseUrl = "https://addressvalidation.googleapis.com";

    public string? ApiKey { get; set; }
    public string AddressValidationBaseUrl { get; set; } = DefaultAddressValidationBaseUrl;
}
