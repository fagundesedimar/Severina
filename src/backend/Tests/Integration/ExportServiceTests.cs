using Severina.Application.Interfaces;
using Severina.Infrastructure.Services;
using Xunit;

namespace Severina.Tests.Integration;

public class ExportServiceTests
{
    private readonly IExportService _csvService;
    private readonly IExportService _xlsxService;

    public ExportServiceTests()
    {
        _csvService = new CsvExportService();
        _xlsxService = new XlsxExportService();
    }

    [Fact]
    public async Task ExportTransactionsCsv_ReturnsBytes()
    {
        var rows = new List<TransactionExportRow>
        {
            new(DateTime.UtcNow, "Receita", 500m, "Servicos", "Descrição teste", "Approved"),
            new(DateTime.UtcNow.AddDays(-1), "Despesa", 200m, "Materiais", null, "Pending"),
        };

        var result = await _csvService.ExportTransactionsCsvAsync(rows);

        Assert.NotNull(result);
        Assert.NotEmpty(result);

        var csv = System.Text.Encoding.UTF8.GetString(result);
        Assert.Contains("Receita", csv);
        Assert.Contains("Despesa", csv);
        Assert.Contains("500", csv);
    }

    [Fact]
    public async Task ExportInvoicesCsv_ReturnsBytes()
    {
        var rows = new List<InvoiceExportRow>
        {
            new("INV-001", DateTime.UtcNow.AddDays(30), 1000m, 0m, null, "Fatura teste", "Pending"),
            new("INV-002", DateTime.UtcNow.AddDays(-5), 500m, 500m, DateTime.UtcNow, "Paga", "Paid"),
        };

        var result = await _csvService.ExportInvoicesCsvAsync(rows);

        Assert.NotNull(result);
        Assert.NotEmpty(result);

        var csv = System.Text.Encoding.UTF8.GetString(result);
        Assert.Contains("INV-001", csv);
        Assert.Contains("INV-002", csv);
    }

    [Fact]
    public async Task ExportTransactionsXlsx_ReturnsBytes()
    {
        var rows = new List<TransactionExportRow>
        {
            new(DateTime.UtcNow, "Receita", 500m, "Servicos", "Descrição", "Approved"),
        };

        var result = await _xlsxService.ExportTransactionsXlsxAsync(rows);

        Assert.NotNull(result);
        Assert.NotEmpty(result);
        Assert.True(result.Length > 100);
    }

    [Fact]
    public async Task ExportInvoicesXlsx_ReturnsBytes()
    {
        var rows = new List<InvoiceExportRow>
        {
            new("INV-001", DateTime.UtcNow.AddDays(30), 1000m, 0m, null, "Fatura", "Pending"),
        };

        var result = await _xlsxService.ExportInvoicesXlsxAsync(rows);

        Assert.NotNull(result);
        Assert.NotEmpty(result);
        Assert.True(result.Length > 100);
    }

    [Fact]
    public async Task ExportTransactionsCsv_EmptyList_ReturnsHeaderOnly()
    {
        var rows = new List<TransactionExportRow>();
        var result = await _csvService.ExportTransactionsCsvAsync(rows);

        Assert.NotNull(result);
        var csv = System.Text.Encoding.UTF8.GetString(result);
        Assert.Contains("Data", csv);
    }
}
