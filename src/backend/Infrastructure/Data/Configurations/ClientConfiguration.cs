using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Severina.Domain.Entities;

namespace Severina.Infrastructure.Data.Configurations;

public class ClientConfiguration : IEntityTypeConfiguration<Client>
{
    public void Configure(EntityTypeBuilder<Client> builder)
    {
        builder.ToTable("Clients");
        builder.HasKey(c => c.Id);

        builder.Property(c => c.Nome)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(c => c.Telefone)
            .HasMaxLength(20);

        builder.Property(c => c.Empresa)
            .HasMaxLength(200);

        builder.Property(c => c.Status)
            .HasConversion<int>();

        builder.OwnsOne(c => c.Email, emailBuilder =>
        {
            emailBuilder.Property(e => e.Value)
                .HasColumnName("Email")
                .HasMaxLength(256);

            emailBuilder.HasIndex(e => e.Value)
                .HasDatabaseName("IX_Clients_Email_Search");
        });

        builder.Ignore(c => c.Tags);
        builder.Ignore(c => c.Notes);
        builder.Ignore(c => c.Interactions);

        builder.HasIndex(c => c.CompanyId)
            .HasDatabaseName("IX_Clients_CompanyId");

        builder.HasIndex(c => new { c.CompanyId, c.Nome })
            .HasDatabaseName("IX_Clients_CompanyId_Nome");

        builder.HasIndex(c => new { c.CompanyId, c.Telefone })
            .HasDatabaseName("IX_Clients_CompanyId_Telefone");

        builder.HasIndex(c => new { c.CompanyId, c.Nome })
            .HasDatabaseName("IX_Clients_CompanyId_Nome_Search");

        builder.HasIndex(c => new { c.CompanyId, c.Empresa })
            .HasDatabaseName("IX_Clients_CompanyId_Empresa_Search");

        builder.HasQueryFilter(c => c.DeletedAt == null);

        builder.Ignore(c => c.DomainEvents);
    }
}
