using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Severina.Domain.Entities;

namespace Severina.Infrastructure.Data.Configurations;

public class UserPreferenceConfiguration : IEntityTypeConfiguration<UserPreference>
{
    public void Configure(EntityTypeBuilder<UserPreference> builder)
    {
        builder.ToTable("UserPreferences");

        builder.HasKey(p => p.Id);

        builder.Property(p => p.Key)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(p => p.Value)
            .IsRequired()
            .HasMaxLength(500);

        builder.HasIndex(p => new { p.UserId, p.Key })
            .IsUnique();

        builder.HasOne(p => p.User)
            .WithMany()
            .HasForeignKey(p => p.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasQueryFilter(p => p.DeletedAt == null);
    }
}
