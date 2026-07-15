using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Severina.Domain.Entities;

namespace Severina.Infrastructure.Data.Configurations;

public class ImportJobConfiguration : IEntityTypeConfiguration<ImportJob>
{
    public void Configure(EntityTypeBuilder<ImportJob> builder)
    {
        builder.ToTable("ImportJobs");
        builder.HasKey(j => j.Id);

        builder.Property(j => j.FileName)
            .IsRequired()
            .HasMaxLength(256);

        builder.Property(j => j.ErrorMessage)
            .HasMaxLength(1000);

        builder.Property(j => j.Status)
            .HasConversion<int>();

        builder.HasIndex(j => new { j.CompanyId, j.Id })
            .HasDatabaseName("IX_ImportJobs_CompanyId_Id");
    }
}
