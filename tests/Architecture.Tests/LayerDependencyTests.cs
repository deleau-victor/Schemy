namespace Architecture.Tests;

/// <summary>
/// Clean Architecture dependency rule: code dependencies only point inwards
/// (Domain ← Application ← Infrastructure / Presentation).
/// </summary>
public class LayerDependencyTests
{
    [Theory]
    [InlineData("Application")]
    [InlineData("Infrastructure")]
    [InlineData("Presentation")]
    public void Domain_ShouldNotDependOn_OuterLayers(string layer)
    {
        TestResult result = Types.InAssembly(Assemblies.Domain).ShouldNot().HaveDependencyOn(layer).GetResult();

        result.IsSuccessful.ShouldBeTrue(Assemblies.FailingTypes(result));
    }

    [Theory]
    [InlineData("Microsoft.EntityFrameworkCore")]
    [InlineData("Microsoft.AspNetCore")]
    [InlineData("Microsoft.Extensions")]
    [InlineData("FluentValidation")]
    [InlineData("Npgsql")]
    [InlineData("Dapper")]
    [InlineData("Minio")]
    public void Domain_ShouldNotDependOn_Frameworks(string framework)
    {
        TestResult result = Types.InAssembly(Assemblies.Domain).ShouldNot().HaveDependencyOn(framework).GetResult();

        result.IsSuccessful.ShouldBeTrue(Assemblies.FailingTypes(result));
    }

    [Theory]
    [InlineData("Infrastructure")]
    [InlineData("Presentation")]
    public void Application_ShouldNotDependOn_OuterLayers(string layer)
    {
        TestResult result = Types.InAssembly(Assemblies.Application).ShouldNot().HaveDependencyOn(layer).GetResult();

        result.IsSuccessful.ShouldBeTrue(Assemblies.FailingTypes(result));
    }

    [Theory]
    [InlineData("Microsoft.EntityFrameworkCore")]
    [InlineData("Npgsql")]
    [InlineData("Dapper")]
    [InlineData("Minio")]
    [InlineData("Microsoft.AspNetCore")]
    public void Application_ShouldNotDependOn_TechnicalDetails(string technology)
    {
        TestResult result = Types.InAssembly(Assemblies.Application).ShouldNot().HaveDependencyOn(technology).GetResult();

        result.IsSuccessful.ShouldBeTrue(Assemblies.FailingTypes(result));
    }

    [Fact]
    public void Infrastructure_ShouldNotDependOn_Presentation()
    {
        TestResult result = Types.InAssembly(Assemblies.Infrastructure).ShouldNot().HaveDependencyOn("Presentation").GetResult();

        result.IsSuccessful.ShouldBeTrue(Assemblies.FailingTypes(result));
    }

    [Theory]
    [InlineData("Microsoft.EntityFrameworkCore")]
    [InlineData("Infrastructure.Persistence")]
    public void PresentationEndpoints_ShouldNotDependOn_Persistence(string dependency)
    {
        TestResult result = Types.InAssembly(Assemblies.Presentation)
            .That().ResideInNamespaceStartingWith("Presentation.Endpoints")
            .ShouldNot().HaveDependencyOn(dependency)
            .GetResult();

        result.IsSuccessful.ShouldBeTrue(Assemblies.FailingTypes(result));
    }
}
