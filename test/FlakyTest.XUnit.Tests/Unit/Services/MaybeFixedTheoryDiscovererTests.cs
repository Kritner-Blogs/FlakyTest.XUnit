using FlakyTest.XUnit.Models;
using FlakyTest.XUnit.Services;
using FluentAssertions;
using Moq;
using Xunit;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace FlakyTest.XUnit.Tests.Unit.Services;

public class MaybeFixedTheoryDiscovererTests
{
    private readonly TestableMaybeFixedTheoryDiscoverer _sut = new(new Mock<IMessageSink>().Object);
    private class TestableMaybeFixedTheoryDiscoverer : MaybeFixedTheoryDiscoverer
    {
        public TestableMaybeFixedTheoryDiscoverer(IMessageSink diagnosticMessageSink) : base(diagnosticMessageSink)
        {
        }

        public IEnumerable<IXunitTestCase> TestableCreateTestCasesForDataRow(
            ITestFrameworkDiscoveryOptions discoveryOptions, ITestMethod testMethod, IAttributeInfo theoryAttribute,
            object[] dataRow) => base.CreateTestCasesForDataRow(discoveryOptions, testMethod, theoryAttribute, dataRow);
    }


    [Fact]
    public void WhenDiscover_ShouldReturnSingleTestCase()
    {
        var result = _sut.TestableCreateTestCasesForDataRow(
            new Mock<ITestFrameworkDiscoveryOptions>().Object,
            new Mock<ITestMethod>().Object,
            new Mock<IAttributeInfo>().Object,
            new object[] { })
            .ToList();

        result.Should().HaveCount(1);
        result[0].Should().BeOfType<MaybeFixedTestCase>();
    }
}
