using Microsoft.EntityFrameworkCore;
using Severina.Domain.Entities;
using Severina.Domain.Enums;
using Severina.Domain.ValueObjects;
using Severina.Infrastructure.Data;
using Xunit;

namespace Severina.Tests.Integration;

public class RbacTests : IDisposable
{
    private readonly SeverinaDbContext _context;
    private readonly Company _company;

    public RbacTests()
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
    public void AdminUser_HasAdministradorRole()
    {
        var user = new User(
            _company.Id,
            "Admin",
            Email.Create("admin@teste.com"),
            "hash",
            PapelUsuario.Administrador);

        Assert.True(user.IsAdministrador);
    }

    [Fact]
    public void OperationalUser_DoesNotHaveAdministradorRole()
    {
        var user = new User(
            _company.Id,
            "Operador",
            Email.Create("operador@teste.com"),
            "hash",
            PapelUsuario.Operacional);

        Assert.False(user.IsAdministrador);
    }

    [Fact]
    public async Task ChangeRole_UpdatesUserRole()
    {
        var user = new User(
            _company.Id,
            "Operador",
            Email.Create("operador@teste.com"),
            "hash",
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
    public async Task UsersInDifferentCompanies_AreIsolated()
    {
        var company1 = new Company(
            "Empresa 1",
            CnpjCpf.Create("52998224725"),
            Email.Create("empresa1@teste.com"),
            TipoPessoa.Fisica);

        var company2 = new Company(
            "Empresa 2",
            CnpjCpf.Create("12345678000195"),
            Email.Create("empresa2@teste.com"),
            TipoPessoa.Fisica);

        _context.Companies.AddRange(company1, company2);
        await _context.SaveChangesAsync();

        var user1 = new User(company1.Id, "User 1", Email.Create("user1@empresa1.com"), "hash", PapelUsuario.Operacional);
        var user2 = new User(company2.Id, "User 2", Email.Create("user2@empresa2.com"), "hash", PapelUsuario.Operacional);

        _context.Users.AddRange(user1, user2);
        await _context.SaveChangesAsync();

        var company1Users = await _context.Users.IgnoreQueryFilters()
            .Where(u => u.CompanyId == company1.Id)
            .ToListAsync();

        var company2Users = await _context.Users.IgnoreQueryFilters()
            .Where(u => u.CompanyId == company2.Id)
            .ToListAsync();

        Assert.Single(company1Users);
        Assert.Single(company2Users);
        Assert.NotEqual(company1Users[0].CompanyId, company2Users[0].CompanyId);
    }
}
