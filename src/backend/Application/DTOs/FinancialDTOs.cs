using Severina.Domain.Enums;

namespace Severina.Application.DTOs;

public record TransactionResponse(
    Guid Id,
    Guid CompanyId,
    Guid? ClientId,
    TransactionType Tipo,
    decimal Valor,
    DateTime Data,
    TransactionCategory Categoria,
    string? Descricao,
    TransactionStatus Status,
    DateTime CreatedAt,
    DateTime UpdatedAt);

public record CreateTransactionRequest(
    TransactionType Tipo,
    decimal Valor,
    DateTime Data,
    TransactionCategory Categoria,
    Guid? ClientId,
    string? Descricao);

public record UpdateTransactionRequest(
    TransactionType Tipo,
    decimal Valor,
    DateTime Data,
    TransactionCategory Categoria,
    Guid? ClientId,
    string? Descricao);

public record RejectTransactionRequest(string Motivo);

public record PagedTransactionResponse(
    IReadOnlyList<TransactionResponse> Items,
    int TotalCount,
    int Page,
    int PageSize);

public record InvoiceResponse(
    Guid Id,
    Guid CompanyId,
    Guid? ClientId,
    string Numero,
    decimal Valor,
    decimal ValorPago,
    DateTime DataVencimento,
    DateTime? DataPagamento,
    string? Descricao,
    InvoiceStatus Status,
    DateTime CreatedAt,
    DateTime UpdatedAt);

public record CreateInvoiceRequest(
    decimal Valor,
    DateTime DataVencimento,
    Guid? ClientId,
    string? Descricao);

public record UpdateInvoiceRequest(
    decimal Valor,
    DateTime DataVencimento,
    Guid? ClientId,
    string? Descricao);

public record PayInvoiceRequest(
    decimal ValorPago,
    DateTime DataPagamento);

public record PagedInvoiceResponse(
    IReadOnlyList<InvoiceResponse> Items,
    int TotalCount,
    int Page,
    int PageSize);

public record FinancialDashboardResponse(
    FinancialKpisDto Kpis,
    FinancialChartsDto Charts,
    IReadOnlyList<TransactionResponse> RecentTransactions);

public record FinancialKpisDto(
    decimal Saldo,
    decimal ReceitasMes,
    decimal DespesasMes,
    decimal PrevisaoProximoMes,
    int ContasPendentes,
    int ContasAtrasadas);

public record FinancialChartsDto(
    IReadOnlyList<MonthlyDataDto> MonthlyData,
    IReadOnlyList<CategoryDataDto> CategoryData);

public record MonthlyDataDto(string Mes, decimal Receitas, decimal Despesas);

public record CategoryDataDto(string Categoria, decimal Valor, decimal Percentual);

public record ExportResult(byte[] FileBytes, string ContentType, string FileName);
