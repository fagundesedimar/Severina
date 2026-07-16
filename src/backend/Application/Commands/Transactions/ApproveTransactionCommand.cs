using MediatR;
using Severina.Application.DTOs;
using Severina.Domain.Enums;
using Severina.Domain.Interfaces;
using Severina.Application.Interfaces;

namespace Severina.Application.Commands.Transactions;

public record ApproveTransactionCommand(Guid CompanyId, Guid Id) : IRequest<TransactionResponse>;

public class ApproveTransactionCommandHandler : IRequestHandler<ApproveTransactionCommand, TransactionResponse>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IFinancialCacheService _cacheService;

    public ApproveTransactionCommandHandler(IUnitOfWork unitOfWork, IFinancialCacheService cacheService)
    {
        _unitOfWork = unitOfWork;
        _cacheService = cacheService;
    }

    public async Task<TransactionResponse> Handle(ApproveTransactionCommand request, CancellationToken cancellationToken)
    {
        var transaction = await _unitOfWork.Transactions.GetByIdAsync(request.Id);
        if (transaction == null || transaction.CompanyId != request.CompanyId)
            throw new KeyNotFoundException("Transação não encontrada.");

        transaction.Approve();

        await _unitOfWork.Transactions.UpdateAsync(transaction);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        await _cacheService.InvalidateDashboardAsync(request.CompanyId);

        return new TransactionResponse(
            transaction.Id, transaction.CompanyId, transaction.ClientId,
            transaction.Tipo, transaction.Valor, transaction.Data,
            transaction.Categoria, transaction.Descricao, transaction.Status,
            transaction.CreatedAt, transaction.UpdatedAt);
    }
}
