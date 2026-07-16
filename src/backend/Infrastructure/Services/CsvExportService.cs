using CsvHelper;
using CsvHelper.Configuration;
using System.Globalization;
using Severina.Application.Interfaces;

namespace Severina.Infrastructure.Services;

public class CsvExportService : IExportService
{
    public Task<byte[]> ExportTransactionsCsvAsync(IReadOnlyList<TransactionExportRow> rows, CancellationToken ct = default)
    {
        using var writer = new StringWriter();
        using var csv = new CsvWriter(writer, new CsvConfiguration(CultureInfo.GetCultureInfo("pt-BR"))
        {
            HasHeaderRecord = true
        });

        csv.WriteRecords(rows);
        return Task.FromResult(System.Text.Encoding.UTF8.GetBytes(writer.ToString()));
    }

    public Task<byte[]> ExportInvoicesCsvAsync(IReadOnlyList<InvoiceExportRow> rows, CancellationToken ct = default)
    {
        using var writer = new StringWriter();
        using var csv = new CsvWriter(writer, new CsvConfiguration(CultureInfo.GetCultureInfo("pt-BR"))
        {
            HasHeaderRecord = true
        });

        csv.WriteRecords(rows);
        return Task.FromResult(System.Text.Encoding.UTF8.GetBytes(writer.ToString()));
    }

    public Task<byte[]> ExportTransactionsXlsxAsync(IReadOnlyList<TransactionExportRow> rows, CancellationToken ct = default)
    {
        throw new NotImplementedException("Use XlsxExportService for XLSX exports");
    }

    public Task<byte[]> ExportInvoicesXlsxAsync(IReadOnlyList<InvoiceExportRow> rows, CancellationToken ct = default)
    {
        throw new NotImplementedException("Use XlsxExportService for XLSX exports");
    }
}
