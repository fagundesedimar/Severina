namespace Severina.Application.Interfaces;

public interface IExportService
{
    Task<byte[]> ExportTransactionsCsvAsync(IReadOnlyList<TransactionExportRow> rows, CancellationToken ct = default);
    Task<byte[]> ExportTransactionsXlsxAsync(IReadOnlyList<TransactionExportRow> rows, CancellationToken ct = default);
    Task<byte[]> ExportInvoicesCsvAsync(IReadOnlyList<InvoiceExportRow> rows, CancellationToken ct = default);
    Task<byte[]> ExportInvoicesXlsxAsync(IReadOnlyList<InvoiceExportRow> rows, CancellationToken ct = default);
}

public record TransactionExportRow(
    DateTime Data,
    string Tipo,
    decimal Valor,
    string Categoria,
    string? Descricao,
    string Status);

public record InvoiceExportRow(
    string Numero,
    DateTime DataVencimento,
    decimal Valor,
    decimal ValorPago,
    DateTime? DataPagamento,
    string? Descricao,
    string Status);
