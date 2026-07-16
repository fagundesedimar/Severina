using Severina.Domain.Entities;
using Severina.Domain.Enums;
using Xunit;

namespace Severina.Tests.Unit;

public class InvoiceTests
{
    private static Invoice CreateValidInvoice(
        Guid? companyId = null,
        decimal valor = 1000m,
        DateTime? dataVencimento = null)
    {
        return new Invoice(
            companyId ?? Guid.NewGuid(),
            valor,
            dataVencimento ?? DateTime.UtcNow.AddDays(30));
    }

    [Fact]
    public void Create_ValidInvoice_SetsStatusPending()
    {
        var invoice = CreateValidInvoice();
        Assert.Equal(InvoiceStatus.Pending, invoice.Status);
    }

    [Fact]
    public void Create_ValidInvoice_SetsValorPagoZero()
    {
        var invoice = CreateValidInvoice();
        Assert.Equal(0m, invoice.ValorPago);
    }

    [Fact]
    public void Create_ValidInvoice_RaisesCreatedEvent()
    {
        var invoice = CreateValidInvoice();
        Assert.Contains(invoice.DomainEvents, e => e is Severina.Domain.Events.InvoiceCreatedEvent);
    }

    [Fact]
    public void Pay_FullAmount_SetsStatusPaid()
    {
        var invoice = CreateValidInvoice(valor: 500m);
        invoice.ClearDomainEvents();
        invoice.Pay(500m, DateTime.UtcNow);
        Assert.Equal(InvoiceStatus.Paid, invoice.Status);
        Assert.Equal(500m, invoice.ValorPago);
    }

    [Fact]
    public void Pay_FullAmount_RaisesPaidEvent()
    {
        var invoice = CreateValidInvoice(valor: 500m);
        invoice.ClearDomainEvents();
        invoice.Pay(500m, DateTime.UtcNow);
        Assert.Contains(invoice.DomainEvents, e => e is Severina.Domain.Events.InvoicePaidEvent);
    }

    [Fact]
    public void Pay_PartialAmount_SetsStatusPartial()
    {
        var invoice = CreateValidInvoice(valor: 1000m);
        invoice.ClearDomainEvents();
        invoice.Pay(300m, DateTime.UtcNow);
        Assert.Equal(InvoiceStatus.Partial, invoice.Status);
        Assert.Equal(300m, invoice.ValorPago);
    }

    [Fact]
    public void Pay_MultiplePartialPayments_Accumulates()
    {
        var invoice = CreateValidInvoice(valor: 1000m);
        invoice.Pay(300m, DateTime.UtcNow);
        invoice.Pay(400m, DateTime.UtcNow);
        Assert.Equal(InvoiceStatus.Partial, invoice.Status);
        Assert.Equal(700m, invoice.ValorPago);
    }

    [Fact]
    public void Pay_AccumulatedPayments_ThenFull_SetsPaid()
    {
        var invoice = CreateValidInvoice(valor: 1000m);
        invoice.Pay(600m, DateTime.UtcNow);
        invoice.Pay(400m, DateTime.UtcNow);
        Assert.Equal(InvoiceStatus.Paid, invoice.Status);
    }

    [Fact]
    public void Pay_ExceedsValor_ThrowsException()
    {
        var invoice = CreateValidInvoice(valor: 500m);
        Assert.Throws<ArgumentException>(() => invoice.Pay(600m, DateTime.UtcNow));
    }

    [Fact]
    public void Pay_AlreadyPaid_ThrowsException()
    {
        var invoice = CreateValidInvoice(valor: 500m);
        invoice.Pay(500m, DateTime.UtcNow);
        Assert.Throws<InvalidOperationException>(() => invoice.Pay(100m, DateTime.UtcNow));
    }

    [Fact]
    public void Pay_CancelledInvoice_ThrowsException()
    {
        var invoice = CreateValidInvoice();
        invoice.Cancel();
        Assert.Throws<InvalidOperationException>(() => invoice.Pay(500m, DateTime.UtcNow));
    }

    [Fact]
    public void Cancel_PendingInvoice_SetsStatusCancelled()
    {
        var invoice = CreateValidInvoice();
        invoice.Cancel();
        Assert.Equal(InvoiceStatus.Cancelled, invoice.Status);
    }

    [Fact]
    public void Cancel_PartialInvoice_SetsStatusCancelled()
    {
        var invoice = CreateValidInvoice(valor: 1000m);
        invoice.Pay(300m, DateTime.UtcNow);
        invoice.Cancel();
        Assert.Equal(InvoiceStatus.Cancelled, invoice.Status);
    }

    [Fact]
    public void Cancel_PaidInvoice_ThrowsException()
    {
        var invoice = CreateValidInvoice(valor: 500m);
        invoice.Pay(500m, DateTime.UtcNow);
        Assert.Throws<InvalidOperationException>(() => invoice.Cancel());
    }

    [Fact]
    public void Cancel_AlreadyCancelled_ThrowsException()
    {
        var invoice = CreateValidInvoice();
        invoice.Cancel();
        Assert.Throws<InvalidOperationException>(() => invoice.Cancel());
    }

    [Fact]
    public void MarkOverdue_PendingInvoice_SetsStatusOverdue()
    {
        var invoice = CreateValidInvoice();
        invoice.ClearDomainEvents();
        invoice.MarkOverdue();
        Assert.Equal(InvoiceStatus.Overdue, invoice.Status);
    }

    [Fact]
    public void MarkOverdue_PartialInvoice_SetsStatusOverdue()
    {
        var invoice = CreateValidInvoice(valor: 1000m);
        invoice.Pay(300m, DateTime.UtcNow);
        invoice.ClearDomainEvents();
        invoice.MarkOverdue();
        Assert.Equal(InvoiceStatus.Overdue, invoice.Status);
    }

    [Fact]
    public void MarkOverdue_RaisesOverdueEvent()
    {
        var invoice = CreateValidInvoice();
        invoice.ClearDomainEvents();
        invoice.MarkOverdue();
        Assert.Contains(invoice.DomainEvents, e => e is Severina.Domain.Events.InvoiceOverdueEvent);
    }

    [Fact]
    public void MarkOverdue_PaidInvoice_DoesNothing()
    {
        var invoice = CreateValidInvoice(valor: 500m);
        invoice.Pay(500m, DateTime.UtcNow);
        invoice.MarkOverdue();
        Assert.Equal(InvoiceStatus.Paid, invoice.Status);
    }

    [Fact]
    public void UpdateDetails_PendingInvoice_UpdatesFields()
    {
        var invoice = CreateValidInvoice();
        var newDate = DateTime.UtcNow.AddDays(60);

        invoice.UpdateDetails(2000m, newDate, Guid.NewGuid(), "Updated");

        Assert.Equal(2000m, invoice.Valor);
        Assert.Equal(newDate, invoice.DataVencimento);
        Assert.Equal("Updated", invoice.Descricao);
    }

    [Fact]
    public void UpdateDetails_PaidInvoice_ThrowsException()
    {
        var invoice = CreateValidInvoice(valor: 500m);
        invoice.Pay(500m, DateTime.UtcNow);
        Assert.Throws<InvalidOperationException>(() =>
            invoice.UpdateDetails(2000m, DateTime.UtcNow, null, null));
    }

    [Fact]
    public void SetNumero_Invoice_SetsNumero()
    {
        var invoice = CreateValidInvoice();
        invoice.SetNumero("INV-001");
        Assert.Equal("INV-001", invoice.Numero);
    }
}
