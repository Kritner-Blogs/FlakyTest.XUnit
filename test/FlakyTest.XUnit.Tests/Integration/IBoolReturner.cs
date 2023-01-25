namespace FlakyTest.XUnit.Tests.Integration;

/// <summary>
/// Used to return booleans in a specific order for tests
/// </summary>
public interface IBoolReturner
{
    Task<bool> Get();
}
