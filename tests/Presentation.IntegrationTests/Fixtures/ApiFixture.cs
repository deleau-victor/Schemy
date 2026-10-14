using Testcontainers.PostgreSql;

namespace Presentation.IntegrationTests.Fixtures;

/// <summary>
/// One PostgreSQL container and one WebApplicationFactory for the whole collection.
/// Requires DOCKER_HOST to point at the Podman socket (see docs/dev-setup.md).
/// </summary>
public sealed class ApiFixture : IAsyncLifetime
{
    private PostgreSqlContainer? _postgres;

    public SchemyApiFactory Factory { get; private set; } = null!;

    public async Task InitializeAsync()
    {
        _postgres = new PostgreSqlBuilder("postgres:17-alpine")
            .WithTmpfsMount("/var/lib/postgresql/data")
            .Build();

        await _postgres.StartAsync();

        Factory = new SchemyApiFactory(_postgres.GetConnectionString());
    }

    public async Task DisposeAsync()
    {
        await Factory.DisposeAsync();

        if (_postgres is not null)
            await _postgres.DisposeAsync();
    }
}

[CollectionDefinition(Name)]
public sealed class ApiCollection : ICollectionFixture<ApiFixture>
{
    public const string Name = "api";
}
