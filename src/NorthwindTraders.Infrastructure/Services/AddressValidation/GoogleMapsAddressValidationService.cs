using System.Net.Http.Json;
using System.Text.Json.Nodes;
using Microsoft.Extensions.Configuration;
using NorthwindTraders.Application.Abstractions.Services;
using NorthwindTraders.Application.DTOs.AddressValidation;

namespace NorthwindTraders.Infrastructure.Services.AddressValidation;

public sealed class GoogleMapsAddressValidationService(
    HttpClient httpClient,
    IConfiguration configuration) : IAddressValidationService
{
    private const string ValidatedStatus = "Validated";
    private const string NeedsReviewStatus = "NeedsReview";
    private const string InvalidStatus = "Invalid";
    private const string ValidationUnavailableStatus = "ValidationUnavailable";

    public async Task<AddressValidationResponse> ValidateAsync(
        AddressValidationRequest request,
        CancellationToken cancellationToken = default)
    {
        var originalAddress = BuildOriginalAddress(request);

        if (string.IsNullOrWhiteSpace(originalAddress))
        {
            return new AddressValidationResponse(
                originalAddress,
                null,
                null,
                null,
                InvalidStatus,
                null,
                "Enter a shipping address before validation.",
                null,
                null);
        }

        var apiKey = configuration["GoogleMaps:ApiKey"];

        if (string.IsNullOrWhiteSpace(apiKey))
        {
            return new AddressValidationResponse(
                originalAddress,
                originalAddress,
                null,
                null,
                ValidationUnavailableStatus,
                null,
                "Google address validation is not configured. The address was accepted for local development.",
                null,
                null);
        }

        var response = await httpClient.PostAsJsonAsync(
            $"v1:validateAddress?key={Uri.EscapeDataString(apiKey)}",
            new
            {
                address = new
                {
                    regionCode = NormalizeRegionCode(request.Country),
                    locality = request.City,
                    administrativeArea = request.Region,
                    postalCode = request.PostalCode,
                    addressLines = new[] { originalAddress }
                }
            },
            cancellationToken);

        response.EnsureSuccessStatusCode();

        var document = await response.Content.ReadFromJsonAsync<JsonNode>(cancellationToken);
        var result = document?["result"];
        if (result is null)
        {
            return new AddressValidationResponse(
                originalAddress,
                null,
                null,
                null,
                InvalidStatus,
                null,
                "Google could not validate this address. Please review the shipping information.",
                null,
                null);
        }

        var verdict = result["verdict"];
        var formattedAddress = result["address"]?["formattedAddress"]?.GetValue<string>() ?? originalAddress;
        var latitude = GetDouble(result["geocode"]?["location"]?["latitude"]);
        var longitude = GetDouble(result["geocode"]?["location"]?["longitude"]);
        var placeId = result["geocode"]?["placeId"]?.GetValue<string>();
        var validationGranularity = verdict?["validationGranularity"]?.GetValue<string>();
        var geocodeGranularity = verdict?["geocodeGranularity"]?.GetValue<string>();
        var addressComplete = GetBoolean(verdict?["addressComplete"]);
        var hasUnconfirmedComponents = GetBoolean(verdict?["hasUnconfirmedComponents"]);
        var hasReplacedComponents = GetBoolean(verdict?["hasReplacedComponents"]);
        var hasInferredComponents = GetBoolean(verdict?["hasInferredComponents"]);
        var hasSpellCorrectedComponents = GetBoolean(verdict?["hasSpellCorrectedComponents"]);
        var hasPremiseLevelValidation = IsPremiseLevel(validationGranularity);
        var hasCoordinates = latitude.HasValue && longitude.HasValue;
        var status = GetValidationStatus(
            addressComplete,
            hasPremiseLevelValidation,
            hasCoordinates,
            hasUnconfirmedComponents,
            hasReplacedComponents,
            hasInferredComponents,
            hasSpellCorrectedComponents);

        return new AddressValidationResponse(
            originalAddress,
            formattedAddress,
            latitude,
            longitude,
            status,
            placeId,
            GetValidationMessage(status),
            validationGranularity,
            geocodeGranularity);
    }

    private static string BuildOriginalAddress(AddressValidationRequest request)
    {
        return string.Join(", ", new[]
        {
            request.AddressLine,
            request.City,
            request.Region,
            request.PostalCode,
            request.Country
        }.Where(value => !string.IsNullOrWhiteSpace(value)));
    }

    private static string? NormalizeRegionCode(string? country)
    {
        return country?.Trim().Length == 2
            ? country.Trim().ToUpperInvariant()
            : null;
    }

    private static double? GetDouble(JsonNode? node)
    {
        return node is null ? null : node.GetValue<double>();
    }

    private static bool GetBoolean(JsonNode? node)
    {
        return node is not null && node.GetValue<bool>();
    }

    private static bool IsPremiseLevel(string? granularity)
    {
        return string.Equals(granularity, "PREMISE", StringComparison.OrdinalIgnoreCase) ||
            string.Equals(granularity, "SUB_PREMISE", StringComparison.OrdinalIgnoreCase);
    }

    private static string GetValidationStatus(
        bool addressComplete,
        bool hasPremiseLevelValidation,
        bool hasCoordinates,
        bool hasUnconfirmedComponents,
        bool hasReplacedComponents,
        bool hasInferredComponents,
        bool hasSpellCorrectedComponents)
    {
        if (!addressComplete || !hasPremiseLevelValidation || hasUnconfirmedComponents)
        {
            return InvalidStatus;
        }

        if (!hasCoordinates || hasReplacedComponents || hasInferredComponents || hasSpellCorrectedComponents)
        {
            return NeedsReviewStatus;
        }

        return ValidatedStatus;
    }

    private static string GetValidationMessage(string status)
    {
        return status switch
        {
            ValidatedStatus => "Google validated this address at premise level.",
            NeedsReviewStatus => "Google found the address but adjusted or inferred part of it. Please review the formatted address before saving.",
            InvalidStatus => "Google could not validate this as a complete deliverable address. Please review the shipping information.",
            ValidationUnavailableStatus => "Google address validation is not configured. The address was accepted for local development.",
            _ => "Address validation completed."
        };
    }
}
