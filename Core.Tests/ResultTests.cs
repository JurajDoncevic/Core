namespace Core.Tests;

public class ResultTests
{
    [Fact]
    public void CreateSuccess_WithObject()
    {
        var result = Result.Success<string, Error>("Hello");
        
        Assert.True(result.IsSuccess);
        Assert.Equal("Hello", result.Data);
    }

    [Fact]
    public void CreateSuccess_WithValue()
    {
        int x = 42;
        var result = Result.Success<int, Error>(x);
        
        Assert.True(result.IsSuccess);
        Assert.Equal(x, result.Data);
    }


    [Fact]
    public void CreateFailure_WithValue()
    {
        var failureMessage = "Failure";
        var result = Result.Failure<int, Error>(message: failureMessage);
        
        Assert.False(result.IsSuccess);
        Assert.Equal(failureMessage, result.Message);
    }

    [Fact]
    public void CreateFailure_WithMessage()
    {
        var failureMessage = "Failure";
        var result = Result.Failure<object, Error>(message: failureMessage);
        
        Assert.False(result.IsSuccess);
        Assert.Equal(failureMessage, result.Message);
    }

    [Fact]
    public void CreateException_WithError()
    {
        var exception = new Exception("Failure");
        var result = Result.Failure<int, Error>(Error.FromException(exception));
        
        Assert.False(result.IsSuccess);
        Assert.Equal(exception, result.Error.Exception);
    }

    [Fact]
    public void CreateException_WithException()
    {
        var exception = new Exception("Failure");
        var result = Result.Failure<object, Error>(exception);
        
        Assert.False(result.IsSuccess);
        Assert.Equal(exception, result.Error.Exception);
    }

    [Fact]
    public void CreateSuccess_WithNull_AsFailure()
    {
        var result = Result.Success<string, Error>(null!);
        
        Assert.False(result.IsSuccess);
        Assert.Null(result.Data);
    }

    [Fact]
    public void MapSuccess_WithObject()
    {
        var x = "Hello";
        var result = Result.Success<string, Error>(x);
        var mapped = result.Map(s => s.Length);
        
        Assert.True(mapped.IsSuccess);
        Assert.Equal(x.Length, mapped.Data);
    }

    [Fact]
    public void MapSuccess_ToFailure_WithNullObject()
    {
        var x = "Hello";
        var result = Result.Success<string, Error>(x);
        var mapped = result
            .Map(s => s + "\n")
            .Map(i => (string)null!);
       
        Assert.False(mapped.IsSuccess);
    }

    [Fact]
    public void BindSuccess_WithObject()
    {
        var x = "Hello";
        var result = Result.Success<string, Error>(x);
        var binded = result
            .Bind(s => Result.Success<string, Error>(s + " "))
            .Bind(s => Result.Success<string, Error>(s + "World"));

        Assert.True(binded.IsSuccess);
        Assert.Equal("Hello World", binded.Data);
    }

    [Fact]
    public void BindSuccess_WithFailure()
    {
        var x = "Hello";
        var result = Result.Success<string, Error>(x);
        var binded = result
            .Bind(s => Result.Failure<string, Error>(message: "Failure"))
            .Bind(s => Result.Success<string, Error>(s + "World"));

        Assert.False(binded.IsSuccess);
    }

    [Fact]
    public void CreateSuccess_WithOperation()
    {
        var result = Result.AsResult<string>(() => "Hello");

        Assert.True(result.IsSuccess);
        Assert.Equal("Hello", result.Data);
    }

    [Fact]
    public void CreateFailure_WithOperation()
    {
        var result = Result.AsResult<string>(
            () =>
            {
                return Result.Failure<string, Error>(message: "Failure");
                #pragma warning disable CS0162
                return "Hello";
                #pragma warning restore CS0162
            });

        Assert.False(result.IsSuccess);
        Assert.True(result.IsFailure);
        Assert.Equal("Failure", result.Message);
    }

        [Fact]
    public void CreateException_WithOperation()
    {
        var exception = new Exception("Exceptional failure");
        var result = Result.AsResult<string>(
            () =>
            {
                throw exception;
                #pragma warning disable CS0162
                return "Hello";
                #pragma warning restore CS0162
            });
        Assert.False(result.IsSuccess);
        Assert.True(result.IsFailure);
        Assert.Equal(exception, result.Error.Exception);
    }

    [Fact]
    public void UnfoldResults_WithSuccess()
    {
        var results = 
            Enumerable.AsEnumerable(
            [
                Result.Success<string, Error>("Hello"),
                Result.Success<string, Error>("World"),
                Result.Success<string, Error>("!")
            ]);
        
        var result = Result.Unfold(results);

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
        Assert.Equal(results.Count(), result.Data.Count());
    }


    [Fact]
    public void UnfoldResults_WithFailure()
    {
        var results = 
            Enumerable.AsEnumerable(
            [
                Result.Success<string, Error>("Hello"),
                Result.Failure<string, Error>(message: "World"),
                Result.Success<string, Error>("!")
            ]);
        
        var result = Result.Unfold(results);

        Assert.False(result.IsSuccess);
        Assert.Null(result.Data);
        Assert.Equal("World", result.Message);
        Assert.Equal("World", result.Error.Message);
    }
}
