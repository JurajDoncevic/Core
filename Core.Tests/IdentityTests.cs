namespace Core.Tests;

public class IdentityTests
{
    [Fact]
    public void CreateIdentity_WithValueTypes()
    {
        var identity1 = 2.ToIdentity();
        var identity2 = (X: 2, Y: 3).ToIdentity();

        Assert.Equal(2, identity1.Data);
        Assert.Equal((X: 2, Y: 3), identity2.Data);
    }

    [Fact]
    public void MapIdentity_WithValueType()
    {
        var valueTuple = (X: 2, Y: 3, Z: 4);

        var result =
            valueTuple.ToIdentity()
                      .Map(_ => (_.X, _.Y))
                      .Map(_ => _.X + 1)
                      .Data;

        Assert.Equal(3, result);
    }

    [Fact]
    public void MapIdentity_WithReferenceType()
    {
        var obj = new { X = 2, Y = 3 };

        var result =
            obj.ToIdentity()
               .Map(_ => new { X = _.X + 1, Y = _.Y })
               .Data;

        Assert.Equal(3, result.X);
        Assert.Equal(2, obj.X);
    }
}
