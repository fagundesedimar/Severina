using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Severina.Domain.Entities;
using Severina.Domain.Enums;

namespace Severina.Infrastructure.Data.Configurations;

public class TransactionConfiguration : IEntityTypeConfiguration<Transaction>
{
    public void Configure(EntityTypeBuilder<Transaction> builder)
    {
        builder.ToTable("Transactions");
        builder.HasKey(t => t.Id);

        builder.Property(t => t.Valor)
            .HasColumnType("decimal(18,2)");

        builder.Property(t => t.Tipo)
            .HasConversion<int>();

        builder.Property(t => t.Status)
            .HasConversion<int>();

        builder.Property(t => t.Categoria)
            .HasConversion<int>();

        builder.Property(t => t.Descricao)
            .HasMaxLength(500);

        builder.Ignore(t => t.DomainEvents);

        builder.HasIndex(t => new { t.CompanyId, t.Data })
            .HasDatabaseName("IX_Transactions_CompanyId_Data");

        builder.HasIndex(t => new { t.CompanyId, t.Status })
            .HasDatabaseName("IX_Transactions_CompanyId_Status");

        builder.HasQueryFilter(t => t.DeletedAt == null);
    }
}
