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
                "Invalid",
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
                "ValidationUnavailable",
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
        var formattedAddress = result?["address"]?["formattedAddress"]?.GetValue<string>() ?? originalAddress;
        var latitude = GetDouble(result?["geocode"]?["location"]?["latitude"]);
        var longitude = GetDouble(result?["geocode"]?["location"]?["longitude"]);
        var placeId = result?["geocode"]?["placeId"]?.GetValue<string>();
        var validationGranularity = result?["verdict"]?["validationGranularity"]?.GetValue<string>();
        var status = latitude.HasValue && longitude.HasValue
            ? "Validated"
            : validationGranularity ?? "ValidatedWithoutCoordinates";

        return new AddressValidationResponse(
            originalAddress,
            formattedAddress,
            latitude,
            longitude,
            status,
            placeId);
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
}
