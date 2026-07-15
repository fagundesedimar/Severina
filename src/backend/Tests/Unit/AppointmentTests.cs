using Severina.Domain.Entities;
using Severina.Domain.Enums;
using Xunit;

namespace Severina.Tests.Unit;

public class AppointmentTests
{
    private static Appointment CreateValidAppointment(
        Guid? companyId = null,
        DateTime? dataHoraInicio = null,
        DateTime? dataHoraFim = null)
    {
        var inicio = dataHoraInicio ?? DateTime.UtcNow.AddHours(1);
        var fim = dataHoraFim ?? DateTime.UtcNow.AddHours(2);

        return new Appointment(
            companyId ?? Guid.NewGuid(),
            "Reunião de teste",
            inicio,
            fim,
            TipoCompromisso.Reuniao);
    }

    [Fact]
    public void Create_ValidAppointment_SetsStatusScheduled()
    {
        var appointment = CreateValidAppointment();
        Assert.Equal(StatusCompromisso.Scheduled, appointment.Status);
    }

    [Fact]
    public void Create_ValidAppointment_SetsProperties()
    {
        var companyId = Guid.NewGuid();
        var inicio = DateTime.UtcNow.AddHours(1);
        var fim = DateTime.UtcNow.AddHours(2);

        var appointment = new Appointment(
            companyId,
            "Reunião importante",
            inicio,
            fim,
            TipoCompromisso.FollowUp,
            Guid.NewGuid(),
            "Descrição teste");

        Assert.Equal(companyId, appointment.CompanyId);
        Assert.Equal("Reunião importante", appointment.Titulo);
        Assert.Equal(inicio, appointment.DataHoraInicio);
        Assert.Equal(fim, appointment.DataHoraFim);
        Assert.Equal(TipoCompromisso.FollowUp, appointment.Tipo);
        Assert.NotNull(appointment.ClientId);
        Assert.Equal("Descrição teste", appointment.Descricao);
    }

    [Fact]
    public void Confirm_ScheduledAppointment_SetsStatusConfirmed()
    {
        var appointment = CreateValidAppointment();
        appointment.Confirm();
        Assert.Equal(StatusCompromisso.Confirmed, appointment.Status);
    }

    [Fact]
    public void Confirm_CancelledAppointment_ThrowsException()
    {
        var appointment = CreateValidAppointment();
        appointment.Cancel();
        Assert.Throws<InvalidOperationException>(() => appointment.Confirm());
    }

    [Fact]
    public void Confirm_CompletedAppointment_ThrowsException()
    {
        var appointment = CreateValidAppointment(
            dataHoraInicio: DateTime.UtcNow.AddHours(-2),
            dataHoraFim: DateTime.UtcNow.AddHours(-1));
        appointment.Complete();
        Assert.Throws<InvalidOperationException>(() => appointment.Confirm());
    }

    [Fact]
    public void Cancel_ScheduledAppointment_SetsStatusCancelled()
    {
        var appointment = CreateValidAppointment();
        appointment.Cancel();
        Assert.Equal(StatusCompromisso.Cancelled, appointment.Status);
    }

    [Fact]
    public void Cancel_CompletedAppointment_ThrowsException()
    {
        var appointment = CreateValidAppointment(
            dataHoraInicio: DateTime.UtcNow.AddHours(-2),
            dataHoraFim: DateTime.UtcNow.AddHours(-1));
        appointment.Complete();
        Assert.Throws<InvalidOperationException>(() => appointment.Cancel());
    }

    [Fact]
    public void Cancel_AlreadyCancelledAppointment_ThrowsException()
    {
        var appointment = CreateValidAppointment();
        appointment.Cancel();
        Assert.Throws<InvalidOperationException>(() => appointment.Cancel());
    }

    [Fact]
    public void Complete_ScheduledAppointment_SetsStatusCompleted()
    {
        var appointment = CreateValidAppointment(
            dataHoraInicio: DateTime.UtcNow.AddHours(-2),
            dataHoraFim: DateTime.UtcNow.AddHours(-1));
        appointment.Complete();
        Assert.Equal(StatusCompromisso.Completed, appointment.Status);
    }

    [Fact]
    public void Complete_CancelledAppointment_ThrowsException()
    {
        var appointment = CreateValidAppointment();
        appointment.Cancel();
        Assert.Throws<InvalidOperationException>(() => appointment.Complete());
    }

    [Fact]
    public void Complete_FutureAppointment_ThrowsException()
    {
        var appointment = CreateValidAppointment(
            dataHoraInicio: DateTime.UtcNow.AddHours(2),
            dataHoraFim: DateTime.UtcNow.AddHours(3));
        Assert.Throws<InvalidOperationException>(() => appointment.Complete());
    }

    [Fact]
    public void Reschedule_ScheduledAppointment_UpdatesTimes()
    {
        var appointment = CreateValidAppointment();
        var newInicio = DateTime.UtcNow.AddHours(5);
        var newFim = DateTime.UtcNow.AddHours(6);

        appointment.Reschedule(newInicio, newFim);

        Assert.Equal(newInicio, appointment.DataHoraInicio);
        Assert.Equal(newFim, appointment.DataHoraFim);
    }

    [Fact]
    public void Reschedule_CompletedAppointment_ThrowsException()
    {
        var appointment = CreateValidAppointment(
            dataHoraInicio: DateTime.UtcNow.AddHours(-2),
            dataHoraFim: DateTime.UtcNow.AddHours(-1));
        appointment.Complete();

        Assert.Throws<InvalidOperationException>(() =>
            appointment.Reschedule(DateTime.UtcNow.AddHours(5), DateTime.UtcNow.AddHours(6)));
    }

    [Fact]
    public void Reschedule_InvalidTimeRange_ThrowsException()
    {
        var appointment = CreateValidAppointment();
        var newInicio = DateTime.UtcNow.AddHours(6);
        var newFim = DateTime.UtcNow.AddHours(5);

        Assert.Throws<ArgumentException>(() => appointment.Reschedule(newInicio, newFim));
    }

    [Fact]
    public void SoftDelete_Appointment_SetsDeletedAt()
    {
        var appointment = CreateValidAppointment();
        appointment.SoftDelete();
        Assert.NotNull(appointment.DeletedAt);
    }
}
