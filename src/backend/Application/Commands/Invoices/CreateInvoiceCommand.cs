using FluentValidation;
using MediatR;
using Severina.Application.DTOs;
using Severina.Application.Interfaces;
using Severina.Domain.Entities;
using Severina.Domain.Enums;
using Severina.Domain.Interfaces;

namespace Severina.Application.Commands.Invoices;

public record CreateInvoiceCommand(
    Guid CompanyId,
    decimal Valor,
    DateTime DataVencimento,
    Guid? ClientId,
    string? Descricao) : IRequest<InvoiceResponse>;

public class CreateInvoiceCommandValidator : AbstractValidator<CreateInvoiceCommand>
{
    public CreateInvoiceCommandValidator()
    {
        RuleFor(x => x.Valor)
            .GreaterThan(0).WithMessage("Valor deve ser maior que zero");

        RuleFor(x => x.DataVencimento)
            .NotEmpty().WithMessage("Data de vencimento é obrigatória");

        RuleFor(x => x.Descricao)
            .MaximumLength(500).WithMessage("Descrição deve ter no máximo 500 caracteres");
    }
}

public class CreateInvoiceCommandHandler : IRequestHandler<CreateInvoiceCommand, InvoiceResponse>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IFinancialCacheService _cacheService;

    public CreateInvoiceCommandHandler(IUnitOfWork unitOfWork, IFinancialCacheService cacheService)
    {
        _unitOfWork = unitOfWork;
        _cacheService = cacheService;
    }

    public async Task<InvoiceResponse> Handle(CreateInvoiceCommand request, CancellationToken cancellationToken)
    {
        var nextNumber = await _unitOfWork.Invoices.GetNextInvoiceNumberAsync(request.CompanyId);
        var numero = $"INV-{nextNumber:D6}";

        var invoice = new Invoice(
            request.CompanyId, request.Valor, request.DataVencimento,
            request.ClientId, request.Descricao, numero);

        await _unitOfWork.Invoices.AddAsync(invoice);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        await _cacheService.InvalidateDashboardAsync(request.CompanyId);

        return MapToResponse(invoice);
    }

    private static InvoiceResponse MapToResponse(Invoice i)
    {
        return new InvoiceResponse(
            i.Id, i.CompanyId, i.ClientId, i.Numero, i.Valor, i.ValorPago,
            i.DataVencimento, i.DataPagamento, i.Descricao, i.Status,
            i.CreatedAt, i.UpdatedAt);
    }
}
