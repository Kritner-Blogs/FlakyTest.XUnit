using System;
using FlakyTest.XUnit.Guards;
using Xunit;
using Xunit.Sdk;

namespace FlakyTest.XUnit.Attributes;

/// <summary>
/// <para>
/// Use of this attribute indicates a flaky test.
/// </para>
/// <para>
/// This attribute should be used sparingly, but it can be used to mark a <see cref="FactAttribute"/> test as "flaky",
/// which will cause the test runner to attempt to run the test until either:
/// <list type="bullet">
/// <item>The test passes</item>
/// <item>The test fails the number of times as specified by the <see cref="RetriesBeforeFail"/></item>
/// </list>
/// If the test fails up to the maximum retries, it reports as failure.
/// </para>
/// </summary>
[XunitTestCaseDiscoverer("FlakyTest.XUnit.Services.FlakyFactDiscoverer", "FlakyTest.XUnit")]
[AttributeUsage(AttributeTargets.Method)]
public class FlakyFactAttribute : FactAttribute
{
    /// <summary>
    /// The default number of retries before failing a test case.
    /// </summary>
    public const int DefaultRetriesBeforeFail = 5;

    /// <summary>
    /// The explanation as to why the test case is being annotated as flaky.
    /// </summary>
    public readonly string FlakyExplanation;

    /// <summary>
    /// The number of attempts to retry a test case before deeming it a failed test. 
    /// </summary>
    public readonly int RetriesBeforeFail;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="flakyExplanation">The explanation to why the test is being marked flaky.</param>
    /// <param name="retriesBeforeFail">The number of retries prior to marking a test as failed.</param>
    public FlakyFactAttribute(string flakyExplanation, int retriesBeforeFail = DefaultRetriesBeforeFail)
    {
        Guard.AgainstNotProvidedFlakyExplanation(flakyExplanation);
        Guard.AgainstInvalidRetries(retriesBeforeFail);

        FlakyExplanation = flakyExplanation;
        RetriesBeforeFail = retriesBeforeFail;
    }
}
