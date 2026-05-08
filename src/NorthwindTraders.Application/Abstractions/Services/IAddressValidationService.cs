using NorthwindTraders.Application.DTOs.AddressValidation;

namespace NorthwindTraders.Application.Abstractions.Services;

public interface IAddressValidationService
{
    Task<AddressValidationResponse> ValidateAsync(
        AddressValidationRequest request,
        CancellationToken cancellationToken = default);
}
