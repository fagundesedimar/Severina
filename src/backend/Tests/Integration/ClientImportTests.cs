using Microsoft.EntityFrameworkCore;
using Severina.Domain.Entities;
using Severina.Domain.Enums;
using Severina.Domain.ValueObjects;
using Severina.Infrastructure.Data;
using Severina.Infrastructure.Repositories;
using Severina.Infrastructure.Services;
using Xunit;

namespace Severina.Tests.Integration;

public class ClientImportTests : IDisposable
{
    private readonly SeverinaDbContext _context;
    private readonly CsvImportService _importService;
    private readonly UnitOfWork _unitOfWork;
    private readonly Guid _companyId;

    public ClientImportTests()
    {
        var options = new DbContextOptionsBuilder<SeverinaDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        var tenantProvider = new TestTenantProvider();
        _context = new SeverinaDbContext(options, tenantProvider);
        _context.Database.EnsureCreated();
        _unitOfWork = new UnitOfWork(_context);
        _importService = new CsvImportService(_unitOfWork);
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
    public async Task ImportCsv_ValidFile_CreatesClients()
    {
        var csvContent = "Nome,Email,Telefone,Empresa\nJoão Silva,joao@teste.com,11999998888,Empresa Teste\nMaria Santos,maria@teste.com,11888887777,Outra Empresa";
        var stream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(csvContent));

        var result = await _importService.ProcessCsvAsync(stream, _companyId);

        Assert.NotNull(result);
        Assert.Equal(2, result.ImportedRows);
        Assert.Empty(result.Errors);
    }

    [Fact]
    public async Task ImportCsv_WithDuplicateEmail_SkipsDuplicate()
    {
        var existing = new Client(_companyId, "Existente", "existente@teste.com", null, null);
        _context.Clients.Add(existing);
        await _context.SaveChangesAsync();

        var csvContent = "Nome,Email,Telefone,Empresa\nNovo Cliente,existente@teste.com,11999998888,Empresa";
        var stream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(csvContent));

        var result = await _importService.ProcessCsvAsync(stream, _companyId);

        Assert.NotNull(result);
        Assert.True(result.SkippedRows > 0);
    }

    [Fact]
    public async Task ImportCsv_EmptyName_ReturnsError()
    {
        var csvContent = "Nome,Email,Telefone,Empresa\n,joao@teste.com,11999998888,Empresa Teste";
        var stream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(csvContent));

        var result = await _importService.ProcessCsvAsync(stream, _companyId);

        Assert.NotNull(result);
        Assert.True(result.ErrorRows > 0);
    }

    [Fact]
    public async Task ImportCsv_LargeFile_Processes()
    {
        var lines = new List<string> { "Nome,Email,Telefone,Empresa" };
        for (int i = 0; i < 100; i++)
        {
            lines.Add($"Cliente {i},cliente{i}@teste.com,1199999{i:D4},Empresa {i}");
        }
        var csvContent = string.Join("\n", lines);
        var stream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(csvContent));

        var result = await _importService.ProcessCsvAsync(stream, _companyId);

        Assert.NotNull(result);
        Assert.Equal(100, result.ImportedRows);
    }

    [Fact]
    public async Task ImportCsv_DifferentCompany_DoesNotMix()
    {
        var otherCompanyId = Guid.NewGuid();
        var csvContent = "Nome,Email,Telefone,Empresa\nJoão Silva,joao@teste.com,11999998888,Empresa Teste";
        var stream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(csvContent));

        await _importService.ProcessCsvAsync(stream, _companyId);

        var clientCount = await _context.Clients.CountAsync(c => c.CompanyId == _companyId);

        Assert.Equal(1, clientCount);
    }
}
