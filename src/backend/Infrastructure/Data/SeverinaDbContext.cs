using Microsoft.EntityFrameworkCore;
using Severina.Application.Interfaces;
using Severina.Domain.Entities;

namespace Severina.Infrastructure.Data;

public class SeverinaDbContext : DbContext
{
    private readonly ITenantProvider _tenantProvider;
    private Guid? _tenantCompanyId;

    public SeverinaDbContext(DbContextOptions<SeverinaDbContext> options, ITenantProvider tenantProvider)
        : base(options)
    {
        _tenantProvider = tenantProvider;
    }

    public DbSet<Company> Companies => Set<Company>();
    public DbSet<User> Users => Set<User>();
    public DbSet<UserPreference> UserPreferences => Set<UserPreference>();
    public DbSet<Appointment> Appointments => Set<Appointment>();
    public DbSet<Client> Clients => Set<Client>();
    public DbSet<Interaction> Interactions => Set<Interaction>();
    public DbSet<ImportJob> ImportJobs => Set<ImportJob>();
    public DbSet<Transaction> Transactions => Set<Transaction>();
    public DbSet<Invoice> Invoices => Set<Invoice>();
    public DbSet<ExportJob> ExportJobs => Set<ExportJob>();

    public void SetTenantCompanyId(Guid? companyId)
    {
        _tenantCompanyId = companyId;
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(SeverinaDbContext).Assembly);

        modelBuilder.Entity<User>()
            .HasQueryFilter(u => u.DeletedAt == null && u.CompanyId == _tenantCompanyId);

        modelBuilder.Entity<Company>()
            .HasQueryFilter(c => c.DeletedAt == null);

        modelBuilder.Entity<Appointment>()
            .HasQueryFilter(a => a.DeletedAt == null && a.CompanyId == _tenantCompanyId);

        modelBuilder.Entity<Client>()
            .HasQueryFilter(c => c.DeletedAt == null && c.CompanyId == _tenantCompanyId);

        modelBuilder.Entity<Interaction>()
            .HasQueryFilter(i => i.DeletedAt == null && i.CompanyId == _tenantCompanyId);

        modelBuilder.Entity<Transaction>()
            .HasQueryFilter(t => t.DeletedAt == null && t.CompanyId == _tenantCompanyId);

        modelBuilder.Entity<Invoice>()
            .HasQueryFilter(i => i.DeletedAt == null && i.CompanyId == _tenantCompanyId);
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
