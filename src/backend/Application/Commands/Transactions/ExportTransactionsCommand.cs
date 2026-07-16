using MediatR;
using Severina.Application.DTOs;
using Severina.Application.Interfaces;
using Severina.Domain.Enums;
using Severina.Domain.Interfaces;

namespace Severina.Application.Commands.Transactions;

public record ExportTransactionsCommand(
    Guid CompanyId,
    string Format,
    DateTime? From,
    DateTime? To,
    TransactionType? Tipo,
    TransactionCategory? Categoria) : IRequest<ExportResult>;

public class ExportTransactionsCommandHandler : IRequestHandler<ExportTransactionsCommand, ExportResult>
{
    private readonly ITransactionRepository _transactionRepo;
    private readonly IExportService _exportService;

    public ExportTransactionsCommandHandler(ITransactionRepository transactionRepo, IExportService exportService)
    {
        _transactionRepo = transactionRepo;
        _exportService = exportService;
    }

    public async Task<ExportResult> Handle(ExportTransactionsCommand request, CancellationToken cancellationToken)
    {
        IReadOnlyList<Domain.Entities.Transaction> transactions;

        if (request.From.HasValue && request.To.HasValue)
            transactions = await _transactionRepo.GetByDateRangeAsync(request.CompanyId, request.From.Value, request.To.Value);
        else if (request.Tipo.HasValue)
            transactions = await _transactionRepo.GetByTypeAsync(request.CompanyId, request.Tipo.Value);
        else if (request.Categoria.HasValue)
            transactions = await _transactionRepo.GetByCategoryAsync(request.CompanyId, request.Categoria.Value);
        else
            transactions = await _transactionRepo.GetByDateRangeAsync(request.CompanyId, DateTime.MinValue, DateTime.MaxValue);

        var rows = transactions.Select(t => new TransactionExportRow(
            t.Data,
            t.Tipo.ToString(),
            t.Valor,
            t.Categoria.ToString(),
            t.Descricao,
            t.Status.ToString()
        )).ToList();

        var format = request.Format.ToLowerInvariant();
        var fileBytes = format == "xlsx"
            ? await _exportService.ExportTransactionsXlsxAsync(rows, cancellationToken)
            : await _exportService.ExportTransactionsCsvAsync(rows, cancellationToken);

        var extension = format == "xlsx" ? "xlsx" : "csv";
        var contentType = format == "xlsx"
            ? "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"
            : "text/csv";

        return new ExportResult(fileBytes, contentType, $"transacoes.{extension}");
    }
}
