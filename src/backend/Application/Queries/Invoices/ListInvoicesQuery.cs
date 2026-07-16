using MediatR;
using Severina.Application.DTOs;
using Severina.Domain.Enums;
using Severina.Domain.Interfaces;

namespace Severina.Application.Queries.Invoices;

public record ListInvoicesQuery(
    Guid CompanyId,
    int Page = 1,
    int PageSize = 20,
    InvoiceStatus? Status = null,
    Guid? ClientId = null,
    DateTime? From = null,
    DateTime? To = null) : IRequest<PagedInvoiceResponse>;

public class ListInvoicesQueryHandler : IRequestHandler<ListInvoicesQuery, PagedInvoiceResponse>
{
    private readonly IInvoiceRepository _repository;

    public ListInvoicesQueryHandler(IInvoiceRepository repository)
    {
        _repository = repository;
    }

    public async Task<PagedInvoiceResponse> Handle(ListInvoicesQuery request, CancellationToken cancellationToken)
    {
        var items = await _repository.GetPagedAsync(
            request.CompanyId, request.Page, request.PageSize,
            request.Status, request.ClientId, request.From, request.To);

        var totalCount = await _repository.CountPagedAsync(
            request.CompanyId, request.Status, request.ClientId, request.From, request.To);

        var responses = items.Select(i => new InvoiceResponse(
            i.Id, i.CompanyId, i.ClientId, i.Numero, i.Valor, i.ValorPago,
            i.DataVencimento, i.DataPagamento, i.Descricao, i.Status,
            i.CreatedAt, i.UpdatedAt)).ToList();

        return new PagedInvoiceResponse(responses, totalCount, request.Page, request.PageSize);
    }
}
