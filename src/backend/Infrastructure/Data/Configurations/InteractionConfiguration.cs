using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Severina.Domain.Entities;

namespace Severina.Infrastructure.Data.Configurations;

public class InteractionConfiguration : IEntityTypeConfiguration<Interaction>
{
    public void Configure(EntityTypeBuilder<Interaction> builder)
    {
        builder.ToTable("Interactions");
        builder.HasKey(i => i.Id);

        builder.Property(i => i.Content)
            .IsRequired()
            .HasMaxLength(4000);

        builder.Property(i => i.Type)
            .HasConversion<int>();

        builder.OwnsOne(i => i.Metadata, metadataBuilder =>
        {
            metadataBuilder.Property(m => m.Direction)
                .HasColumnName("MetadataDirection")
                .HasMaxLength(10);

            metadataBuilder.Property(m => m.Status)
                .HasColumnName("MetadataStatus")
                .HasMaxLength(20);

            metadataBuilder.Property(m => m.ContentPreview)
                .HasColumnName("MetadataContentPreview")
                .HasMaxLength(200);
        });

        builder.HasIndex(i => new { i.CompanyId, i.ClientId, i.CreatedAt })
            .HasDatabaseName("IX_Interactions_CompanyId_ClientId_CreatedAt");

        builder.HasQueryFilter(i => i.DeletedAt == null);
    }
}
