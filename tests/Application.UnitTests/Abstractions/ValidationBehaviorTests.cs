using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace Application.UnitTests.Abstractions;

public class ValidationBehaviorTests
{
    public sealed record RenameCommand(string Name) : ICommand<string>;

    public sealed record SearchQuery(string Text) : IQuery<string>;

    public sealed class RenameCommandValidator : AbstractValidator<RenameCommand>
    {
        public RenameCommandValidator() => RuleFor(c => c.Name).NotEmpty().WithErrorCode("Rename.NameEmpty");
    }

    public sealed class SearchQueryValidator : AbstractValidator<SearchQuery>
    {
        public SearchQueryValidator() => RuleFor(q => q.Text).NotEmpty();
    }

    private static ValidationBehavior<Result<string>> CreateBehavior() =>
        new(new ServiceCollection()
            .AddSingleton<IValidator<RenameCommand>, RenameCommandValidator>()
            .AddSingleton<IValidator<SearchQuery>, SearchQueryValidator>()
            .BuildServiceProvider());

    [Fact]
    public async Task InvalidCommand_ShouldShortCircuit_WithStructuredValidationError()
    {
        bool nextCalled = false;

        Result<string> result = await CreateBehavior().Handle(
            new RenameCommand(""),
            () => { nextCalled = true; return Task.FromResult(Result.Success("ok")); },
            CancellationToken.None);

        nextCalled.ShouldBeFalse();
        result.IsFailure.ShouldBeTrue();
        result.Error.Code.ShouldBe("Validation.Failed");
        result.Error.Type.ShouldBe(ErrorType.Validation);
        result.Error.ValidationErrors!["Name"][0].Code.ShouldBe("Rename.NameEmpty");
    }

    [Fact]
    public async Task ValidCommand_ShouldReachHandler()
    {
        Result<string> result = await CreateBehavior().Handle(
            new RenameCommand("Architecture"),
            () => Task.FromResult(Result.Success("ok")),
            CancellationToken.None);

        result.Value.ShouldBe("ok");
    }

    [Fact]
    public async Task Query_ShouldNotBeValidated_EvenWithValidatorRegistered()
    {
        Result<string> result = await CreateBehavior().Handle(
            new SearchQuery(""),
            () => Task.FromResult(Result.Success("ok")),
            CancellationToken.None);

        result.Value.ShouldBe("ok");
    }
}
