using MediatR;
using Severina.Application.DTOs;
using Severina.Application.Interfaces;
using Severina.Domain.Enums;
using Severina.Domain.Interfaces;

namespace Severina.Application.Commands.Invoices;

public record ExportInvoicesCommand(
    Guid CompanyId,
    string Format,
    InvoiceStatus? Status) : IRequest<ExportResult>;

public class ExportInvoicesCommandHandler : IRequestHandler<ExportInvoicesCommand, ExportResult>
{
    private readonly IInvoiceRepository _invoiceRepo;
    private readonly IExportService _exportService;

    public ExportInvoicesCommandHandler(IInvoiceRepository invoiceRepo, IExportService exportService)
    {
        _invoiceRepo = invoiceRepo;
        _exportService = exportService;
    }

    public async Task<ExportResult> Handle(ExportInvoicesCommand request, CancellationToken cancellationToken)
    {
        IReadOnlyList<Domain.Entities.Invoice> invoices;

        if (request.Status.HasValue)
            invoices = await _invoiceRepo.GetByStatusAsync(request.CompanyId, request.Status.Value);
        else
            invoices = await _invoiceRepo.GetPagedAsync(request.CompanyId, 1, int.MaxValue);

        var rows = invoices.Select(inv => new InvoiceExportRow(
            inv.Numero,
            inv.DataVencimento,
            inv.Valor,
            inv.ValorPago,
            inv.DataPagamento,
            inv.Descricao,
            inv.Status.ToString()
        )).ToList();

        var format = request.Format.ToLowerInvariant();
        var fileBytes = format == "xlsx"
            ? await _exportService.ExportInvoicesXlsxAsync(rows, cancellationToken)
            : await _exportService.ExportInvoicesCsvAsync(rows, cancellationToken);

        var extension = format == "xlsx" ? "xlsx" : "csv";
        var contentType = format == "xlsx"
            ? "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"
            : "text/csv";

        return new ExportResult(fileBytes, contentType, $"faturas.{extension}");
    }
}
