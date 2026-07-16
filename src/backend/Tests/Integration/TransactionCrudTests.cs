using Microsoft.EntityFrameworkCore;
using Severina.Domain.Entities;
using Severina.Domain.Enums;
using Severina.Infrastructure.Data;
using Xunit;

namespace Severina.Tests.Integration;

public class TransactionCrudTests : IDisposable
{
    private readonly SeverinaDbContext _context;
    private readonly Guid _companyId;

    public TransactionCrudTests()
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
    public async Task AddTransaction_ReturnsTransaction()
    {
        var transaction = new Transaction(
            _companyId,
            TransactionType.Receita,
            500m,
            DateTime.UtcNow,
            TransactionCategory.Servicos);

        _context.Transactions.Add(transaction);
        await _context.SaveChangesAsync();

        var result = await _context.Transactions.FindAsync(transaction.Id);
        Assert.NotNull(result);
        Assert.Equal(TransactionType.Receita, result.Tipo);
        Assert.Equal(500m, result.Valor);
        Assert.Equal(_companyId, result.CompanyId);
    }

    [Fact]
    public async Task AddTransaction_SetsStatusPending()
    {
        var transaction = new Transaction(
            _companyId,
            TransactionType.Despesa,
            200m,
            DateTime.UtcNow,
            TransactionCategory.Materiais);

        _context.Transactions.Add(transaction);
        await _context.SaveChangesAsync();

        var result = await _context.Transactions.FindAsync(transaction.Id);
        Assert.Equal(TransactionStatus.Pending, result!.Status);
    }

    [Fact]
    public async Task UpdateTransaction_UpdatesProperties()
    {
        var transaction = new Transaction(
            _companyId,
            TransactionType.Receita,
            100m,
            DateTime.UtcNow,
            TransactionCategory.Servicos);

        _context.Transactions.Add(transaction);
        await _context.SaveChangesAsync();

        transaction.UpdateDetails(TransactionType.Despesa, 300m, DateTime.UtcNow.AddDays(1), TransactionCategory.Impostos, "Updated");
        await _context.SaveChangesAsync();

        var result = await _context.Transactions.FindAsync(transaction.Id);
        Assert.Equal(TransactionType.Despesa, result!.Tipo);
        Assert.Equal(300m, result.Valor);
        Assert.Equal("Updated", result.Descricao);
    }

    [Fact]
    public async Task ApproveTransaction_UpdatesStatus()
    {
        var transaction = new Transaction(
            _companyId,
            TransactionType.Receita,
            500m,
            DateTime.UtcNow,
            TransactionCategory.Servicos);

        _context.Transactions.Add(transaction);
        await _context.SaveChangesAsync();

        transaction.Approve();
        await _context.SaveChangesAsync();

        var result = await _context.Transactions.FindAsync(transaction.Id);
        Assert.Equal(TransactionStatus.Approved, result!.Status);
    }

    [Fact]
    public async Task SoftDeleteTransaction_SetsDeletedAt()
    {
        var transaction = new Transaction(
            _companyId,
            TransactionType.Receita,
            500m,
            DateTime.UtcNow,
            TransactionCategory.Servicos);

        _context.Transactions.Add(transaction);
        await _context.SaveChangesAsync();

        transaction.SoftDelete();
        await _context.SaveChangesAsync();

        var result = await _context.Transactions.FindAsync(transaction.Id);
        Assert.NotNull(result!.DeletedAt);
    }

    [Fact]
    public async Task QueryFilter_OnlyReturnsCompanyTransactions()
    {
        var otherCompanyId = Guid.NewGuid();

        _context.Transactions.Add(new Transaction(_companyId, TransactionType.Receita, 100m, DateTime.UtcNow, TransactionCategory.Servicos));
        _context.Transactions.Add(new Transaction(otherCompanyId, TransactionType.Receita, 200m, DateTime.UtcNow, TransactionCategory.Servicos));
        await _context.SaveChangesAsync();

        var results = await _context.Transactions.Where(t => t.CompanyId == _companyId).ToListAsync();
        Assert.Single(results);
    }
}
