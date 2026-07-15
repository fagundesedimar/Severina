using Microsoft.EntityFrameworkCore;
using Severina.Domain.Entities;
using Severina.Domain.Enums;
using Severina.Domain.ValueObjects;
using Severina.Infrastructure.Data;
using Xunit;

namespace Severina.Tests.Integration;

public class UserCrudTests : IDisposable
{
    private readonly SeverinaDbContext _context;
    private readonly Company _company;

    public UserCrudTests()
    {
        var options = new DbContextOptionsBuilder<SeverinaDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        var tenantProvider = new TestTenantProvider();
        _context = new SeverinaDbContext(options, tenantProvider);
        _context.Database.EnsureCreated();

        _company = new Company(
            "Empresa Teste",
            CnpjCpf.Create("52998224725"),
            Email.Create("empresa@teste.com"),
            TipoPessoa.Fisica);

        _context.Companies.Add(_company);
        _context.SaveChanges();
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }

    [Fact]
    public async Task AddUser_ReturnsUser()
    {
        var user = new User(
            _company.Id,
            "Usuário Teste",
            Email.Create("usuario@teste.com"),
            "senha_hash",
            PapelUsuario.Operacional);

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        var result = await _context.Users.IgnoreQueryFilters().FirstOrDefaultAsync(u => u.Id == user.Id);
        Assert.NotNull(result);
        Assert.Equal("Usuário Teste", result.Nome);
    }

    [Fact]
    public async Task UpdateUserRole_UpdatesPapel()
    {
        var user = new User(
            _company.Id,
            "Usuário Teste",
            Email.Create("usuario@teste.com"),
            "senha_hash",
            PapelUsuario.Operacional);

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        user.ChangeRole(PapelUsuario.Administrador);
        await _context.SaveChangesAsync();

        var result = await _context.Users.IgnoreQueryFilters().FirstOrDefaultAsync(u => u.Id == user.Id);
        Assert.NotNull(result);
        Assert.Equal(PapelUsuario.Administrador, result.Papel);
    }

    [Fact]
    public async Task DeactivateUser_UpdatesStatus()
    {
        var user = new User(
            _company.Id,
            "Usuário Teste",
            Email.Create("usuario@teste.com"),
            "senha_hash",
            PapelUsuario.Operacional);

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        user.Deactivate();
        await _context.SaveChangesAsync();

        var result = await _context.Users.IgnoreQueryFilters().FirstOrDefaultAsync(u => u.Id == user.Id);
        Assert.NotNull(result);
        Assert.Equal(StatusUsuario.Inativo, result.Status);
    }

    [Fact]
    public async Task GetByCompanyId_ReturnsCompanyUsers()
    {
        var user1 = new User(_company.Id, "Usuário 1", Email.Create("user1@teste.com"), "hash", PapelUsuario.Operacional);
        var user2 = new User(_company.Id, "Usuário 2", Email.Create("user2@teste.com"), "hash", PapelUsuario.Administrador);

        _context.Users.AddRange(user1, user2);
        await _context.SaveChangesAsync();

        var result = await _context.Users.IgnoreQueryFilters()
            .Where(u => u.CompanyId == _company.Id)
            .ToListAsync();

        Assert.Equal(2, result.Count);
    }

    [Fact]
    public async Task UpdateProfile_UpdatesNome()
    {
        var user = new User(
            _company.Id,
            "Usuário Teste",
            Email.Create("usuario@teste.com"),
            "senha_hash",
            PapelUsuario.Operacional);

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        user.UpdateProfile("Novo Nome", null);
        await _context.SaveChangesAsync();

        var result = await _context.Users.IgnoreQueryFilters().FirstOrDefaultAsync(u => u.Id == user.Id);
        Assert.NotNull(result);
        Assert.Equal("Novo Nome", result.Nome);
    }
}
