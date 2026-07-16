using Microsoft.EntityFrameworkCore;
using Severina.Domain.Entities;
using Severina.Domain.Enums;
using Severina.Infrastructure.Data;
using Severina.Infrastructure.Repositories;
using Xunit;

namespace Severina.Tests.Integration;

public class ClientSearchTests : IDisposable
{
    private readonly SeverinaDbContext _context;
    private readonly ClientRepository _repository;
    private readonly Guid _companyId;

    public ClientSearchTests()
    {
        var options = new DbContextOptionsBuilder<SeverinaDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        var tenantProvider = new TestTenantProvider();
        _context = new SeverinaDbContext(options, tenantProvider);
        _context.Database.EnsureCreated();
        _repository = new ClientRepository(_context);
        _companyId = Guid.NewGuid();
        tenantProvider.SetCompanyId(_companyId);
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }

    [Fact]
    public async Task Search_CaseInsensitive_ReturnsMatches()
    {
        _context.Clients.AddRange(
            new Client(_companyId, "JOÃO SILVA", "JOAO@TESTE.COM", null, "EMPRESA ABC"),
            new Client(_companyId, "maria santos", "maria@teste.com", null, null));
        await _context.SaveChangesAsync();

        var results = await _repository.SearchAsync(_companyId, "joão", 0, 10);

        Assert.Single(results);
        Assert.Equal("JOÃO SILVA", results.First().Nome);
    }

    [Fact]
    public async Task Search_PartialMatch_ReturnsMatches()
    {
        _context.Clients.AddRange(
            new Client(_companyId, "João Silva", null, null, null),
            new Client(_companyId, "João Paulo", null, null, null),
            new Client(_companyId, "Maria Santos", null, null, null));
        await _context.SaveChangesAsync();

        var results = await _repository.SearchAsync(_companyId, "jo", 0, 10);

        Assert.Equal(2, results.Count);
    }

    [Fact]
    public async Task Search_TrimmedTerm_Works()
    {
        _context.Clients.Add(new Client(_companyId, "João Silva", null, null, null));
        await _context.SaveChangesAsync();

        var results = await _repository.SearchAsync(_companyId, "  joão  ", 0, 10);

        Assert.Single(results);
    }

    [Fact]
    public async Task Search_NoMatches_ReturnsEmpty()
    {
        _context.Clients.Add(new Client(_companyId, "João Silva", null, null, null));
        await _context.SaveChangesAsync();

        var results = await _repository.SearchAsync(_companyId, "XYZ", 0, 10);

        Assert.Empty(results);
    }

    [Fact]
    public async Task Search_OnlyActiveClients_ReturnsActive()
    {
        var activeClient = new Client(_companyId, "Ativo Silva", null, null, null);
        var inactiveClient = new Client(_companyId, "Inativo Santos", null, null, null);
        inactiveClient.Deactivate();

        _context.Clients.AddRange(activeClient, inactiveClient);
        await _context.SaveChangesAsync();

        var results = await _repository.SearchAsync(_companyId, "Silva", 0, 10);

        Assert.Single(results);
        Assert.Equal("Ativo Silva", results.First().Nome);
    }

    [Fact]
    public async Task Search_ExcludesDeletedClients()
    {
        var client = new Client(_companyId, "Deletado Silva", null, null, null);
        _context.Clients.Add(client);
        await _context.SaveChangesAsync();

        _context.Clients.Remove(client);
        await _context.SaveChangesAsync();

        var results = await _repository.SearchAsync(_companyId, "Silva", 0, 10);

        Assert.Empty(results);
    }

    [Fact]
    public async Task Search_OrderByCreatedAt_Descending()
    {
        var olderClient = new Client(_companyId, "Cliente Antigo", null, null, null);
        _context.Clients.Add(olderClient);
        await _context.SaveChangesAsync();

        var newerClient = new Client(_companyId, "Cliente Novo", null, null, null);
        _context.Clients.Add(newerClient);
        await _context.SaveChangesAsync();

        var results = await _repository.SearchAsync(_companyId, "Cliente", 0, 10);

        Assert.Equal(2, results.Count);
        Assert.Equal("Cliente Novo", results.First().Nome);
    }

    [Fact]
    public async Task Search_CountMatches_SearchCount()
    {
        _context.Clients.AddRange(
            new Client(_companyId, "João Silva", null, null, null),
            new Client(_companyId, "Maria Santos", null, null, null),
            new Client(_companyId, "João Paulo", null, null, null));
        await _context.SaveChangesAsync();

        var searchResults = await _repository.SearchAsync(_companyId, "João", 0, 10);
        var count = await _repository.CountSearchAsync(_companyId, "João");

        Assert.Equal(searchResults.Count, count);
    }

    [Fact]
    public async Task Search_MultiFieldMatch_ReturnsUnion()
    {
        _context.Clients.AddRange(
            new Client(_companyId, "Empresa Teste", null, null, "Empresa ABC"),
            new Client(_companyId, "João Silva", "empresa@teste.com", null, null),
            new Client(_companyId, "Maria Santos", null, null, "XYZ Corp"));
        await _context.SaveChangesAsync();

        var results = await _repository.SearchAsync(_companyId, "empresa", 0, 10);

        Assert.Equal(2, results.Count);
    }
}
