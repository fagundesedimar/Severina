using Microsoft.EntityFrameworkCore;
using Severina.Domain.Entities;
using Severina.Domain.Enums;
using Severina.Domain.ValueObjects;
using Severina.Infrastructure.Data;
using Severina.Infrastructure.Repositories;
using Xunit;

namespace Severina.Tests.Integration;

public class ClientCrudTests : IDisposable
{
    private readonly SeverinaDbContext _context;
    private readonly ClientRepository _repository;
    private readonly Guid _companyId;

    public ClientCrudTests()
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
        _context.SetTenantCompanyId(_companyId);
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }

    [Fact]
    public async Task AddClient_ReturnsClient()
    {
        var client = new Client(_companyId, "João Silva", "joao@teste.com", "11999998888", "Empresa Teste");

        _context.Clients.Add(client);
        await _context.SaveChangesAsync();

        var result = await _context.Clients.FindAsync(client.Id);
        Assert.NotNull(result);
        Assert.Equal("João Silva", result.Nome);
        Assert.Equal(_companyId, result.CompanyId);
    }

    [Fact]
    public async Task AddClient_WithTags_PersistsTags()
    {
        var client = new Client(_companyId, "João Silva", "joao@teste.com", null, null);
        client.AddTag("VIP");
        client.AddTag("Premium");

        _context.Clients.Add(client);
        await _context.SaveChangesAsync();

        var result = await _context.Clients.FindAsync(client.Id);
        Assert.NotNull(result);
        Assert.Equal(2, result.Tags.Count);
    }

    [Fact]
    public async Task AddClient_WithNotes_PersistsNotes()
    {
        var client = new Client(_companyId, "João Silva", "joao@teste.com", null, null);
        var authorId = Guid.NewGuid();
        client.AddNote("Nota de teste", authorId);

        _context.Clients.Add(client);
        await _context.SaveChangesAsync();

        var result = await _context.Clients.FindAsync(client.Id);
        Assert.NotNull(result);
        Assert.Single(result.Notes);
        Assert.Equal("Nota de teste", result.Notes.First().Content);
    }

    [Fact]
    public async Task UpdateClient_UpdatesProperties()
    {
        var client = new Client(_companyId, "João Silva", "joao@teste.com", null, null);
        _context.Clients.Add(client);
        await _context.SaveChangesAsync();

        client.UpdateContactInfo("Maria Santos", "maria@teste.com", "11888887777", "Nova Empresa");
        await _context.SaveChangesAsync();

        var result = await _context.Clients.FindAsync(client.Id);
        Assert.NotNull(result);
        Assert.Equal("Maria Santos", result.Nome);
        Assert.Equal("maria@teste.com", result.Email!.Value);
    }

    [Fact]
    public async Task DeleteClient_SoftDelete_SetsDeletedAt()
    {
        var client = new Client(_companyId, "João Silva", "joao@teste.com", null, null);
        _context.Clients.Add(client);
        await _context.SaveChangesAsync();

        client.Deactivate();
        await _context.SaveChangesAsync();

        var result = await _context.Clients.IgnoreQueryFilters()
            .FirstOrDefaultAsync(c => c.Id == client.Id);

        Assert.NotNull(result);
        Assert.Equal(StatusCliente.Inativo, result.Status);
    }

    [Fact]
    public async Task SearchClients_ByNome_ReturnsMatching()
    {
        _context.Clients.AddRange(
            new Client(_companyId, "João Silva", "joao@teste.com", null, null),
            new Client(_companyId, "Maria Santos", "maria@teste.com", null, null),
            new Client(_companyId, "João Paulo", "jp@teste.com", null, null));
        await _context.SaveChangesAsync();

        var results = await _repository.SearchAsync(_companyId, "João", 0, 10);

        Assert.Equal(2, results.Count);
        Assert.All(results, c => Assert.Contains("joão", c.Nome.ToLower()));
    }

    [Fact]
    public async Task SearchClients_ByEmpresa_ReturnsMatching()
    {
        _context.Clients.AddRange(
            new Client(_companyId, "João Silva", null, null, "Empresa ABC"),
            new Client(_companyId, "Maria Santos", null, null, "Empresa XYZ"),
            new Client(_companyId, "Pedro Costa", null, null, "Empresa ABC"));
        await _context.SaveChangesAsync();

        var results = await _repository.SearchAsync(_companyId, "ABC", 0, 10);

        Assert.Equal(2, results.Count);
    }

    [Fact]
    public async Task SearchClients_ByEmail_ReturnsMatching()
    {
        _context.Clients.AddRange(
            new Client(_companyId, "João Silva", "joao@teste.com", null, null),
            new Client(_companyId, "Maria Santos", "maria@outro.com", null, null));
        await _context.SaveChangesAsync();

        var results = await _repository.SearchAsync(_companyId, "teste.com", 0, 10);

        Assert.Single(results);
        Assert.Equal("João Silva", results.First().Nome);
    }

    [Fact]
    public async Task SearchClients_Pagination_Works()
    {
        for (int i = 0; i < 25; i++)
        {
            _context.Clients.Add(new Client(_companyId, $"Cliente {i}", null, null, null));
        }
        await _context.SaveChangesAsync();

        var page1 = await _repository.SearchAsync(_companyId, "Cliente", 0, 10);
        var page2 = await _repository.SearchAsync(_companyId, "Cliente", 10, 10);
        var page3 = await _repository.SearchAsync(_companyId, "Cliente", 20, 10);

        Assert.Equal(10, page1.Count);
        Assert.Equal(10, page2.Count);
        Assert.Equal(5, page3.Count);
    }

    [Fact]
    public async Task CountSearch_ReturnsCorrectCount()
    {
        _context.Clients.AddRange(
            new Client(_companyId, "João Silva", "joao@teste.com", null, null),
            new Client(_companyId, "Maria Santos", "maria@teste.com", null, null),
            new Client(_companyId, "João Paulo", "jp@teste.com", null, null));
        await _context.SaveChangesAsync();

        var count = await _repository.CountSearchAsync(_companyId, "João");

        Assert.Equal(2, count);
    }

    [Fact]
    public async Task ExistsByEmail_ExistingEmail_ReturnsTrue()
    {
        _context.Clients.Add(new Client(_companyId, "João Silva", "joao@teste.com", null, null));
        await _context.SaveChangesAsync();

        var exists = await _repository.ExistsByEmailAsync(_companyId, "joao@teste.com");

        Assert.True(exists);
    }

    [Fact]
    public async Task ExistsByEmail_NonExistingEmail_ReturnsFalse()
    {
        var exists = await _repository.ExistsByEmailAsync(_companyId, "inexistente@teste.com");

        Assert.False(exists);
    }

    [Fact]
    public async Task ExistsByEmail_WithExcludeId_ReturnsFalse()
    {
        var client = new Client(_companyId, "João Silva", "joao@teste.com", null, null);
        _context.Clients.Add(client);
        await _context.SaveChangesAsync();

        var exists = await _repository.ExistsByEmailAsync(_companyId, "joao@teste.com", client.Id);

        Assert.False(exists);
    }

    [Fact]
    public async Task GetByIdWithTags_ReturnsClientWithTags()
    {
        var client = new Client(_companyId, "João Silva", "joao@teste.com", null, null);
        client.AddTag("VIP");
        _context.Clients.Add(client);
        await _context.SaveChangesAsync();

        var result = await _repository.GetByIdWithTagsAsync(_companyId, client.Id);

        Assert.NotNull(result);
        Assert.Single(result.Tags);
    }

    [Fact]
    public async Task DifferentCompany_ReturnsNull()
    {
        var otherCompanyId = Guid.NewGuid();
        var client = new Client(otherCompanyId, "Outro Cliente", null, null, null);
        _context.Set<Company>();
        _context.Clients.Add(client);
        await _context.SaveChangesAsync();

        var result = await _repository.GetByIdWithTagsAsync(_companyId, client.Id);

        Assert.Null(result);
    }
}
