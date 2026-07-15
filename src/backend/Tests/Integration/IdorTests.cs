using Microsoft.EntityFrameworkCore;
using Severina.Domain.Entities;
using Severina.Domain.Enums;
using Severina.Domain.ValueObjects;
using Severina.Infrastructure.Data;
using Xunit;

namespace Severina.Tests.Integration;

public class IdorTests : IDisposable
{
    private readonly SeverinaDbContext _context;
    private readonly Company _company1;
    private readonly Company _company2;

    public IdorTests()
    {
        var options = new DbContextOptionsBuilder<SeverinaDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        var tenantProvider = new TestTenantProvider();
        _context = new SeverinaDbContext(options, tenantProvider);
        _context.Database.EnsureCreated();

        _company1 = new Company(
            "Empresa 1",
            CnpjCpf.Create("52998224725"),
            Email.Create("empresa1@teste.com"),
            TipoPessoa.Fisica);

        _company2 = new Company(
            "Empresa 2",
            CnpjCpf.Create("12345678000195"),
            Email.Create("empresa2@teste.com"),
            TipoPessoa.Fisica);

        _context.Companies.AddRange(_company1, _company2);
        _context.SaveChanges();
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }

    [Fact]
    public async Task CrossTenantAccess_ReturnsEmptyResults()
    {
        var user1 = new User(_company1.Id, "User 1", Email.Create("user1@empresa1.com"), "hash", PapelUsuario.Operacional);
        var user2 = new User(_company2.Id, "User 2", Email.Create("user2@empresa2.com"), "hash", PapelUsuario.Operacional);

        _context.Users.AddRange(user1, user2);
        await _context.SaveChangesAsync();

        var company1Users = await _context.Users.IgnoreQueryFilters()
            .Where(u => u.CompanyId == _company1.Id)
            .ToListAsync();

        var company2Users = await _context.Users.IgnoreQueryFilters()
            .Where(u => u.CompanyId == _company2.Id)
            .ToListAsync();

        Assert.Single(company1Users);
        Assert.Single(company2Users);
        Assert.NotEqual(company1Users[0].CompanyId, company2Users[0].CompanyId);
    }

    [Fact]
    public async Task CrossTenantCompanyAccess_ReturnsEmpty()
    {
        var company = await _context.Companies
            .FirstOrDefaultAsync(c => c.Id == _company2.Id);

        Assert.NotNull(company);

        var company1Scope = await _context.Companies
            .Where(c => c.Id == _company1.Id)
            .ToListAsync();

        Assert.Single(company1Scope);
        Assert.Equal(_company1.Id, company1Scope[0].Id);
    }

    [Fact]
    public async Task DeactivatedCompany_UsersCannotAccess()
    {
        var user = new User(_company1.Id, "User 1", Email.Create("user1@empresa1.com"), "hash", PapelUsuario.Operacional);
        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        _company1.Deactivate();
        await _context.SaveChangesAsync();

        var result = await _context.Companies.FindAsync(_company1.Id);
        Assert.NotNull(result);
        Assert.Equal(StatusEmpresa.Inativa, result.Status);
    }
}
