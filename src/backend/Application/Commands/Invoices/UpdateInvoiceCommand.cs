using FluentValidation;
using MediatR;
using Severina.Application.DTOs;
using Severina.Domain.Enums;
using Severina.Domain.Interfaces;
using Severina.Application.Interfaces;

namespace Severina.Application.Commands.Invoices;

public record UpdateInvoiceCommand(
    Guid CompanyId,
    Guid Id,
    decimal Valor,
    DateTime DataVencimento,
    Guid? ClientId,
    string? Descricao) : IRequest<InvoiceResponse>;

public class UpdateInvoiceCommandValidator : AbstractValidator<UpdateInvoiceCommand>
{
    public UpdateInvoiceCommandValidator()
    {
        RuleFor(x => x.Valor)
            .GreaterThan(0).WithMessage("Valor deve ser maior que zero");

        RuleFor(x => x.DataVencimento)
            .NotEmpty().WithMessage("Data de vencimento é obrigatória");
    }
}

public class UpdateInvoiceCommandHandler : IRequestHandler<UpdateInvoiceCommand, InvoiceResponse>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IFinancialCacheService _cacheService;

    public UpdateInvoiceCommandHandler(IUnitOfWork unitOfWork, IFinancialCacheService cacheService)
    {
        _unitOfWork = unitOfWork;
        _cacheService = cacheService;
    }

    public async Task<InvoiceResponse> Handle(UpdateInvoiceCommand request, CancellationToken cancellationToken)
    {
        var invoice = await _unitOfWork.Invoices.GetByIdAsync(request.Id);
        if (invoice == null || invoice.CompanyId != request.CompanyId)
            throw new KeyNotFoundException("Cobrança não encontrada.");

        if (invoice.Status == InvoiceStatus.Paid)
            throw new InvalidOperationException("Cobrança paga não pode ser alterada.");

        invoice.UpdateDetails(request.Valor, request.DataVencimento, request.ClientId, request.Descricao);

        await _unitOfWork.Invoices.UpdateAsync(invoice);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        await _cacheService.InvalidateDashboardAsync(request.CompanyId);

        return new InvoiceResponse(
            invoice.Id, invoice.CompanyId, invoice.ClientId, invoice.Numero,
            invoice.Valor, invoice.ValorPago, invoice.DataVencimento, invoice.DataPagamento,
            invoice.Descricao, invoice.Status, invoice.CreatedAt, invoice.UpdatedAt);
    }
}
