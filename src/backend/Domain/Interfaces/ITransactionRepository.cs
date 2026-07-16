using Severina.Domain.Entities;
using Severina.Domain.Enums;

namespace Severina.Domain.Interfaces;

public interface ITransactionRepository : IRepository<Transaction>
{
    Task<IReadOnlyList<Transaction>> GetByDateRangeAsync(Guid companyId, DateTime from, DateTime to);
    Task<IReadOnlyList<Transaction>> GetByTypeAsync(Guid companyId, TransactionType tipo);
    Task<IReadOnlyList<Transaction>> GetByCategoryAsync(Guid companyId, TransactionCategory categoria);
    Task<IReadOnlyList<Transaction>> GetByClientIdAsync(Guid companyId, Guid clientId);
    Task<IReadOnlyList<Transaction>> GetPagedAsync(Guid companyId, int page, int pageSize, TransactionType? tipo = null, TransactionCategory? categoria = null, DateTime? from = null, DateTime? to = null, Guid? clientId = null);
    Task<int> CountPagedAsync(Guid companyId, TransactionType? tipo = null, TransactionCategory? categoria = null, DateTime? from = null, DateTime? to = null, Guid? clientId = null);
    Task<decimal> GetTotalApprovedByTypeAsync(Guid companyId, TransactionType tipo, DateTime from, DateTime to);
}
