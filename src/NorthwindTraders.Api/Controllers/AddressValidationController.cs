using System.Linq;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using NorthwindTraders.Application.DTOs.AddressValidation;
using NorthwindTraders.Application.UseCases.AddressValidation.ValidateAddress;

namespace NorthwindTraders.Api.Controllers;

/// <summary>
/// Validates shipping addresses before an order is saved so the frontend can show status and map data.
/// </summary>
[ApiController]
[Route("api/address-validation")]
public sealed class AddressValidationController(
    ValidateAddressUseCase validateAddressUseCase,
    IValidator<AddressValidationRequest> validator) : ControllerBase
{
    [HttpPost("validate")]
    public async Task<IActionResult> ValidateAsync(
        AddressValidationRequest request,
        CancellationToken cancellationToken)
    {
        var validationResult = await validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            // Return field-level validation details in the same shape the frontend error handler expects.
            var errors = validationResult.Errors
                .GroupBy(error => error.PropertyName)
                .ToDictionary(
                    group => group.Key,
                    group => group.Select(error => error.ErrorMessage).ToArray());

            return ValidationProblem(new ValidationProblemDetails(errors));
        }

        var result = await validateAddressUseCase.ExecuteAsync(request, cancellationToken);

        return Ok(result);
    }
}
