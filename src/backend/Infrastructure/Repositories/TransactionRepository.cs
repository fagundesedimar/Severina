using Microsoft.EntityFrameworkCore;
using Severina.Domain.Entities;
using Severina.Domain.Enums;
using Severina.Domain.Interfaces;
using Severina.Infrastructure.Data;

namespace Severina.Infrastructure.Repositories;

public class TransactionRepository : Repository<Transaction>, ITransactionRepository
{
    public TransactionRepository(SeverinaDbContext context) : base(context) { }

    public async Task<IReadOnlyList<Transaction>> GetByDateRangeAsync(Guid companyId, DateTime from, DateTime to)
    {
        return await _dbSet
            .Where(t => t.CompanyId == companyId && t.Data >= from && t.Data <= to)
            .OrderByDescending(t => t.Data)
            .ToListAsync();
    }

    public async Task<IReadOnlyList<Transaction>> GetByTypeAsync(Guid companyId, TransactionType tipo)
    {
        return await _dbSet
            .Where(t => t.CompanyId == companyId && t.Tipo == tipo)
            .OrderByDescending(t => t.Data)
            .ToListAsync();
    }

    public async Task<IReadOnlyList<Transaction>> GetByCategoryAsync(Guid companyId, TransactionCategory categoria)
    {
        return await _dbSet
            .Where(t => t.CompanyId == companyId && t.Categoria == categoria)
            .OrderByDescending(t => t.Data)
            .ToListAsync();
    }

    public async Task<IReadOnlyList<Transaction>> GetByClientIdAsync(Guid companyId, Guid clientId)
    {
        return await _dbSet
            .Where(t => t.CompanyId == companyId && t.ClientId == clientId)
            .OrderByDescending(t => t.Data)
            .ToListAsync();
    }

    public async Task<IReadOnlyList<Transaction>> GetPagedAsync(Guid companyId, int page, int pageSize, TransactionType? tipo = null, TransactionCategory? categoria = null, DateTime? from = null, DateTime? to = null, Guid? clientId = null)
    {
        var query = _dbSet.Where(t => t.CompanyId == companyId);

        if (tipo.HasValue) query = query.Where(t => t.Tipo == tipo.Value);
        if (categoria.HasValue) query = query.Where(t => t.Categoria == categoria.Value);
        if (from.HasValue) query = query.Where(t => t.Data >= from.Value);
        if (to.HasValue) query = query.Where(t => t.Data <= to.Value);
        if (clientId.HasValue) query = query.Where(t => t.ClientId == clientId.Value);

        return await query
            .OrderByDescending(t => t.Data)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task<int> CountPagedAsync(Guid companyId, TransactionType? tipo = null, TransactionCategory? categoria = null, DateTime? from = null, DateTime? to = null, Guid? clientId = null)
    {
        var query = _dbSet.Where(t => t.CompanyId == companyId);

        if (tipo.HasValue) query = query.Where(t => t.Tipo == tipo.Value);
        if (categoria.HasValue) query = query.Where(t => t.Categoria == categoria.Value);
        if (from.HasValue) query = query.Where(t => t.Data >= from.Value);
        if (to.HasValue) query = query.Where(t => t.Data <= to.Value);
        if (clientId.HasValue) query = query.Where(t => t.ClientId == clientId.Value);

        return await query.CountAsync();
    }

    public async Task<decimal> GetTotalApprovedByTypeAsync(Guid companyId, TransactionType tipo, DateTime from, DateTime to)
    {
        return await _dbSet
            .Where(t => t.CompanyId == companyId && t.Tipo == tipo && t.Status == TransactionStatus.Approved && t.Data >= from && t.Data <= to)
            .SumAsync(t => t.Valor);
    }
}
