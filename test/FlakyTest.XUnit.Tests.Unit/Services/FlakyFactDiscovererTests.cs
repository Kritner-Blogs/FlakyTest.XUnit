using FlakyTest.XUnit.Models;
using FlakyTest.XUnit.Services;
using FluentAssertions;
using Moq;
using Xunit;
using Xunit.Abstractions;

namespace FlakyTest.XUnit.Tests.Unit.Services;

public class FlakyFactDiscovererTests
{
    private readonly FlakyFactDiscoverer _sut = new(new Mock<IMessageSink>().Object);

    [Fact]
    public void WhenDiscover_ShouldReturnSingleTestCase()
    {
        var result = _sut.Discover(
            new Mock<ITestFrameworkDiscoveryOptions>().Object, 
            new Mock<ITestMethod>().Object, new Mock<IAttributeInfo>().Object)
            .ToList();

        result.Should().HaveCount(1);
        result[0].Should().BeOfType<FlakyTestCase>();
    }
}
