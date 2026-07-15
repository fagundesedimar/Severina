using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Severina.Domain.Entities;
using Severina.Domain.Enums;

namespace Severina.Infrastructure.Data.Configurations;

public class AppointmentConfiguration : IEntityTypeConfiguration<Appointment>
{
    public void Configure(EntityTypeBuilder<Appointment> builder)
    {
        builder.ToTable("Appointments");

        builder.HasKey(a => a.Id);

        builder.Property(a => a.Titulo)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(a => a.Descricao)
            .HasMaxLength(2000);

        builder.Property(a => a.Tipo)
            .HasConversion<int>();

        builder.Property(a => a.Status)
            .HasConversion<int>();

        builder.Property(a => a.RecurrenceRuleJson)
            .HasMaxLength(2000);

        builder.Ignore(a => a.Company);
        builder.Ignore(a => a.DomainEvents);

        builder.HasIndex(a => new { a.CompanyId, a.DataHoraInicio })
            .HasDatabaseName("IX_Appointments_CompanyId_DataHoraInicio");

        builder.HasIndex(a => a.ClientId)
            .HasDatabaseName("IX_Appointments_ClientId");

        builder.HasOne(a => a.Company)
            .WithMany()
            .HasForeignKey(a => a.CompanyId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
