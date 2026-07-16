using MediatR;
using Severina.Domain.Enums;
using Severina.Domain.Interfaces;
using Severina.Application.Interfaces;

namespace Severina.Application.Commands.Invoices;

public record CancelInvoiceCommand(Guid CompanyId, Guid Id) : IRequest<Unit>;

public class CancelInvoiceCommandHandler : IRequestHandler<CancelInvoiceCommand, Unit>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IFinancialCacheService _cacheService;

    public CancelInvoiceCommandHandler(IUnitOfWork unitOfWork, IFinancialCacheService cacheService)
    {
        _unitOfWork = unitOfWork;
        _cacheService = cacheService;
    }

    public async Task<Unit> Handle(CancelInvoiceCommand request, CancellationToken cancellationToken)
    {
        var invoice = await _unitOfWork.Invoices.GetByIdAsync(request.Id);
        if (invoice == null || invoice.CompanyId != request.CompanyId)
            throw new KeyNotFoundException("Cobrança não encontrada.");

        invoice.Cancel();

        await _unitOfWork.Invoices.UpdateAsync(invoice);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        await _cacheService.InvalidateDashboardAsync(request.CompanyId);

        return Unit.Value;
    }
}
