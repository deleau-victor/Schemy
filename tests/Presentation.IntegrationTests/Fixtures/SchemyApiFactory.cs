using Infrastructure.DependencyInjection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;

namespace Presentation.IntegrationTests.Fixtures;

public sealed class SchemyApiFactory(string postgresConnectionString) : WebApplicationFactory<Program>
{
    private const string ConnectionKey = $"ConnectionStrings:{InfrastructureDependencyInjectionRoot.DatabaseConnectionName}";

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");

        // Injected before Aspire's AddNpgsqlDbContext reads configuration.
        builder.UseSetting(ConnectionKey, postgresConnectionString);
        builder.ConfigureAppConfiguration((_, config) =>
            config.AddInMemoryCollection(new Dictionary<string, string?> { [ConnectionKey] = postgresConnectionString }));
    }
}
