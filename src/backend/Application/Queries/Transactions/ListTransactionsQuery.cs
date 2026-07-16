using MediatR;
using Severina.Application.DTOs;
using Severina.Domain.Enums;
using Severina.Domain.Interfaces;

namespace Severina.Application.Queries.Transactions;

public record ListTransactionsQuery(
    Guid CompanyId,
    int Page = 1,
    int PageSize = 20,
    TransactionType? Tipo = null,
    TransactionCategory? Categoria = null,
    DateTime? From = null,
    DateTime? To = null,
    Guid? ClientId = null) : IRequest<PagedTransactionResponse>;

public class ListTransactionsQueryHandler : IRequestHandler<ListTransactionsQuery, PagedTransactionResponse>
{
    private readonly ITransactionRepository _repository;

    public ListTransactionsQueryHandler(ITransactionRepository repository)
    {
        _repository = repository;
    }

    public async Task<PagedTransactionResponse> Handle(ListTransactionsQuery request, CancellationToken cancellationToken)
    {
        var items = await _repository.GetPagedAsync(
            request.CompanyId, request.Page, request.PageSize,
            request.Tipo, request.Categoria, request.From, request.To, request.ClientId);

        var totalCount = await _repository.CountPagedAsync(
            request.CompanyId, request.Tipo, request.Categoria, request.From, request.To, request.ClientId);

        var responses = items.Select(t => new TransactionResponse(
            t.Id, t.CompanyId, t.ClientId, t.Tipo, t.Valor, t.Data,
            t.Categoria, t.Descricao, t.Status, t.CreatedAt, t.UpdatedAt)).ToList();

        return new PagedTransactionResponse(responses, totalCount, request.Page, request.PageSize);
    }
}
