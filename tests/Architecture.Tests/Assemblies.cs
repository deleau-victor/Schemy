using System.Reflection;

namespace Architecture.Tests;

internal static class Assemblies
{
    public static readonly Assembly Domain = typeof(Domain.Abstractions.Entity).Assembly;
    public static readonly Assembly Application = typeof(Application.Abstractions.Messaging.ICommand<>).Assembly;
    public static readonly Assembly Infrastructure = typeof(Infrastructure.Persistence.SchemyDbContext).Assembly;
    public static readonly Assembly Presentation = Assembly.Load("Presentation");

    public static string FailingTypes(TestResult result) =>
        result.FailingTypeNames is null ? string.Empty : $"Types fautifs : {string.Join(", ", result.FailingTypeNames)}";
}
