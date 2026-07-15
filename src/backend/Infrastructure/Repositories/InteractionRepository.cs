using Microsoft.EntityFrameworkCore;
using Severina.Domain.Entities;
using Severina.Domain.Enums;
using Severina.Domain.Interfaces;
using Severina.Infrastructure.Data;

namespace Severina.Infrastructure.Repositories;

public class InteractionRepository : Repository<Interaction>, IInteractionRepository
{
    public InteractionRepository(SeverinaDbContext context) : base(context) { }

    public async Task<IReadOnlyList<Interaction>> GetByClientIdAsync(Guid companyId, Guid clientId, int skip, int take)
    {
        return await _dbSet
            .Where(i => i.CompanyId == companyId && i.ClientId == clientId)
            .OrderByDescending(i => i.CreatedAt)
            .Skip(skip)
            .Take(take)
            .ToListAsync();
    }

    public async Task<int> CountByClientIdAsync(Guid companyId, Guid clientId)
    {
        return await _dbSet
            .CountAsync(i => i.CompanyId == companyId && i.ClientId == clientId);
    }

    public async Task<IReadOnlyList<Interaction>> GetByClientIdAndTypeAsync(Guid companyId, Guid clientId, InteractionType type, int skip, int take)
    {
        return await _dbSet
            .Where(i => i.CompanyId == companyId && i.ClientId == clientId && i.Type == type)
            .OrderByDescending(i => i.CreatedAt)
            .Skip(skip)
            .Take(take)
            .ToListAsync();
    }

    public async Task<int> CountByClientIdAndTypeAsync(Guid companyId, Guid clientId, InteractionType type)
    {
        return await _dbSet
            .CountAsync(i => i.CompanyId == companyId && i.ClientId == clientId && i.Type == type);
    }
}
