using Microsoft.EntityFrameworkCore;
using Severina.Domain.Entities;
using Severina.Domain.Enums;
using Severina.Infrastructure.Data;
using Xunit;

namespace Severina.Tests.Integration;

public class InvoiceCrudTests : IDisposable
{
    private readonly SeverinaDbContext _context;
    private readonly Guid _companyId;

    public InvoiceCrudTests()
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
    public async Task AddInvoice_ReturnsInvoice()
    {
        var invoice = new Invoice(_companyId, 1000m, DateTime.UtcNow.AddDays(30));

        _context.Invoices.Add(invoice);
        await _context.SaveChangesAsync();

        var result = await _context.Invoices.FindAsync(invoice.Id);
        Assert.NotNull(result);
        Assert.Equal(1000m, result.Valor);
        Assert.Equal(InvoiceStatus.Pending, result.Status);
        Assert.Equal(_companyId, result.CompanyId);
    }

    [Fact]
    public async Task PayInvoice_FullAmount_SetsPaid()
    {
        var invoice = new Invoice(_companyId, 500m, DateTime.UtcNow.AddDays(30));
        _context.Invoices.Add(invoice);
        await _context.SaveChangesAsync();

        invoice.Pay(500m, DateTime.UtcNow);
        await _context.SaveChangesAsync();

        var result = await _context.Invoices.FindAsync(invoice.Id);
        Assert.Equal(InvoiceStatus.Paid, result!.Status);
        Assert.Equal(500m, result.ValorPago);
    }

    [Fact]
    public async Task PayInvoice_PartialAmount_SetsPartial()
    {
        var invoice = new Invoice(_companyId, 1000m, DateTime.UtcNow.AddDays(30));
        _context.Invoices.Add(invoice);
        await _context.SaveChangesAsync();

        invoice.Pay(300m, DateTime.UtcNow);
        await _context.SaveChangesAsync();

        var result = await _context.Invoices.FindAsync(invoice.Id);
        Assert.Equal(InvoiceStatus.Partial, result!.Status);
        Assert.Equal(300m, result.ValorPago);
    }

    [Fact]
    public async Task CancelInvoice_SetsCancelled()
    {
        var invoice = new Invoice(_companyId, 500m, DateTime.UtcNow.AddDays(30));
        _context.Invoices.Add(invoice);
        await _context.SaveChangesAsync();

        invoice.Cancel();
        await _context.SaveChangesAsync();

        var result = await _context.Invoices.FindAsync(invoice.Id);
        Assert.Equal(InvoiceStatus.Cancelled, result!.Status);
    }

    [Fact]
    public async Task MarkOverdue_SetsOverdue()
    {
        var invoice = new Invoice(_companyId, 500m, DateTime.UtcNow.AddDays(-5));
        _context.Invoices.Add(invoice);
        await _context.SaveChangesAsync();

        invoice.MarkOverdue();
        await _context.SaveChangesAsync();

        var result = await _context.Invoices.FindAsync(invoice.Id);
        Assert.Equal(InvoiceStatus.Overdue, result!.Status);
    }

    [Fact]
    public async Task SoftDeleteInvoice_SetsDeletedAt()
    {
        var invoice = new Invoice(_companyId, 500m, DateTime.UtcNow.AddDays(30));
        _context.Invoices.Add(invoice);
        await _context.SaveChangesAsync();

        invoice.SoftDelete();
        await _context.SaveChangesAsync();

        var result = await _context.Invoices.FindAsync(invoice.Id);
        Assert.NotNull(result!.DeletedAt);
    }

    [Fact]
    public async Task QueryFilter_OnlyReturnsCompanyInvoices()
    {
        var otherCompanyId = Guid.NewGuid();

        _context.Invoices.Add(new Invoice(_companyId, 500m, DateTime.UtcNow.AddDays(30)));
        _context.Invoices.Add(new Invoice(otherCompanyId, 1000m, DateTime.UtcNow.AddDays(30)));
        await _context.SaveChangesAsync();

        var results = await _context.Invoices.Where(i => i.CompanyId == _companyId).ToListAsync();
        Assert.Single(results);
    }
}
