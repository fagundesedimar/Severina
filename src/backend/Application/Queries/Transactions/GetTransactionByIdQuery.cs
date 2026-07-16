using MediatR;
using Severina.Application.DTOs;
using Severina.Domain.Interfaces;

namespace Severina.Application.Queries.Transactions;

public record GetTransactionByIdQuery(Guid CompanyId, Guid Id) : IRequest<TransactionResponse?>;

public class GetTransactionByIdQueryHandler : IRequestHandler<GetTransactionByIdQuery, TransactionResponse?>
{
    private readonly ITransactionRepository _repository;

    public GetTransactionByIdQueryHandler(ITransactionRepository repository)
    {
        _repository = repository;
    }

    public async Task<TransactionResponse?> Handle(GetTransactionByIdQuery request, CancellationToken cancellationToken)
    {
        var transaction = await _repository.GetByIdAsync(request.Id);
        if (transaction == null || transaction.CompanyId != request.CompanyId)
            return null;

        return new TransactionResponse(
            transaction.Id, transaction.CompanyId, transaction.ClientId,
            transaction.Tipo, transaction.Valor, transaction.Data,
            transaction.Categoria, transaction.Descricao, transaction.Status,
            transaction.CreatedAt, transaction.UpdatedAt);
    }
}
