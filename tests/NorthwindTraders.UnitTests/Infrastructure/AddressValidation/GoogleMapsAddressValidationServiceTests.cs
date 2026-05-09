using System.Net;
using System.Text;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using NorthwindTraders.Application.DTOs.AddressValidation;
using NorthwindTraders.Infrastructure.Services.AddressValidation;

namespace NorthwindTraders.UnitTests.Infrastructure.AddressValidation;

public sealed class GoogleMapsAddressValidationServiceTests
{
    [Fact]
    public async Task ValidateAsync_ShouldReturnValidatedForCompletePremiseAddress()
    {
        var service = CreateService(
            """
            {
              "result": {
                "verdict": {
                  "validationGranularity": "PREMISE",
                  "geocodeGranularity": "PREMISE",
                  "addressComplete": true
                },
                "address": {
                  "formattedAddress": "1600 Amphitheatre Parkway, Mountain View, CA 94043-1351, USA"
                },
                "geocode": {
                  "placeId": "test-place",
                  "location": {
                    "latitude": 37.422,
                    "longitude": -122.084
                  }
                }
              }
            }
            """);

        var result = await service.ValidateAsync(CreateRequest());

        result.ValidationStatus.Should().Be("Validated");
        result.Latitude.Should().Be(37.422);
        result.Longitude.Should().Be(-122.084);
        result.ValidationGranularity.Should().Be("PREMISE");
    }

    [Fact]
    public async Task ValidateAsync_ShouldReturnInvalidForIncompleteOrUnconfirmedAddress()
    {
        var service = CreateService(
            """
            {
              "result": {
                "verdict": {
                  "validationGranularity": "OTHER",
                  "geocodeGranularity": "OTHER",
                  "hasUnconfirmedComponents": true
                },
                "address": {
                  "formattedAddress": "Fake St, New York, NY, USA"
                }
              }
            }
            """);

        var result = await service.ValidateAsync(CreateRequest(addressLine: "Fake St"));

        result.ValidationStatus.Should().Be("Invalid");
        result.ValidationMessage.Should().Contain("could not validate");
    }

    [Fact]
    public async Task ValidateAsync_ShouldReturnNeedsReviewWhenGoogleAdjustedComponents()
    {
        var service = CreateService(
            """
            {
              "result": {
                "verdict": {
                  "validationGranularity": "PREMISE",
                  "geocodeGranularity": "PREMISE",
                  "addressComplete": true,
                  "hasReplacedComponents": true
                },
                "address": {
                  "formattedAddress": "1600 Amphitheatre Parkway, Mountain View, CA 94043-1351, USA"
                },
                "geocode": {
                  "location": {
                    "latitude": 37.422,
                    "longitude": -122.084
                  }
                }
              }
            }
            """);

        var result = await service.ValidateAsync(CreateRequest());

        result.ValidationStatus.Should().Be("NeedsReview");
        result.ValidationMessage.Should().Contain("review");
    }

    [Fact]
    public async Task ValidateAsync_ShouldReturnValidationUnavailableWhenApiKeyIsMissing()
    {
        var service = CreateService(
            "{}",
            includeApiKey: false);

        var result = await service.ValidateAsync(CreateRequest());

        result.ValidationStatus.Should().Be("ValidationUnavailable");
    }

    private static GoogleMapsAddressValidationService CreateService(
        string responseJson,
        bool includeApiKey = true)
    {
        var configurationValues = includeApiKey
            ? new Dictionary<string, string?> { ["GoogleMaps:ApiKey"] = "test-key" }
            : new Dictionary<string, string?>();
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(configurationValues)
            .Build();
        var httpClient = new HttpClient(new StubHttpMessageHandler(responseJson))
        {
            BaseAddress = new Uri("https://addressvalidation.googleapis.com/")
        };

        return new GoogleMapsAddressValidationService(httpClient, configuration);
    }

    private static AddressValidationRequest CreateRequest(string addressLine = "1600 Amphitheatre Parkway")
    {
        return new AddressValidationRequest(addressLine, "Mountain View", "CA", "94043", "US");
    }

    private sealed class StubHttpMessageHandler(string responseJson) : HttpMessageHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            var response = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
            };

            return Task.FromResult(response);
        }
    }
}
