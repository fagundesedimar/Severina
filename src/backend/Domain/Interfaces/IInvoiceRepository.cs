using Severina.Domain.Entities;
using Severina.Domain.Enums;

namespace Severina.Domain.Interfaces;

public interface IInvoiceRepository : IRepository<Invoice>
{
    Task<IReadOnlyList<Invoice>> GetByStatusAsync(Guid companyId, InvoiceStatus status);
    Task<IReadOnlyList<Invoice>> GetByClientIdAsync(Guid companyId, Guid clientId);
    Task<IReadOnlyList<Invoice>> GetByDateRangeAsync(Guid companyId, DateTime from, DateTime to);
    Task<IReadOnlyList<Invoice>> GetPagedAsync(Guid companyId, int page, int pageSize, InvoiceStatus? status = null, Guid? clientId = null, DateTime? from = null, DateTime? to = null);
    Task<int> CountPagedAsync(Guid companyId, InvoiceStatus? status = null, Guid? clientId = null, DateTime? from = null, DateTime? to = null);
    Task<IReadOnlyList<Invoice>> GetOverdueInvoicesAsync(Guid companyId);
    Task<int> GetNextInvoiceNumberAsync(Guid companyId);
}
