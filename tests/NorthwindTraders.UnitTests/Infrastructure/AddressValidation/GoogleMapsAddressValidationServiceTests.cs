using System.Net;
using System.Text;
using System.Text.Json;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using NorthwindTraders.Application.DTOs.AddressValidation;
using NorthwindTraders.Infrastructure.Options;
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
    public async Task ValidateAsync_ShouldSendExpectedGoogleRequest()
    {
        var handler = new CapturingHttpMessageHandler("{}", HttpStatusCode.OK);
        var service = CreateService(
            "{}",
            handler: handler);

        await service.ValidateAsync(CreateRequest());

        handler.LastRequestUri.Should().NotBeNull();
        handler.LastRequestUri!.AbsolutePath.Should().Be("/v1:validateAddress");
        handler.LastRequestUri!.Query.Should().Be("?key=test-key");
        handler.LastRequestBody.Should().NotBeNull();

        using var document = JsonDocument.Parse(handler.LastRequestBody!);
        var address = document.RootElement.GetProperty("address");
        address.GetProperty("regionCode").GetString().Should().Be("US");
        address.GetProperty("locality").GetString().Should().Be("Mountain View");
        address.GetProperty("administrativeArea").GetString().Should().Be("CA");
        address.GetProperty("postalCode").GetString().Should().Be("94043");
        address.GetProperty("addressLines")[0].GetString().Should().Be("1600 Amphitheatre Parkway");
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
        result.ValidationMessage.Should().Be("Google address validation is not configured.");
    }

    [Fact]
    public async Task ValidateAsync_ShouldReturnValidationUnavailableWhenGoogleReturnsBadRequest()
    {
        var service = CreateService(
            """
            {
              "error": {
              "code": 400,
              "message": "Invalid address."
              }
            }
            """,
            statusCode: HttpStatusCode.BadRequest);

        var result = await service.ValidateAsync(CreateRequest());

        result.ValidationStatus.Should().Be("ValidationUnavailable");
        result.ValidationMessage.Should().Contain("rejected");
    }

    [Fact]
    public async Task ValidateAsync_ShouldReturnValidationUnavailableWhenGoogleRejectsApiKey()
    {
        var service = CreateService(
            """
            {
              "error": {
                "code": 403,
                "message": "API key not valid."
              }
            }
            """,
            statusCode: HttpStatusCode.Forbidden);

        var result = await service.ValidateAsync(CreateRequest());

        result.ValidationStatus.Should().Be("ValidationUnavailable");
        result.ValidationMessage.Should().Contain("API key");
    }

    private static GoogleMapsAddressValidationService CreateService(
        string responseJson,
        bool includeApiKey = true,
        HttpStatusCode statusCode = HttpStatusCode.OK,
        HttpMessageHandler? handler = null)
    {
        var configurationValues = includeApiKey
            ? new Dictionary<string, string?> { ["GoogleMaps:ApiKey"] = "test-key" }
            : new Dictionary<string, string?>();
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(configurationValues)
            .Build();
        var options = Options.Create(new GoogleMapsOptions
        {
            ApiKey = configuration["GoogleMaps:ApiKey"],
            AddressValidationBaseUrl = GoogleMapsOptions.DefaultAddressValidationBaseUrl
        });
        var httpClient = new HttpClient(handler ?? new StubHttpMessageHandler(responseJson, statusCode))
        {
            BaseAddress = new Uri("https://addressvalidation.googleapis.com/")
        };

        return new GoogleMapsAddressValidationService(
            httpClient,
            options,
            NullLogger<GoogleMapsAddressValidationService>.Instance);
    }

    private static AddressValidationRequest CreateRequest(string addressLine = "1600 Amphitheatre Parkway")
    {
        return new AddressValidationRequest(addressLine, "Mountain View", "CA", "94043", "US");
    }

    private sealed class StubHttpMessageHandler(
        string responseJson,
        HttpStatusCode statusCode) : HttpMessageHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            var response = new HttpResponseMessage(statusCode)
            {
                Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
            };

            return Task.FromResult(response);
        }
    }

    private sealed class CapturingHttpMessageHandler(
        string responseJson,
        HttpStatusCode statusCode) : HttpMessageHandler
    {
        public Uri? LastRequestUri { get; private set; }
        public string? LastRequestBody { get; private set; }

        protected override async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            LastRequestUri = request.RequestUri;

            if (request.Content is not null)
            {
                LastRequestBody = await request.Content.ReadAsStringAsync(cancellationToken);
            }

            return new HttpResponseMessage(statusCode)
            {
                Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
            };
        }
    }
}
