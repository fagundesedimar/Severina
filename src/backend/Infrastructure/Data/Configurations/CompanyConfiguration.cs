using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Severina.Domain.Entities;
using Severina.Domain.ValueObjects;

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
            .HasConversion(
                v => v.Value,
                v => CnpjCpf.Create(v))
            .IsRequired()
            .HasMaxLength(14);

        builder.Property(c => c.Email)
            .HasConversion(
                v => v.Value,
                v => Email.Create(v))
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(c => c.Telefone)
            .HasConversion(
                v => v != null ? v.Value : null,
                v => v != null ? Telefone.Create(v) : null)
            .HasMaxLength(11);

        builder.Property(c => c.TipoPessoa)
            .HasConversion<int>();

        builder.Property(c => c.Status)
            .HasConversion<int>();

        builder.Property(c => c.Plano)
            .HasMaxLength(50);

        builder.Property(c => c.Configuracoes)
            .IsRequired()
            .HasColumnType("text")
            .HasDefaultValue("{}");

        builder.HasIndex(c => c.CnpjCpf)
            .IsUnique();

        builder.HasIndex(c => c.Email)
            .IsUnique();

        builder.HasQueryFilter(c => c.DeletedAt == null);
    }
}
