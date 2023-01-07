using FlakyTest.XUnit.Guards;
using Xunit;

namespace FlakyTest.XUnit.Attributes
{
    /// <summary>
    /// <para>
    /// Use of this attribute indicates a flaky test.
    /// </para>
    /// <para>
    /// This attribute should be used sparingly, but it can be used to mark a test as "flaky", which will cause
    /// the test runner to attempt to run the test until either:
    /// <list type="bullet">
    /// <item>The test passes</item>
    /// <item>The test fails the number of times as specified by the <see cref="RetriesBeforeFail"/></item>
    /// </list>
    /// </para>
    /// </summary>
    public class FlakyFactAttribute : FactAttribute
    {
        public const int DefaultRetriesBeforeFail = 5;

        public readonly string FlakyExplanation;
        public readonly int RetriesBeforeFail;

        public FlakyFactAttribute(string flakyExplanation, int retriesBeforeFail = DefaultRetriesBeforeFail)
        {
            Guard.AgainstNotProvidedFlakyExplanation(flakyExplanation);
            Guard.AgainstInvalidRetries(retriesBeforeFail);

            FlakyExplanation = flakyExplanation;
            RetriesBeforeFail = retriesBeforeFail;
        }
    }
}
