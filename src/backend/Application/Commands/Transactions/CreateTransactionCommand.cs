using FluentValidation;
using MediatR;
using Severina.Application.DTOs;
using Severina.Domain.Entities;
using Severina.Domain.Enums;
using Severina.Domain.Interfaces;
using Severina.Application.Interfaces;

namespace Severina.Application.Commands.Transactions;

public record CreateTransactionCommand(
    Guid CompanyId,
    TransactionType Tipo,
    decimal Valor,
    DateTime Data,
    TransactionCategory Categoria,
    Guid? ClientId,
    string? Descricao) : IRequest<TransactionResponse>;

public class CreateTransactionCommandValidator : AbstractValidator<CreateTransactionCommand>
{
    public CreateTransactionCommandValidator()
    {
        RuleFor(x => x.Valor)
            .GreaterThan(0).WithMessage("Valor deve ser positivo");

        RuleFor(x => x.Data)
            .NotEmpty().WithMessage("Data é obrigatória");

        RuleFor(x => x.Descricao)
            .MaximumLength(500).WithMessage("Descrição deve ter no máximo 500 caracteres");
    }
}

public class CreateTransactionCommandHandler : IRequestHandler<CreateTransactionCommand, TransactionResponse>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IFinancialCacheService _cacheService;

    public CreateTransactionCommandHandler(IUnitOfWork unitOfWork, IFinancialCacheService cacheService)
    {
        _unitOfWork = unitOfWork;
        _cacheService = cacheService;
    }

    public async Task<TransactionResponse> Handle(CreateTransactionCommand request, CancellationToken cancellationToken)
    {
        var transaction = new Transaction(
            request.CompanyId, request.Tipo, request.Valor, request.Data,
            request.Categoria, request.ClientId, request.Descricao);

        await _unitOfWork.Transactions.AddAsync(transaction);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        await _cacheService.InvalidateDashboardAsync(request.CompanyId);

        return MapToResponse(transaction);
    }

    private static TransactionResponse MapToResponse(Transaction t)
    {
        return new TransactionResponse(
            t.Id, t.CompanyId, t.ClientId, t.Tipo, t.Valor, t.Data,
            t.Categoria, t.Descricao, t.Status, t.CreatedAt, t.UpdatedAt);
    }
}
