using Microsoft.AspNetCore.Mvc;
using NorthwindTraders.Application.DTOs.AddressValidation;
using NorthwindTraders.Application.UseCases.AddressValidation.ValidateAddress;

namespace NorthwindTraders.Api.Controllers;

[ApiController]
[Route("api/address-validation")]
public sealed class AddressValidationController(
    ValidateAddressUseCase validateAddressUseCase) : ControllerBase
{
    [HttpPost("validate")]
    public async Task<IActionResult> ValidateAsync(
        AddressValidationRequest request,
        CancellationToken cancellationToken)
    {
        var result = await validateAddressUseCase.ExecuteAsync(request, cancellationToken);

        return Ok(result);
    }
}
