using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using NorthwindTraders.Application.Common.Exceptions;

namespace NorthwindTraders.Api.Middleware;

public sealed class ExceptionHandlingMiddleware(
    RequestDelegate next,
    ILogger<ExceptionHandlingMiddleware> logger,
    IWebHostEnvironment environment)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (ValidationException exception)
        {
            await WriteProblemDetailsAsync(
                context,
                StatusCodes.Status400BadRequest,
                "Validation failed.",
                exception.Errors
                    .GroupBy(error => error.PropertyName)
                    .ToDictionary(
                        group => group.Key,
                        group => group.Select(error => error.ErrorMessage).ToArray()));
        }
        catch (NotFoundException exception)
        {
            await WriteProblemDetailsAsync(
                context,
                StatusCodes.Status404NotFound,
                exception.Message);
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "Unhandled API exception.");

            await WriteProblemDetailsAsync(
                context,
                StatusCodes.Status500InternalServerError,
                "An unexpected server error occurred. Please try again or contact support.",
                detail: environment.IsDevelopment() ? exception.Message : null);
        }
    }

    private static async Task WriteProblemDetailsAsync(
        HttpContext context,
        int statusCode,
        string title,
        IDictionary<string, string[]>? errors = null,
        string? detail = null)
    {
        context.Response.StatusCode = statusCode;

        var problemDetails = errors is null
            ? new ProblemDetails
            {
                Detail = detail,
                Status = statusCode,
                Title = title,
            }
            : new ValidationProblemDetails(errors)
            {
                Detail = detail,
                Status = statusCode,
                Title = title,
            };

        await context.Response.WriteAsJsonAsync(problemDetails);
    }
}
