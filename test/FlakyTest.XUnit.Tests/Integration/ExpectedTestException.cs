namespace FlakyTest.XUnit.Tests.Integration;

/// <summary>
/// Just an expected exception used in testing
/// </summary>
internal sealed class ExpectedTestException : Exception
{
    internal static void ThrowException() => throw new ExpectedTestException();
}
