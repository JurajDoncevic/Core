namespace Core.Tests;

public class UnitTypeTests
{
    [Fact]
    public void CreateUnit()
    {
        var unit = Unit();
        Assert.Equal(default(Unit), unit);
    }

    [Fact]
    public void IgnoreValue_WithUnit()
    {
        var unit = new object().Ignore();
        Assert.Equal(default(Unit), unit);
    }
}
