namespace Domain.UnitTests.Abstractions;

public class ResultTests
{
    private static readonly ResultError NotFound = new("Test.NotFound", "Introuvable", ErrorType.NotFound);

    [Fact]
    public void Success_ShouldExposeValue_AndRefuseError()
    {
        Result<int> result = Result.Success(42);

        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldBe(42);
        Should.Throw<InvalidOperationException>(() => result.Error);
    }

    [Fact]
    public void Failure_ShouldExposeError_AndRefuseValue()
    {
        Result<int> result = NotFound;

        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldBe(NotFound);
        Should.Throw<InvalidOperationException>(() => result.Value);
    }

    [Fact]
    public void Map_ShouldPropagateFailure()
    {
        Result<int> result = NotFound;

        Result<string> mapped = result.Map(v => v.ToString(System.Globalization.CultureInfo.InvariantCulture));

        mapped.IsFailure.ShouldBeTrue();
        mapped.Error.Code.ShouldBe("Test.NotFound");
    }

    [Fact]
    public void ResultErrors_WithSameArgs_ShouldBeEqual()
    {
        var first = new ResultError("Folder.NameTooLong", "Nom trop long ({0})", [255], ErrorType.Validation);
        var second = new ResultError("Folder.NameTooLong", "Nom trop long ({0})", [255], ErrorType.Validation);

        first.ShouldBe(second);
        first.Message.ShouldBe("Nom trop long (255)");
    }
}
