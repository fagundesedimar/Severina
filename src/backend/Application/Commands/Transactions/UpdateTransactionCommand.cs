using FluentValidation;
using MediatR;
using Severina.Application.DTOs;
using Severina.Domain.Enums;
using Severina.Domain.Interfaces;
using Severina.Application.Interfaces;

namespace Severina.Application.Commands.Transactions;

public record UpdateTransactionCommand(
    Guid CompanyId,
    Guid Id,
    TransactionType Tipo,
    decimal Valor,
    DateTime Data,
    TransactionCategory Categoria,
    Guid? ClientId,
    string? Descricao) : IRequest<TransactionResponse>;

public class UpdateTransactionCommandValidator : AbstractValidator<UpdateTransactionCommand>
{
    public UpdateTransactionCommandValidator()
    {
        RuleFor(x => x.Valor)
            .GreaterThan(0).WithMessage("Valor deve ser positivo");

        RuleFor(x => x.Data)
            .NotEmpty().WithMessage("Data é obrigatória");

        RuleFor(x => x.Descricao)
            .MaximumLength(500).WithMessage("Descrição deve ter no máximo 500 caracteres");
    }
}

public class UpdateTransactionCommandHandler : IRequestHandler<UpdateTransactionCommand, TransactionResponse>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IFinancialCacheService _cacheService;

    public UpdateTransactionCommandHandler(IUnitOfWork unitOfWork, IFinancialCacheService cacheService)
    {
        _unitOfWork = unitOfWork;
        _cacheService = cacheService;
    }

    public async Task<TransactionResponse> Handle(UpdateTransactionCommand request, CancellationToken cancellationToken)
    {
        var transaction = await _unitOfWork.Transactions.GetByIdAsync(request.Id);
        if (transaction == null || transaction.CompanyId != request.CompanyId)
            throw new KeyNotFoundException("Transação não encontrada.");

        transaction.UpdateDetails(request.Tipo, request.Valor, request.Data, request.Categoria, request.Descricao);

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
