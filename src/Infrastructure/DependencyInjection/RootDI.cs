using Infrastructure.Persistence;
using Microsoft.Extensions.Hosting;

namespace Infrastructure.DependencyInjection;

public static class InfrastructureDependencyInjectionRoot
{
    /// <summary>Aspire connection name, shared with AspireHost (postgres.AddDatabase("schemydb")).</summary>
    public const string DatabaseConnectionName = "schemydb";

    extension(IHostApplicationBuilder builder)
    {
        public IHostApplicationBuilder AddInfrastructure()
        {
            builder.AddNpgsqlDbContext<SchemyDbContext>(DatabaseConnectionName);
            return builder;
        }
    }
}
