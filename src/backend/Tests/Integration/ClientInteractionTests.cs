using Microsoft.EntityFrameworkCore;
using Severina.Domain.Entities;
using Severina.Domain.Enums;
using Severina.Domain.ValueObjects;
using Severina.Infrastructure.Data;
using Severina.Infrastructure.Repositories;
using Xunit;

namespace Severina.Tests.Integration;

public class ClientInteractionTests : IDisposable
{
    private readonly SeverinaDbContext _context;
    private readonly InteractionRepository _repository;
    private readonly Guid _companyId;
    private readonly Client _client;

    public ClientInteractionTests()
    {
        var options = new DbContextOptionsBuilder<SeverinaDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        var tenantProvider = new TestTenantProvider();
        _context = new SeverinaDbContext(options, tenantProvider);
        _context.Database.EnsureCreated();
        _repository = new InteractionRepository(_context);
        _companyId = Guid.NewGuid();
        tenantProvider.SetCompanyId(_companyId);

        _client = new Client(_companyId, "João Silva", "joao@teste.com", null, null);
        _context.Clients.Add(_client);
        _context.SaveChanges();
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }

    [Fact]
    public async Task AddInteraction_ReturnsInteraction()
    {
        var interaction = Interaction.Create(
            _client.Id,
            _companyId,
            InteractionType.Message,
            "Mensagem de teste");

        _context.Interactions.Add(interaction);
        await _context.SaveChangesAsync();

        var result = await _context.Interactions.FindAsync(interaction.Id);
        Assert.NotNull(result);
        Assert.Equal("Mensagem de teste", result.Content);
    }

    [Fact]
    public async Task GetByClientId_ReturnsInteractions()
    {
        _context.Interactions.AddRange(
            Interaction.Create(_client.Id, _companyId, InteractionType.Message, "Msg 1"),
            Interaction.Create(_client.Id, _companyId, InteractionType.Call, "Call 1"),
            Interaction.Create(_client.Id, _companyId, InteractionType.Email, "Email 1"));
        await _context.SaveChangesAsync();

        var results = await _repository.GetByClientIdAsync(_companyId, _client.Id, 0, 10);

        Assert.Equal(3, results.Count);
    }

    [Fact]
    public async Task GetByClientId_Pagination_Works()
    {
        for (int i = 0; i < 25; i++)
        {
            _context.Interactions.Add(
                Interaction.Create(_client.Id, _companyId, InteractionType.Message, $"Msg {i}"));
        }
        await _context.SaveChangesAsync();

        var page1 = await _repository.GetByClientIdAsync(_companyId, _client.Id, 0, 10);
        var page2 = await _repository.GetByClientIdAsync(_companyId, _client.Id, 10, 10);
        var page3 = await _repository.GetByClientIdAsync(_companyId, _client.Id, 20, 10);

        Assert.Equal(10, page1.Count);
        Assert.Equal(10, page2.Count);
        Assert.Equal(5, page3.Count);
    }

    [Fact]
    public async Task GetByClientIdAndType_FiltersCorrectly()
    {
        _context.Interactions.AddRange(
            Interaction.Create(_client.Id, _companyId, InteractionType.Message, "Msg 1"),
            Interaction.Create(_client.Id, _companyId, InteractionType.Call, "Call 1"),
            Interaction.Create(_client.Id, _companyId, InteractionType.Message, "Msg 2"));
        await _context.SaveChangesAsync();

        var results = await _repository.GetByClientIdAndTypeAsync(
            _companyId, _client.Id, InteractionType.Message, 0, 10);

        Assert.Equal(2, results.Count);
        Assert.All(results, i => Assert.Equal(InteractionType.Message, i.Type));
    }

    [Fact]
    public async Task CountByClientId_ReturnsCorrectCount()
    {
        _context.Interactions.AddRange(
            Interaction.Create(_client.Id, _companyId, InteractionType.Message, "Msg 1"),
            Interaction.Create(_client.Id, _companyId, InteractionType.Call, "Call 1"));
        await _context.SaveChangesAsync();

        var count = await _repository.CountByClientIdAsync(_companyId, _client.Id);

        Assert.Equal(2, count);
    }

    [Fact]
    public async Task DifferentCompany_DoesNotReturnInteractions()
    {
        var otherCompanyId = Guid.NewGuid();
        var otherClient = new Client(otherCompanyId, "Outro", null, null, null);
        _context.Clients.Add(otherClient);
        await _context.SaveChangesAsync();

        _context.Interactions.AddRange(
            Interaction.Create(_client.Id, _companyId, InteractionType.Message, "Msg 1"),
            Interaction.Create(otherClient.Id, otherCompanyId, InteractionType.Message, "Msg 2"));
        await _context.SaveChangesAsync();

        var results = await _repository.GetByClientIdAsync(_companyId, _client.Id, 0, 10);

        Assert.Single(results);
        Assert.Equal(_client.Id, results.First().ClientId);
    }

    [Fact]
    public async Task Interaction_WithMetadata_PersistsMetadata()
    {
        var metadata = InteractionMetadata.Create(
            direction: "inbound",
            durationSeconds: 120,
            status: "completed",
            contentPreview: "Preview");

        var interaction = Interaction.Create(
            _client.Id,
            _companyId,
            InteractionType.Call,
            "Chamada de teste",
            metadata);

        _context.Interactions.Add(interaction);
        await _context.SaveChangesAsync();

        var result = await _context.Interactions.FindAsync(interaction.Id);
        Assert.NotNull(result);
        Assert.NotNull(result.Metadata);
        Assert.Equal("inbound", result.Metadata.Direction);
        Assert.Equal(120, result.Metadata.DurationSeconds);
    }

    [Fact]
    public async Task Interactions_OrderedByCreatedAt_Descending()
    {
        var older = Interaction.Create(_client.Id, _companyId, InteractionType.Message, "Old");
        _context.Interactions.Add(older);
        await _context.SaveChangesAsync();

        var newer = Interaction.Create(_client.Id, _companyId, InteractionType.Message, "New");
        _context.Interactions.Add(newer);
        await _context.SaveChangesAsync();

        var results = await _repository.GetByClientIdAsync(_companyId, _client.Id, 0, 10);

        Assert.Equal(2, results.Count);
        Assert.Equal("New", results.First().Content);
    }
}
