namespace Core.Tests;

public class ResultAsyncTests
{
    [Fact]
    public async void MapSuccess_WithObject()
    {
        var x = "Hello";
        var result = Task.FromResult(Result.Success<string, Error>(x));
        var mapped = await result.Map(s => s.Length);
        
        Assert.True(mapped.IsSuccess);
        Assert.Equal(x.Length, mapped.Data);
    }

    [Fact]
    public async void MapSuccess_ToFailure_WithNullObject()
    {
        var x = "Hello";
        var result = Task.FromResult(Result.Success<string, Error>(x));
        var mapped = await result
            .Map(s => s + "\n")
            .Map(i => (string)null!);
       
        Assert.False(mapped.IsSuccess);
    }

    [Fact]
    public async void BindSuccess_WithObject()
    {
        var x = "Hello";
        var result = Task.FromResult(Result.Success<string, Error>(x));
        var binded = await result
            .Bind(s => Task.FromResult(Result.Success<string, Error>(s + " ")))
            .Bind(s => Task.FromResult(Result.Success<string, Error>(s + "World")));

        Assert.True(binded.IsSuccess);
        Assert.Equal("Hello World", binded.Data);
    }

    [Fact]
    public async void BindSuccess_WithFailure()
    {
        var x = "Hello";
        var result = Task.FromResult(Result.Success<string, Error>(x));
        var binded = await result
            .Bind(s => Task.FromResult(Result.Failure<string, Error>(message: "Failure")))
            .Bind(s => Task.FromResult(Result.Success<string, Error>(s + "World")));

        Assert.False(binded.IsSuccess);
        Assert.Equal("Failure", binded.Error.Message);
    }

    [Fact]
    public async void CreateSuccess_WithOperation()
    {
        var result = await Result.AsResult<string>(async () => await Task.FromResult("Hello"));

        Assert.True(result.IsSuccess);
        Assert.Equal("Hello", result.Data);
    }

    [Fact]
    public async void CreateFailure_WithOperation()
    {
        var result = await Result.AsResult<string>(
            async () =>
            {
                return await Task.FromResult(Result.Failure<string, Error>(message: "Failure"));
                #pragma warning disable CS0162
                return "Hello";
                #pragma warning restore CS0162
            });

        Assert.False(result.IsSuccess);
        Assert.True(result.IsFailure);
        Assert.Equal("Failure", result.Message);
    }

        [Fact]
    public async void CreateException_WithOperation()
    {
        var exception = new Exception("Exceptional failure");
        var result = await Result.AsResult<string>(
            async () =>
            {
                throw exception;
                #pragma warning disable CS0162
                return await Task.FromResult("Hello");
                #pragma warning restore CS0162
            });
        Assert.False(result.IsSuccess);
        Assert.True(result.IsFailure);
        Assert.Equal(exception, result.Error.Exception);
    }

    [Fact]
    public async void UnfoldResults_WithSuccess()
    {
        var results = 
            Enumerable.AsEnumerable(
            [
                Task.FromResult(Result.Success<string, Error>("Hello")),
                Task.FromResult(Result.Success<string, Error>("World")),
                Task.FromResult(Result.Success<string, Error>("!"))
            ]);
        
        var result = await Result.Unfold(results);

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
        Assert.Equal(results.Count(), result.Data.Count());
    }


    [Fact]
    public async void UnfoldResults_WithFailure()
    {
        var results = 
            Enumerable.AsEnumerable(
            [
                Task.FromResult(Result.Success<string, Error>("Hello")),
                Task.FromResult(Result.Failure<string, Error>(message: "World")),
                Task.FromResult(Result.Success<string, Error>("!"))
            ]);
        
        var result = await Result.Unfold(results);

        Assert.False(result.IsSuccess);
        Assert.Null(result.Data);
        Assert.Equal("World", result.Message);
        Assert.Equal("World", result.Error.Message);
    }
}
