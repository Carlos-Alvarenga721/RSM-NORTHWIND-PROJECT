using FluentValidation;

namespace NorthwindTraders.Application.DTOs.Reports;

public sealed class ReportFilterRequestValidator : AbstractValidator<ReportFilterRequest>
{
    public ReportFilterRequestValidator()
    {
        RuleFor(filter => filter.Year)
            .InclusiveBetween(1900, 2100)
            .When(filter => filter.Year.HasValue);

        RuleFor(filter => filter.Month)
            .InclusiveBetween(1, 12)
            .When(filter => filter.Month.HasValue);

        RuleFor(filter => filter.Week)
            .InclusiveBetween(1, 53)
            .When(filter => filter.Week.HasValue);

        RuleFor(filter => filter.Region)
            .MaximumLength(50);
    }
}
