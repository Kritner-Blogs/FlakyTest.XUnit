using System;
using FlakyTest.XUnit.Guards;
using FlakyTest.XUnit.Interfaces;
using Xunit;
using Xunit.Sdk;

namespace FlakyTest.XUnit.Attributes;

/// <summary>
/// <para>
/// Use of this attribute indicates a maybe fixed test (when it was previously flaky).
/// </para>
/// <para>
/// This attribute can be used to run a test multiple times, only "passing" if each attempt at the test passes, up to
/// the total number of tests desired and specified in the attribute.
/// </para>
/// </summary>
[XunitTestCaseDiscoverer("FlakyTest.XUnit.Services.MaybeFixedFactDiscoverer", "FlakyTest.XUnit")]
[AttributeUsage(AttributeTargets.Method)]
public class MaybeFixedFactAttribute : FactAttribute, IMaybeFixedAttribute
{
    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="retriesBeforeDeemingNoLongerFlaky">The number of retries prior to marking a test as failed.</param>
    public MaybeFixedFactAttribute(
        int retriesBeforeDeemingNoLongerFlaky = IMaybeFixedAttribute.DefaultRetriesBeforeDeemingNoLongerFlaky)
    {
        Guard.AgainstInvalidRetries(retriesBeforeDeemingNoLongerFlaky);

        RetriesBeforeDeemingNoLongerFlaky = retriesBeforeDeemingNoLongerFlaky;
    }

    /// <inheritdoc />
    public int RetriesBeforeDeemingNoLongerFlaky { get; }
}
