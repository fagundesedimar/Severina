using Microsoft.EntityFrameworkCore;
using Severina.Domain.Entities;
using Severina.Domain.Enums;
using Severina.Domain.Interfaces;
using Severina.Infrastructure.Data;

namespace Severina.Infrastructure.Repositories;

public class InvoiceRepository : Repository<Invoice>, IInvoiceRepository
{
    public InvoiceRepository(SeverinaDbContext context) : base(context) { }

    public async Task<IReadOnlyList<Invoice>> GetByStatusAsync(Guid companyId, InvoiceStatus status)
    {
        return await _dbSet
            .Where(i => i.CompanyId == companyId && i.Status == status)
            .OrderBy(i => i.DataVencimento)
            .ToListAsync();
    }

    public async Task<IReadOnlyList<Invoice>> GetByClientIdAsync(Guid companyId, Guid clientId)
    {
        return await _dbSet
            .Where(i => i.CompanyId == companyId && i.ClientId == clientId)
            .OrderBy(i => i.DataVencimento)
            .ToListAsync();
    }

    public async Task<IReadOnlyList<Invoice>> GetByDateRangeAsync(Guid companyId, DateTime from, DateTime to)
    {
        return await _dbSet
            .Where(i => i.CompanyId == companyId && i.DataVencimento >= from && i.DataVencimento <= to)
            .OrderBy(i => i.DataVencimento)
            .ToListAsync();
    }

    public async Task<IReadOnlyList<Invoice>> GetPagedAsync(Guid companyId, int page, int pageSize, InvoiceStatus? status = null, Guid? clientId = null, DateTime? from = null, DateTime? to = null)
    {
        var query = _dbSet.Where(i => i.CompanyId == companyId);

        if (status.HasValue) query = query.Where(i => i.Status == status.Value);
        if (clientId.HasValue) query = query.Where(i => i.ClientId == clientId.Value);
        if (from.HasValue) query = query.Where(i => i.DataVencimento >= from.Value);
        if (to.HasValue) query = query.Where(i => i.DataVencimento <= to.Value);

        return await query
            .OrderBy(i => i.DataVencimento)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task<int> CountPagedAsync(Guid companyId, InvoiceStatus? status = null, Guid? clientId = null, DateTime? from = null, DateTime? to = null)
    {
        var query = _dbSet.Where(i => i.CompanyId == companyId);

        if (status.HasValue) query = query.Where(i => i.Status == status.Value);
        if (clientId.HasValue) query = query.Where(i => i.ClientId == clientId.Value);
        if (from.HasValue) query = query.Where(i => i.DataVencimento >= from.Value);
        if (to.HasValue) query = query.Where(i => i.DataVencimento <= to.Value);

        return await query.CountAsync();
    }

    public async Task<IReadOnlyList<Invoice>> GetOverdueInvoicesAsync(Guid companyId)
    {
        return await _dbSet
            .Where(i => i.CompanyId == companyId && i.Status == InvoiceStatus.Pending && i.DataVencimento < DateTime.UtcNow)
            .ToListAsync();
    }

    public async Task<int> GetNextInvoiceNumberAsync(Guid companyId)
    {
        var count = await _dbSet
            .CountAsync(i => i.CompanyId == companyId);
        return count + 1;
    }
}
