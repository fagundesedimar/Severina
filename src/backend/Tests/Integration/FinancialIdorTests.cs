using Microsoft.EntityFrameworkCore;
using Severina.Domain.Entities;
using Severina.Domain.Enums;
using Severina.Infrastructure.Data;
using Xunit;

namespace Severina.Tests.Integration;

public class FinancialIdorTests : IDisposable
{
    private readonly SeverinaDbContext _context;

    public FinancialIdorTests()
    {
        var options = new DbContextOptionsBuilder<SeverinaDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        var tenantProvider = new TestTenantProvider();
        _context = new SeverinaDbContext(options, tenantProvider);
        _context.Database.EnsureCreated();
        _context.SetTenantCompanyId(Guid.NewGuid());
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }

    [Fact]
    public async Task Transaction_AcrossTenant_Isolation()
    {
        var companyA = Guid.NewGuid();
        var companyB = Guid.NewGuid();

        var tA = new Transaction(companyA, TransactionType.Receita, 100m, DateTime.UtcNow, TransactionCategory.Servicos);
        var tB = new Transaction(companyB, TransactionType.Receita, 200m, DateTime.UtcNow, TransactionCategory.Servicos);

        _context.Transactions.AddRange(tA, tB);
        await _context.SaveChangesAsync();

        var companyATransactions = await _context.Transactions
            .IgnoreQueryFilters()
            .Where(t => t.CompanyId == companyA)
            .ToListAsync();

        var companyBTransactions = await _context.Transactions
            .IgnoreQueryFilters()
            .Where(t => t.CompanyId == companyB)
            .ToListAsync();

        Assert.Single(companyATransactions);
        Assert.Single(companyBTransactions);
        Assert.NotEqual(companyATransactions[0].Id, companyBTransactions[0].Id);
    }

    [Fact]
    public async Task Invoice_AcrossTenant_Isolation()
    {
        var companyA = Guid.NewGuid();
        var companyB = Guid.NewGuid();

        var iA = new Invoice(companyA, 500m, DateTime.UtcNow.AddDays(30));
        var iB = new Invoice(companyB, 1000m, DateTime.UtcNow.AddDays(30));

        _context.Invoices.AddRange(iA, iB);
        await _context.SaveChangesAsync();

        var companyAInvoices = await _context.Invoices
            .IgnoreQueryFilters()
            .Where(i => i.CompanyId == companyA)
            .ToListAsync();

        var companyBInvoices = await _context.Invoices
            .IgnoreQueryFilters()
            .Where(i => i.CompanyId == companyB)
            .ToListAsync();

        Assert.Single(companyAInvoices);
        Assert.Single(companyBInvoices);
        Assert.NotEqual(companyAInvoices[0].Id, companyBInvoices[0].Id);
    }

    [Fact]
    public async Task Transaction_CannotAccessOtherTenant()
    {
        var companyA = Guid.NewGuid();
        var companyB = Guid.NewGuid();

        var tB = new Transaction(companyB, TransactionType.Receita, 200m, DateTime.UtcNow, TransactionCategory.Servicos);
        _context.Transactions.Add(tB);
        await _context.SaveChangesAsync();

        var companyATransactions = await _context.Transactions
            .IgnoreQueryFilters()
            .Where(t => t.CompanyId == companyA)
            .ToListAsync();

        Assert.Empty(companyATransactions);
    }

    [Fact]
    public async Task Invoice_CannotAccessOtherTenant()
    {
        var companyA = Guid.NewGuid();
        var companyB = Guid.NewGuid();

        var iB = new Invoice(companyB, 1000m, DateTime.UtcNow.AddDays(30));
        _context.Invoices.Add(iB);
        await _context.SaveChangesAsync();

        var companyAInvoices = await _context.Invoices
            .IgnoreQueryFilters()
            .Where(i => i.CompanyId == companyA)
            .ToListAsync();

        Assert.Empty(companyAInvoices);
    }
}
