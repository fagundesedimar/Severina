using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Severina.Domain.Entities;
using Severina.Domain.Enums;

namespace Severina.Infrastructure.Data.Configurations;

public class InvoiceConfiguration : IEntityTypeConfiguration<Invoice>
{
    public void Configure(EntityTypeBuilder<Invoice> builder)
    {
        builder.ToTable("Invoices");
        builder.HasKey(i => i.Id);

        builder.Property(i => i.Numero)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(i => i.Valor)
            .HasColumnType("decimal(18,2)");

        builder.Property(i => i.ValorPago)
            .HasColumnType("decimal(18,2)");

        builder.Property(i => i.Descricao)
            .HasMaxLength(500);

        builder.Property(i => i.Status)
            .HasConversion<int>();

        builder.Ignore(i => i.DomainEvents);

        builder.HasIndex(i => new { i.CompanyId, i.DataVencimento, i.Status })
            .HasDatabaseName("IX_Invoices_CompanyId_Vencimento_Status");

        builder.HasQueryFilter(i => i.DeletedAt == null);
    }
}
