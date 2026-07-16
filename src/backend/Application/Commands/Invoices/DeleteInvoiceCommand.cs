using MediatR;
using Severina.Domain.Enums;
using Severina.Domain.Interfaces;
using Severina.Application.Interfaces;

namespace Severina.Application.Commands.Invoices;

public record DeleteInvoiceCommand(Guid CompanyId, Guid Id) : IRequest<Unit>;

public class DeleteInvoiceCommandHandler : IRequestHandler<DeleteInvoiceCommand, Unit>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IFinancialCacheService _cacheService;

    public DeleteInvoiceCommandHandler(IUnitOfWork unitOfWork, IFinancialCacheService cacheService)
    {
        _unitOfWork = unitOfWork;
        _cacheService = cacheService;
    }

    public async Task<Unit> Handle(DeleteInvoiceCommand request, CancellationToken cancellationToken)
    {
        var invoice = await _unitOfWork.Invoices.GetByIdAsync(request.Id);
        if (invoice == null || invoice.CompanyId != request.CompanyId)
            throw new KeyNotFoundException("Cobrança não encontrada.");

        invoice.SoftDelete();

        await _unitOfWork.Invoices.UpdateAsync(invoice);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        await _cacheService.InvalidateDashboardAsync(request.CompanyId);

        return Unit.Value;
    }
}
