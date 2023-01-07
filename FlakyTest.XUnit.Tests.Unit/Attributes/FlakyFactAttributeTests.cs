using FlakyTest.XUnit.Attributes;
using FluentAssertions;
using Xunit;

namespace FlakyTest.XUnit.Tests.Unit.Attributes;

/// <summary>
/// Tests against the <see cref="FlakyFactAttribute"/> class.
/// </summary>
public class FlakyFactAttributeTests
{
    private FlakyFactAttribute _sut;

    private const string FlakyTestExplanation =
        "This test is flaky because of an unstable API... further investigation and fixing should be done in JIRA-1234";
    
    [Fact]
    public void ctor_WhenGivenNoRetryCount_ShouldUseDefault()
    {
        _sut = new FlakyFactAttribute(FlakyTestExplanation);

        _sut.RetriesBeforeFail.Should().Be(FlakyFactAttribute.DefaultRetriesBeforeFail);
    }
    
    [Theory]
    [InlineData(1)]
    [InlineData(7)]
    [InlineData(42)]
    public void ctor_WhenGivenRetryCount_ShouldSetRetryCount(int retryCount)
    {
        _sut = new FlakyFactAttribute(FlakyTestExplanation, retryCount);

        _sut.RetriesBeforeFail.Should().Be(retryCount);
    }
    
    [Theory]
    [InlineData(FlakyTestExplanation)]
    [InlineData("fixed stuff")]
    public void ctor_WhenGivenFlakyReason_ShouldSetFlakyReason(string flakyReason)
    {
        _sut = new FlakyFactAttribute(flakyReason);

        _sut.FlakyExplanation.Should().Be(flakyReason);
    }
}