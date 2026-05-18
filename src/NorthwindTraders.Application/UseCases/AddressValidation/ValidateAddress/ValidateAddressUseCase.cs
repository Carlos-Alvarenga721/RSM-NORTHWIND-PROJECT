using FluentValidation;
using NorthwindTraders.Application.Abstractions.Services;
using NorthwindTraders.Application.DTOs.AddressValidation;

namespace NorthwindTraders.Application.UseCases.AddressValidation.ValidateAddress;

/// <summary>
/// Validates the address request before delegating the external Google Maps call to Infrastructure.
/// </summary>
public sealed class ValidateAddressUseCase(
    IAddressValidationService addressValidationService,
    IValidator<AddressValidationRequest> validator)
{
    public Task<AddressValidationResponse> ExecuteAsync(
        AddressValidationRequest request,
        CancellationToken cancellationToken = default)
    {
        return ExecuteInternalAsync(request, cancellationToken);
    }

    private async Task<AddressValidationResponse> ExecuteInternalAsync(
        AddressValidationRequest request,
        CancellationToken cancellationToken)
    {
        await validator.ValidateAndThrowAsync(request, cancellationToken);

        // The application layer only depends on the service contract, keeping HTTP details out of this use case.
        return await addressValidationService.ValidateAsync(request, cancellationToken);
    }
}
