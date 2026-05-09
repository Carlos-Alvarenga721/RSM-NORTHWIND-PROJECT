using FluentValidation;
using NorthwindTraders.Application.Abstractions.Services;
using NorthwindTraders.Application.DTOs.AddressValidation;

namespace NorthwindTraders.Application.UseCases.AddressValidation.ValidateAddress;

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

        return await addressValidationService.ValidateAsync(request, cancellationToken);
    }
}
