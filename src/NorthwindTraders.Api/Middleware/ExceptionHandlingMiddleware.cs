using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using NorthwindTraders.Application.Common.Exceptions;

namespace NorthwindTraders.Api.Middleware;

public sealed class ExceptionHandlingMiddleware(RequestDelegate next)
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
    }

    private static async Task WriteProblemDetailsAsync(
        HttpContext context,
        int statusCode,
        string title,
        IDictionary<string, string[]>? errors = null)
    {
        context.Response.StatusCode = statusCode;

        var problemDetails = errors is null
            ? new ProblemDetails
            {
                Status = statusCode,
                Title = title
            }
            : new ValidationProblemDetails(errors)
            {
                Status = statusCode,
                Title = title
            };

        await context.Response.WriteAsJsonAsync(problemDetails);
    }
}
