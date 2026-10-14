using Application.Abstractions.Messaging;

namespace Architecture.Tests;

public class ApplicationDesignTests
{
    // Guard against vacuous greens: if NetArchTest stops matching open generics, the rules below
    // would pass on an empty set. GetSystemInfoQueryHandler must always be found.
    [Fact]
    public void HandlerDiscovery_ShouldFindAtLeastOneQueryHandler()
    {
        Types.InAssembly(Assemblies.Application)
            .That().ImplementInterface(typeof(IQueryHandler<,>))
            .GetTypes()
            .ShouldNotBeEmpty();
    }

    [Theory]
    [InlineData(typeof(ICommandHandler<,>))]
    [InlineData(typeof(IQueryHandler<,>))]
    public void Handlers_ShouldBeInternalAndSealed(Type handlerInterface)
    {
        TestResult result = Types.InAssembly(Assemblies.Application)
            .That().ImplementInterface(handlerInterface).And().AreClasses()
            .Should().NotBePublic().And().BeSealed()
            .GetResult();

        result.IsSuccessful.ShouldBeTrue(Assemblies.FailingTypes(result));
    }

    [Fact]
    public void DomainEntities_ShouldBeSealed()
    {
        TestResult result = Types.InAssembly(Assemblies.Domain)
            .That().Inherit(typeof(Domain.Abstractions.Entity)).And().AreNotAbstract()
            .Should().BeSealed()
            .GetResult();

        result.IsSuccessful.ShouldBeTrue(Assemblies.FailingTypes(result));
    }
}
