using MediatR;
using Severina.Application.DTOs;
using Severina.Domain.Interfaces;

namespace Severina.Application.Queries.Invoices;

public record GetInvoiceByIdQuery(Guid CompanyId, Guid Id) : IRequest<InvoiceResponse?>;

public class GetInvoiceByIdQueryHandler : IRequestHandler<GetInvoiceByIdQuery, InvoiceResponse?>
{
    private readonly IInvoiceRepository _repository;

    public GetInvoiceByIdQueryHandler(IInvoiceRepository repository)
    {
        _repository = repository;
    }

    public async Task<InvoiceResponse?> Handle(GetInvoiceByIdQuery request, CancellationToken cancellationToken)
    {
        var invoice = await _repository.GetByIdAsync(request.Id);
        if (invoice == null || invoice.CompanyId != request.CompanyId)
            return null;

        return new InvoiceResponse(
            invoice.Id, invoice.CompanyId, invoice.ClientId, invoice.Numero,
            invoice.Valor, invoice.ValorPago, invoice.DataVencimento, invoice.DataPagamento,
            invoice.Descricao, invoice.Status, invoice.CreatedAt, invoice.UpdatedAt);
    }
}
