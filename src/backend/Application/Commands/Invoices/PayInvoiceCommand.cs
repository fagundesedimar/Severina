using FluentValidation;
using MediatR;
using Severina.Application.DTOs;
using Severina.Domain.Enums;
using Severina.Domain.Interfaces;
using Severina.Application.Interfaces;

namespace Severina.Application.Commands.Invoices;

public record PayInvoiceCommand(Guid CompanyId, Guid Id, decimal ValorPago, DateTime DataPagamento) : IRequest<InvoiceResponse>;

public class PayInvoiceCommandValidator : AbstractValidator<PayInvoiceCommand>
{
    public PayInvoiceCommandValidator()
    {
        RuleFor(x => x.ValorPago)
            .GreaterThan(0).WithMessage("Valor pago deve ser maior que zero");

        RuleFor(x => x.DataPagamento)
            .NotEmpty().WithMessage("Data de pagamento é obrigatória");
    }
}

public class PayInvoiceCommandHandler : IRequestHandler<PayInvoiceCommand, InvoiceResponse>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IFinancialCacheService _cacheService;

    public PayInvoiceCommandHandler(IUnitOfWork unitOfWork, IFinancialCacheService cacheService)
    {
        _unitOfWork = unitOfWork;
        _cacheService = cacheService;
    }

    public async Task<InvoiceResponse> Handle(PayInvoiceCommand request, CancellationToken cancellationToken)
    {
        var invoice = await _unitOfWork.Invoices.GetByIdAsync(request.Id);
        if (invoice == null || invoice.CompanyId != request.CompanyId)
            throw new KeyNotFoundException("Cobrança não encontrada.");

        invoice.Pay(request.ValorPago, request.DataPagamento);

        await _unitOfWork.Invoices.UpdateAsync(invoice);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        await _cacheService.InvalidateDashboardAsync(request.CompanyId);

        return new InvoiceResponse(
            invoice.Id, invoice.CompanyId, invoice.ClientId, invoice.Numero,
            invoice.Valor, invoice.ValorPago, invoice.DataVencimento, invoice.DataPagamento,
            invoice.Descricao, invoice.Status, invoice.CreatedAt, invoice.UpdatedAt);
    }
}
