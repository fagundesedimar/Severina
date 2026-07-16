using Microsoft.EntityFrameworkCore;
using Severina.Domain.Entities;
using Severina.Domain.Enums;
using Severina.Infrastructure.Data;
using Xunit;

namespace Severina.Tests.Integration;

public class AppointmentCrudTests : IDisposable
{
    private readonly SeverinaDbContext _context;
    private readonly Guid _companyId;

    public AppointmentCrudTests()
    {
        var options = new DbContextOptionsBuilder<SeverinaDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        var tenantProvider = new TestTenantProvider();
        _context = new SeverinaDbContext(options, tenantProvider);
        _context.Database.EnsureCreated();

        _companyId = Guid.NewGuid();
        tenantProvider.SetCompanyId(_companyId);
        _context.SetTenantCompanyId(_companyId);
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }

    [Fact]
    public async Task AddAppointment_ReturnsAppointment()
    {
        var appointment = new Appointment(
            _companyId,
            "Reunião de teste",
            DateTime.UtcNow.AddHours(1),
            DateTime.UtcNow.AddHours(2),
            TipoCompromisso.Reuniao);

        _context.Appointments.Add(appointment);
        await _context.SaveChangesAsync();

        var result = await _context.Appointments.FindAsync(appointment.Id);
        Assert.NotNull(result);
        Assert.Equal("Reunião de teste", result.Titulo);
        Assert.Equal(_companyId, result.CompanyId);
    }

    [Fact]
    public async Task UpdateAppointment_UpdatesProperties()
    {
        var appointment = new Appointment(
            _companyId,
            "Reunião de teste",
            DateTime.UtcNow.AddHours(1),
            DateTime.UtcNow.AddHours(2),
            TipoCompromisso.Reuniao);

        _context.Appointments.Add(appointment);
        await _context.SaveChangesAsync();

        appointment.UpdateDetails("Reunião atualizada", "Nova descrição", TipoCompromisso.FollowUp, null);
        await _context.SaveChangesAsync();

        var result = await _context.Appointments.FindAsync(appointment.Id);
        Assert.NotNull(result);
        Assert.Equal("Reunião atualizada", result.Titulo);
        Assert.Equal(TipoCompromisso.FollowUp, result.Tipo);
    }

    [Fact]
    public async Task DeleteAppointment_SoftDeletes()
    {
        var appointment = new Appointment(
            _companyId,
            "Reunião de teste",
            DateTime.UtcNow.AddHours(1),
            DateTime.UtcNow.AddHours(2),
            TipoCompromisso.Reuniao);

        _context.Appointments.Add(appointment);
        await _context.SaveChangesAsync();

        appointment.SoftDelete();
        await _context.SaveChangesAsync();

        var result = await _context.Appointments.FindAsync(appointment.Id);
        Assert.NotNull(result);
        Assert.NotNull(result.DeletedAt);
    }

    [Fact]
    public async Task QueryFilter_ExcludesDeletedAppointments()
    {
        var appointment = new Appointment(
            _companyId,
            "Reunião de teste",
            DateTime.UtcNow.AddHours(1),
            DateTime.UtcNow.AddHours(2),
            TipoCompromisso.Reuniao);

        _context.Appointments.Add(appointment);
        await _context.SaveChangesAsync();

        appointment.SoftDelete();
        await _context.SaveChangesAsync();

        var result = await _context.Appointments.FindAsync(appointment.Id);
        Assert.Null(result);
    }

    [Fact]
    public async Task QueryFilter_IsolatesByCompanyId()
    {
        var otherCompanyId = Guid.NewGuid();
        var appointment1 = new Appointment(
            _companyId,
            "Empresa 1",
            DateTime.UtcNow.AddHours(1),
            DateTime.UtcNow.AddHours(2),
            TipoCompromisso.Reuniao);

        var appointment2 = new Appointment(
            otherCompanyId,
            "Empresa 2",
            DateTime.UtcNow.AddHours(1),
            DateTime.UtcNow.AddHours(2),
            TipoCompromisso.Reuniao);

        _context.Appointments.AddRange(appointment1, appointment2);
        await _context.SaveChangesAsync();

        var results = await _context.Appointments.ToListAsync();
        Assert.Single(results);
        Assert.Equal(_companyId, results[0].CompanyId);
    }
}
