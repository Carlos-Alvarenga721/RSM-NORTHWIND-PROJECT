using System.Globalization;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Nodes;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NorthwindTraders.Application.Abstractions.Services;
using NorthwindTraders.Application.DTOs.AddressValidation;
using NorthwindTraders.Infrastructure.Options;

namespace NorthwindTraders.Infrastructure.Services.AddressValidation;

public sealed class GoogleMapsAddressValidationService(
    HttpClient httpClient,
    IOptions<GoogleMapsOptions> options,
    ILogger<GoogleMapsAddressValidationService> logger) : IAddressValidationService
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

        var googleMapsOptions = options.Value;
        var apiKey = googleMapsOptions.ApiKey;

        logger.LogInformation(
            "Google Maps address validation configured: {IsConfigured}",
            !string.IsNullOrWhiteSpace(apiKey));
        logger.LogInformation(
            "Google Maps address validation base URL: {BaseUrl}",
            googleMapsOptions.AddressValidationBaseUrl);

        if (string.IsNullOrWhiteSpace(apiKey))
        {
            return new AddressValidationResponse(
                originalAddress,
                originalAddress,
                null,
                null,
                ValidationUnavailableStatus,
                null,
                GetValidationMessage(ValidationUnavailableStatus),
                null,
                null);
        }

        HttpResponseMessage response;

        var requestUri = new Uri(
            $"/v1:validateAddress?key={Uri.EscapeDataString(apiKey)}",
            UriKind.Relative);
        var payload = new
        {
            address = new
            {
                regionCode = NormalizeRegionCode(request.Country),
                locality = request.City?.Trim(),
                administrativeArea = request.Region?.Trim(),
                postalCode = request.PostalCode?.Trim(),
                addressLines = new[] { request.AddressLine?.Trim() }
            }
        };

        try
        {
            response = await httpClient.PostAsJsonAsync(
                requestUri,
                payload,
                cancellationToken);
        }
        catch (HttpRequestException exception)
        {
            logger.LogWarning(
                "Google address validation request failed: {ErrorType}.",
                exception.GetType().Name);

            return CreateGoogleFailureResponse(originalAddress, null);
        }
        catch (TaskCanceledException exception) when (!cancellationToken.IsCancellationRequested)
        {
            logger.LogWarning(
                "Google address validation request timed out: {ErrorType}.",
                exception.GetType().Name);

            return CreateGoogleFailureResponse(originalAddress, null);
        }
        catch (Exception exception) when (exception is InvalidOperationException or NotSupportedException)
        {
            logger.LogWarning(
                "Google address validation request could not be sent: {ErrorType}.",
                exception.GetType().Name);

            return CreateGoogleFailureResponse(originalAddress, null);
        }

        if (!response.IsSuccessStatusCode)
        {
            logger.LogWarning(
                "Google address validation returned status {StatusCode} ({ReasonPhrase}).",
                (int)response.StatusCode,
                response.ReasonPhrase);

            return CreateGoogleFailureResponse(originalAddress, response.StatusCode);
        }

        JsonNode? document;

        try
        {
            document = await response.Content.ReadFromJsonAsync<JsonNode>(cancellationToken);
        }
        catch (Exception exception) when (exception is JsonException or NotSupportedException)
        {
            logger.LogWarning(
                "Google address validation response could not be parsed: {ErrorType}.",
                exception.GetType().Name);

            return new AddressValidationResponse(
                originalAddress,
                originalAddress,
                null,
                null,
                ValidationUnavailableStatus,
                null,
                "Google address validation could not be completed. Please try again later.",
                null,
                null);
        }

        if (document is not JsonObject root || root["result"] is not JsonObject result)
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

        var verdict = result["verdict"] as JsonObject;
        var address = result["address"] as JsonObject;
        var geocode = result["geocode"] as JsonObject;
        var location = geocode?["location"] as JsonObject;
        var formattedAddress = GetString(address?["formattedAddress"]) ?? originalAddress;
        var latitude = GetDouble(location?["latitude"]);
        var longitude = GetDouble(location?["longitude"]);
        var placeId = GetString(geocode?["placeId"]);
        var validationGranularity = GetString(verdict?["validationGranularity"]);
        var geocodeGranularity = GetString(verdict?["geocodeGranularity"]);
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
        if (string.IsNullOrWhiteSpace(country))
        {
            return null;
        }

        return country.Trim().ToUpperInvariant();
    }

    private static string? GetString(JsonNode? node)
    {
        return node is JsonValue value && value.TryGetValue<string?>(out var result)
            ? result
            : null;
    }

    private static double? GetDouble(JsonNode? node)
    {
        if (node is not JsonValue value)
        {
            return null;
        }

        if (value.TryGetValue<double>(out var number))
        {
            return number;
        }

        if (value.TryGetValue<string>(out var text) &&
            double.TryParse(text, NumberStyles.Float, CultureInfo.InvariantCulture, out var parsed))
        {
            return parsed;
        }

        return null;
    }

    private static bool GetBoolean(JsonNode? node)
    {
        if (node is not JsonValue value)
        {
            return false;
        }

        if (value.TryGetValue<bool>(out var boolean))
        {
            return boolean;
        }

        return value.TryGetValue<string>(out var text) &&
            bool.TryParse(text, out var parsed) &&
            parsed;
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
            ValidationUnavailableStatus => "Google address validation is not configured.",
            _ => "Address validation completed."
        };
    }

    private static AddressValidationResponse CreateGoogleFailureResponse(
        string originalAddress,
        HttpStatusCode? statusCode)
    {
        var message = statusCode switch
        {
            HttpStatusCode.BadRequest =>
                "Google rejected the address validation request. Please review the shipping country and address fields.",
            HttpStatusCode.Forbidden or HttpStatusCode.Unauthorized =>
                "Google address validation rejected the API key. Check that Address Validation API is enabled, billing is active, and key restrictions allow this service.",
            HttpStatusCode.TooManyRequests =>
                "Google address validation rate limit was reached. Please try again later.",
            _ =>
                "Google address validation could not complete. Please try again later."
        };

        return new AddressValidationResponse(
            originalAddress,
            originalAddress,
            null,
            null,
            ValidationUnavailableStatus,
            null,
            message,
            null,
            null);
    }
}
