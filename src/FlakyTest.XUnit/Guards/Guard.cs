using FlakyTest.XUnit.Exceptions;

namespace FlakyTest.XUnit.Guards;

/// <summary>
/// Helper guard classes against invalid use.
/// </summary>
public static class Guard
{
    /// <summary>
    /// A guard that protects against an empty or null flakyException.
    /// </summary>
    /// <param name="flakyExplanation">The string flaky explanation to evaluate.</param>
    /// <exception cref="FlakyExplanationException">Thrown when the explanation is null or empty.</exception>
    public static void AgainstNotProvidedFlakyExplanation(string flakyExplanation)
    {
        if (string.IsNullOrEmpty(flakyExplanation))
            throw new FlakyExplanationException();
    }

    /// <summary>
    /// A guard that protects against invalid number of retries on a flaky test.
    /// </summary>
    /// <param name="retriesBeforeFail">The retries amount to validate.</param>
    /// <exception cref="InvalidNumberOfRetriesException">
    /// Thrown when the number of retries is less than or equal to zero.
    /// </exception>
    public static void AgainstInvalidRetries(int retriesBeforeFail)
    {
        if (retriesBeforeFail <= 0)
            throw new InvalidNumberOfRetriesException();
    }
}
