using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence;

/// <summary>
/// EF Core write model. Never exposed outside Infrastructure: Application talks to
/// ILibraryRepository / IUnitOfWork (added in T2).
/// </summary>
public sealed class SchemyDbContext(DbContextOptions<SchemyDbContext> options) : DbContext(options)
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
        => modelBuilder.ApplyConfigurationsFromAssembly(typeof(SchemyDbContext).Assembly);
}
