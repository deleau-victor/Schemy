using Application.Abstractions.Messaging;

namespace Architecture.Tests;

public class NamingConventionTests
{
    [Theory]
    [InlineData(typeof(ICommandHandler<,>), "CommandHandler")]
    [InlineData(typeof(IQueryHandler<,>), "QueryHandler")]
    public void Handlers_ShouldHaveSuffix_AndLiveInFeatures(Type handlerInterface, string suffix)
    {
        TestResult result = Types.InAssembly(Assemblies.Application)
            .That().ImplementInterface(handlerInterface).And().AreClasses()
            .Should().HaveNameEndingWith(suffix, StringComparison.Ordinal)
            .And().ResideInNamespaceStartingWith("Application.Features")
            .GetResult();

        result.IsSuccessful.ShouldBeTrue(Assemblies.FailingTypes(result));
    }

    [Fact]
    public void Validators_ShouldHaveValidatorSuffix()
    {
        TestResult result = Types.InAssembly(Assemblies.Application)
            .That().Inherit(typeof(FluentValidation.AbstractValidator<>))
            .Should().HaveNameEndingWith("Validator", StringComparison.Ordinal)
            .GetResult();

        result.IsSuccessful.ShouldBeTrue(Assemblies.FailingTypes(result));
    }

    [Fact]
    public void Endpoints_ShouldHaveEndpointsSuffix()
    {
        TestResult result = Types.InAssembly(Assemblies.Presentation)
            .That().ResideInNamespace("Presentation.Endpoints").And().AreClasses()
            .Should().HaveNameEndingWith("Endpoints", StringComparison.Ordinal)
            .GetResult();

        result.IsSuccessful.ShouldBeTrue(Assemblies.FailingTypes(result));
    }
}
