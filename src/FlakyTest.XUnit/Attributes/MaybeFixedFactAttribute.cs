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
    /// <param name="retriesBeforeDeemingNoLongerFlaky">
    /// The number of times to run the test.  Each attempt must pass for the test to be overall considered a 
    /// "passing" (or no longer flaky) test.
    /// </param>
    public MaybeFixedFactAttribute(
        int retriesBeforeDeemingNoLongerFlaky = IMaybeFixedAttribute.DefaultRetriesBeforeDeemingNoLongerFlaky)
    {
        Guard.AgainstInvalidRetries(retriesBeforeDeemingNoLongerFlaky);

        RetriesBeforeDeemingNoLongerFlaky = retriesBeforeDeemingNoLongerFlaky;
    }

    /// <inheritdoc />
    public int RetriesBeforeDeemingNoLongerFlaky { get; }
}
