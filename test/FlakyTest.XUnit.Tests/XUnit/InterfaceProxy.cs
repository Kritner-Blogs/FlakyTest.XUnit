namespace FlakyTest.XUnit.Tests.XUnit;

public class InterfaceProxy<T>
{
    public override string ToString() { return typeof(T).Name; }
}
