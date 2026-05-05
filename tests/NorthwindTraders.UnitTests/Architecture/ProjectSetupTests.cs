using FluentAssertions;

namespace NorthwindTraders.UnitTests.Architecture;

public class ProjectSetupTests
{
    [Fact]
    public void ProjectSetup_ShouldBeValid()
    {
        true.Should().BeTrue();
    }
}
