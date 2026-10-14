IDistributedApplicationBuilder builder = DistributedApplication.CreateBuilder(args);

// isProxied: false everywhere — DCP proxy ports only bind 127.0.0.1, unreachable from
// containers under Podman rootless. Ports differ from GrgApp so both stacks can run together.

// PostgreSQL — tree, metadata, Mermaid code, Excalidraw scenes (spec § 3.2)
IResourceBuilder<ParameterResource> postgresPassword = builder.AddParameter("postgres-password", secret: true);
IResourceBuilder<PostgresServerResource> postgres = builder.AddPostgres("postgres", password: postgresPassword)
    .WithDataVolume("schemy-postgres-data")
    .WithEndpoint("tcp", endpoint =>
    {
        endpoint.Port = 5433;
        endpoint.IsProxied = false;
    })
    .WithPgAdmin(pgAdmin => pgAdmin.WithExplicitStart());

IResourceBuilder<PostgresDatabaseResource> database = postgres.AddDatabase("schemydb");

// MinIO — binaries only (folder logos, sketch files), consumed from T6 (spec § 3.3)
IResourceBuilder<ParameterResource> minioUser = builder.AddParameter("minio-user", "schemy");
IResourceBuilder<ParameterResource> minioPassword = builder.AddParameter("minio-password", secret: true);
// Official minio/minio images are no longer pullable: pgsty/minio is a community rebuild (same binary, CLI and S3 API as prod MinIO).
builder.AddContainer("minio", "pgsty/minio", "RELEASE.2026-08-04T00-00-00Z")
    .WithArgs("server", "/data", "--console-address", ":9001")
    .WithEnvironment("MINIO_ROOT_USER", minioUser)
    .WithEnvironment("MINIO_ROOT_PASSWORD", minioPassword)
    .WithVolume("schemy-minio-data", "/data")
    .WithHttpEndpoint(port: 9010, targetPort: 9000, name: "api", isProxied: false)
    .WithHttpEndpoint(port: 9011, targetPort: 9001, name: "console", isProxied: false)
    .WithHttpHealthCheck("/minio/health/live", endpointName: "api");

// API
IResourceBuilder<ProjectResource> api = builder.AddProject<Projects.Presentation>("api")
    .WithReference(database)
    .WaitFor(database)
    .WithEndpoint("http", endpoint =>
    {
        endpoint.Port = 7010;
        endpoint.IsProxied = false;
    });

// Web — Vite+ dev server; it proxies /api, /openapi and /swagger to API_URL (vite.config.ts)
builder.AddExecutable("web", "vp", "../Web", "dev")
    .WithEnvironment("API_URL", api.GetEndpoint("http"))
    .WithHttpEndpoint(port: 5180, env: "PORT", isProxied: false)
    .WaitFor(api);

await builder.Build().RunAsync();
