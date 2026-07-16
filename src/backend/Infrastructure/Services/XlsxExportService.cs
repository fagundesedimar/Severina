using ClosedXML.Excel;
using Severina.Application.Interfaces;

namespace Severina.Infrastructure.Services;

public class XlsxExportService : IExportService
{
    public Task<byte[]> ExportTransactionsCsvAsync(IReadOnlyList<TransactionExportRow> rows, CancellationToken ct = default)
    {
        throw new NotImplementedException("Use CsvExportService for CSV exports");
    }

    public Task<byte[]> ExportInvoicesCsvAsync(IReadOnlyList<InvoiceExportRow> rows, CancellationToken ct = default)
    {
        throw new NotImplementedException("Use CsvExportService for CSV exports");
    }

    public Task<byte[]> ExportTransactionsXlsxAsync(IReadOnlyList<TransactionExportRow> rows, CancellationToken ct = default)
    {
        using var workbook = new XLWorkbook();
        var worksheet = workbook.Worksheets.Add("Transações");

        worksheet.Cell(1, 1).Value = "Data";
        worksheet.Cell(1, 2).Value = "Tipo";
        worksheet.Cell(1, 3).Value = "Valor";
        worksheet.Cell(1, 4).Value = "Categoria";
        worksheet.Cell(1, 5).Value = "Descrição";
        worksheet.Cell(1, 6).Value = "Status";

        for (int i = 0; i < rows.Count; i++)
        {
            var row = rows[i];
            worksheet.Cell(i + 2, 1).Value = row.Data;
            worksheet.Cell(i + 2, 2).Value = row.Tipo;
            worksheet.Cell(i + 2, 3).Value = row.Valor;
            worksheet.Cell(i + 2, 3).Style.NumberFormat.Format = "#,##0.00";
            worksheet.Cell(i + 2, 4).Value = row.Categoria;
            worksheet.Cell(i + 2, 5).Value = row.Descricao ?? "";
            worksheet.Cell(i + 2, 6).Value = row.Status;
        }

        worksheet.Columns().AdjustToContents();

        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        return Task.FromResult(stream.ToArray());
    }

    public Task<byte[]> ExportInvoicesXlsxAsync(IReadOnlyList<InvoiceExportRow> rows, CancellationToken ct = default)
    {
        using var workbook = new XLWorkbook();
        var worksheet = workbook.Worksheets.Add("Faturas");

        worksheet.Cell(1, 1).Value = "Número";
        worksheet.Cell(1, 2).Value = "Vencimento";
        worksheet.Cell(1, 3).Value = "Valor";
        worksheet.Cell(1, 4).Value = "Pago";
        worksheet.Cell(1, 5).Value = "Data Pagamento";
        worksheet.Cell(1, 6).Value = "Descrição";
        worksheet.Cell(1, 7).Value = "Status";

        for (int i = 0; i < rows.Count; i++)
        {
            var row = rows[i];
            worksheet.Cell(i + 2, 1).Value = row.Numero;
            worksheet.Cell(i + 2, 2).Value = row.DataVencimento;
            worksheet.Cell(i + 2, 3).Value = row.Valor;
            worksheet.Cell(i + 2, 3).Style.NumberFormat.Format = "#,##0.00";
            worksheet.Cell(i + 2, 4).Value = row.ValorPago;
            worksheet.Cell(i + 2, 4).Style.NumberFormat.Format = "#,##0.00";
            worksheet.Cell(i + 2, 5).Value = row.DataPagamento?.ToString("dd/MM/yyyy") ?? "";
            worksheet.Cell(i + 2, 6).Value = row.Descricao ?? "";
            worksheet.Cell(i + 2, 7).Value = row.Status;
        }

        worksheet.Columns().AdjustToContents();

        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        return Task.FromResult(stream.ToArray());
    }
}
