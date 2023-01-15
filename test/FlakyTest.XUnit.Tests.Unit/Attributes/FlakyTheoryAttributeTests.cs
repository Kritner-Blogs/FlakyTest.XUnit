using FlakyTest.XUnit.Attributes;
using FlakyTest.XUnit.Interfaces;
using FluentAssertions;
using Xunit;

namespace FlakyTest.XUnit.Tests.Unit.Attributes;

/// <summary>
/// Tests against the <see cref="FlakyTheoryAttribute"/> class.
/// </summary>
public class FlakyTheoryAttributeTests
{
    private FlakyTheoryAttribute? _sut;

    private const string FlakyTestExplanation =
        "This test is flaky because of an unstable API... further investigation and fixing should be done in JIRA-1234";

    [Fact]
    public void ctor_WhenGivenNoRetryCount_ShouldUseDefault()
    {
        _sut = new FlakyTheoryAttribute(FlakyTestExplanation);

        _sut.RetriesBeforeFail.Should().Be(IFlakyAttribute.DefaultRetriesBeforeFail);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(7)]
    [InlineData(42)]
    public void ctor_WhenGivenRetryCount_ShouldSetRetryCount(int retryCount)
    {
        _sut = new FlakyTheoryAttribute(FlakyTestExplanation, retryCount);

        _sut.RetriesBeforeFail.Should().Be(retryCount);
    }

    [Theory]
    [InlineData(FlakyTestExplanation)]
    [InlineData("fixed stuff")]
    public void ctor_WhenGivenFlakyReason_ShouldSetFlakyReason(string flakyReason)
    {
        _sut = new FlakyTheoryAttribute(flakyReason);

        _sut.FlakyExplanation.Should().Be(flakyReason);
    }
}
