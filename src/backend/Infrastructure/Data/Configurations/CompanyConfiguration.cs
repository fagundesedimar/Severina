using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Severina.Domain.Entities;

namespace Severina.Infrastructure.Data.Configurations;

public class CompanyConfiguration : IEntityTypeConfiguration<Company>
{
    public void Configure(EntityTypeBuilder<Company> builder)
    {
        builder.ToTable("Companies");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.Nome)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(c => c.CnpjCpf)
            .IsRequired()
            .HasMaxLength(18);

        builder.Property(c => c.Email)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(c => c.Telefone)
            .HasMaxLength(20);

        builder.Property(c => c.TipoPessoa)
            .HasConversion<int>();

        builder.Property(c => c.Status)
            .HasConversion<int>();

        builder.Property(c => c.Plano)
            .HasMaxLength(50);

        builder.HasIndex(c => c.CnpjCpf)
            .IsUnique();

        builder.HasIndex(c => c.Email)
            .IsUnique();

        builder.HasQueryFilter(c => c.DeletedAt == null);
    }
}
