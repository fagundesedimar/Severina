using Microsoft.EntityFrameworkCore;
using Severina.Domain.Entities;

namespace Severina.Infrastructure.Data;

public class SeverinaDbContext : DbContext
{
    public SeverinaDbContext(DbContextOptions<SeverinaDbContext> options) : base(options) { }

    public DbSet<Company> Companies => Set<Company>();
    public DbSet<User> Users => Set<User>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(SeverinaDbContext).Assembly);
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        foreach (var entry in ChangeTracker.Entries<BaseEntity>())
        {
            if (entry.State == EntityState.Modified)
            {
                entry.Entity.UpdateTimestamp();
            }
        }

        return base.SaveChangesAsync(cancellationToken);
    }
}
