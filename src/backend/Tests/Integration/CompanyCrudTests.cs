using Microsoft.EntityFrameworkCore;
using Severina.Domain.Entities;
using Severina.Domain.Enums;
using Severina.Domain.ValueObjects;
using Severina.Infrastructure.Data;
using Xunit;

namespace Severina.Tests.Integration;

public class CompanyCrudTests : IDisposable
{
    private readonly SeverinaDbContext _context;

    public CompanyCrudTests()
    {
        var options = new DbContextOptionsBuilder<SeverinaDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        var tenantProvider = new TestTenantProvider();
        _context = new SeverinaDbContext(options, tenantProvider);
        _context.Database.EnsureCreated();
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }

    [Fact]
    public async Task AddCompany_ReturnsCompany()
    {
        var company = new Company(
            "Empresa Teste",
            CnpjCpf.Create("52998224725"),
            Email.Create("teste@empresa.com"),
            TipoPessoa.Fisica);

        _context.Companies.Add(company);
        await _context.SaveChangesAsync();

        var result = await _context.Companies.FindAsync(company.Id);
        Assert.NotNull(result);
        Assert.Equal("Empresa Teste", result.Nome);
    }

    [Fact]
    public async Task UpdateCompany_UpdatesProperties()
    {
        var company = new Company(
            "Empresa Teste",
            CnpjCpf.Create("52998224725"),
            Email.Create("teste@empresa.com"),
            TipoPessoa.Fisica);

        _context.Companies.Add(company);
        await _context.SaveChangesAsync();

        company.Update("Nova Empresa", Email.Create("novo@empresa.com"), null);
        await _context.SaveChangesAsync();

        var result = await _context.Companies.FindAsync(company.Id);
        Assert.NotNull(result);
        Assert.Equal("Nova Empresa", result.Nome);
    }

    [Fact]
    public async Task DeactivateCompany_UpdatesStatus()
    {
        var company = new Company(
            "Empresa Teste",
            CnpjCpf.Create("52998224725"),
            Email.Create("teste@empresa.com"),
            TipoPessoa.Fisica);

        _context.Companies.Add(company);
        await _context.SaveChangesAsync();

        company.Deactivate();
        await _context.SaveChangesAsync();

        var result = await _context.Companies.FindAsync(company.Id);
        Assert.NotNull(result);
        Assert.Equal(StatusEmpresa.Inativa, result.Status);
    }

    [Fact]
    public async Task GetByCnpjCpf_ReturnsCompany()
    {
        var company = new Company(
            "Empresa Teste",
            CnpjCpf.Create("52998224725"),
            Email.Create("teste@empresa.com"),
            TipoPessoa.Fisica);

        _context.Companies.Add(company);
        await _context.SaveChangesAsync();

        var result = await _context.Companies
            .FirstOrDefaultAsync(c => c.CnpjCpf.Value == "52998224725");

        Assert.NotNull(result);
        Assert.Equal(company.Id, result.Id);
    }

    [Fact]
    public async Task GetByEmail_ReturnsCompany()
    {
        var company = new Company(
            "Empresa Teste",
            CnpjCpf.Create("52998224725"),
            Email.Create("teste@empresa.com"),
            TipoPessoa.Fisica);

        _context.Companies.Add(company);
        await _context.SaveChangesAsync();

        var result = await _context.Companies
            .FirstOrDefaultAsync(c => c.Email.Value == "teste@empresa.com");

        Assert.NotNull(result);
        Assert.Equal(company.Id, result.Id);
    }
}

public class TestTenantProvider : Application.Interfaces.ITenantProvider
{
    public Guid? CompanyId { get; private set; }

    public void SetCompanyId(Guid companyId)
    {
        CompanyId = companyId;
    }
}
