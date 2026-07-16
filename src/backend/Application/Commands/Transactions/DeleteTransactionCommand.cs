using MediatR;
using Severina.Domain.Enums;
using Severina.Domain.Interfaces;
using Severina.Application.Interfaces;

namespace Severina.Application.Commands.Transactions;

public record DeleteTransactionCommand(Guid CompanyId, Guid Id) : IRequest<Unit>;

public class DeleteTransactionCommandHandler : IRequestHandler<DeleteTransactionCommand, Unit>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IFinancialCacheService _cacheService;

    public DeleteTransactionCommandHandler(IUnitOfWork unitOfWork, IFinancialCacheService cacheService)
    {
        _unitOfWork = unitOfWork;
        _cacheService = cacheService;
    }

    public async Task<Unit> Handle(DeleteTransactionCommand request, CancellationToken cancellationToken)
    {
        var transaction = await _unitOfWork.Transactions.GetByIdAsync(request.Id);
        if (transaction == null || transaction.CompanyId != request.CompanyId)
            throw new KeyNotFoundException("Transação não encontrada.");

        if (transaction.Status == TransactionStatus.Approved)
            throw new InvalidOperationException("Transação aprovada não pode ser removida.");

        transaction.SoftDelete();

        await _unitOfWork.Transactions.UpdateAsync(transaction);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        await _cacheService.InvalidateDashboardAsync(request.CompanyId);

        return Unit.Value;
    }
}
