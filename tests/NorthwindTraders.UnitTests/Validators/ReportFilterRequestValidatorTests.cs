using FluentAssertions;
using NorthwindTraders.Application.DTOs.Reports;

namespace NorthwindTraders.UnitTests.Validators;

public sealed class ReportFilterRequestValidatorTests
{
    [Fact]
    public void Validate_ShouldPassForEmptyFilters()
    {
        var validator = new ReportFilterRequestValidator();
        var request = new ReportFilterRequest(null, null, null, null);

        var result = validator.Validate(request);

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_ShouldFailForInvalidMonthAndWeek()
    {
        var validator = new ReportFilterRequestValidator();
        var request = new ReportFilterRequest(1997, 13, 54, null);

        var result = validator.Validate(request);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(error => error.PropertyName == nameof(ReportFilterRequest.Month));
        result.Errors.Should().Contain(error => error.PropertyName == nameof(ReportFilterRequest.Week));
    }
}
