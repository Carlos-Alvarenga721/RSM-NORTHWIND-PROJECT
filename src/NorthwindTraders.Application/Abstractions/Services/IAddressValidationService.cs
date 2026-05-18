using NorthwindTraders.Application.DTOs.AddressValidation;

namespace NorthwindTraders.Application.Abstractions.Services;

/// <summary>
/// Boundary for address validation providers; the current implementation calls Google Maps from Infrastructure.
/// </summary>
public interface IAddressValidationService
{
    Task<AddressValidationResponse> ValidateAsync(
        AddressValidationRequest request,
        CancellationToken cancellationToken = default);
}
