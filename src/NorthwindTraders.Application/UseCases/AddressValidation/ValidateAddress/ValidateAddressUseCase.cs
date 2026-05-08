using NorthwindTraders.Application.Abstractions.Services;
using NorthwindTraders.Application.DTOs.AddressValidation;

namespace NorthwindTraders.Application.UseCases.AddressValidation.ValidateAddress;

public sealed class ValidateAddressUseCase(IAddressValidationService addressValidationService)
{
    public Task<AddressValidationResponse> ExecuteAsync(
        AddressValidationRequest request,
        CancellationToken cancellationToken = default)
    {
        return addressValidationService.ValidateAsync(request, cancellationToken);
    }
}
