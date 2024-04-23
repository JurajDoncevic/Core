namespace Core.Tests;

public class OptionTests
{
    [Fact]
    public void CreateSome_WithObject()
    {
        var option = Option.Some("Hello");
        Assert.True(option.IsSome);
        Assert.Equal("Hello", option.Value);
    }

    [Fact]
    public void CreateSome_WithValue()
    {
        int x = 42;
        var option = Option.Some(x);
        Assert.True(option.IsSome);
        Assert.Equal(x, option.Value);
    }


    [Fact]
    public void CreateNone_WithValue()
    {
        var option = Option.None<int>();
        Assert.False(option.IsSome);
    }

    [Fact]
    public void CreateNone_WithObject()
    {
        var option = Option.None<string>();
        Assert.False(option.IsSome);
    }

    [Fact]
    public void CreateSome_WithNull_AsNone()
    {
        var option = Option.Some<string>(null!);
        Assert.False(option.IsSome);
        Assert.Null(option.Value);
    }

    [Fact]
    public void MapSome_WithObject()
    {
        var x = "Hello";
        var option = Option.Some(x);
        var mapped = option.Map(s => s.Length);
        Assert.True(mapped.IsSome);
        Assert.Equal(x.Length, mapped.Value);
    }

    [Fact]
    public void MapSome_ToNone_WithNullObject()
    {
        var x = "Hello";
        var option = Option.Some(x);
        var mapped = option
            .Map(s => s + "\n")
            .Map(i => (string)null!);
        Assert.False(mapped.IsSome);
    }

    [Fact]
    public void BindSome_WithObject()
    {
        var x = "Hello";
        var option = Option.Some(x);
        var binded = option
            .Bind(s => Option.Some(s + " "))
            .Bind(s => Option.Some(s + "World"));

        Assert.True(binded.IsSome);
        Assert.Equal("Hello World", binded.Value);
    }

    [Fact]
    public void BindSome_WithNone()
    {
        var x = "Hello";
        var option = Option.Some(x);
        var binded = option
            .Bind(s => Option.None<string>())
            .Bind(s => Option.Some(s + "World"));

        Assert.False(binded.IsSome);
    }
}
