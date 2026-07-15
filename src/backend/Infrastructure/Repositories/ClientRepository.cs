using Microsoft.EntityFrameworkCore;
using Severina.Domain.Entities;
using Severina.Domain.Interfaces;
using Severina.Infrastructure.Data;

namespace Severina.Infrastructure.Repositories;

public class ClientRepository : Repository<Client>, IClientRepository
{
    public ClientRepository(SeverinaDbContext context) : base(context) { }

    public async Task<IReadOnlyList<Client>> SearchAsync(Guid companyId, string searchTerm, int skip, int take)
    {
        var normalizedSearch = searchTerm.ToLower().Trim();

        return await _dbSet
            .Where(c => c.CompanyId == companyId &&
                (c.Nome.ToLower().Contains(normalizedSearch) ||
                 c.Empresa != null && c.Empresa.ToLower().Contains(normalizedSearch) ||
                 c.Email!.Value.ToLower().Contains(normalizedSearch)))
            .OrderByDescending(c => c.CreatedAt)
            .Skip(skip)
            .Take(take)
            .ToListAsync();
    }

    public async Task<int> CountSearchAsync(Guid companyId, string searchTerm)
    {
        var normalizedSearch = searchTerm.ToLower().Trim();

        return await _dbSet
            .Where(c => c.CompanyId == companyId &&
                (c.Nome.ToLower().Contains(normalizedSearch) ||
                 c.Empresa != null && c.Empresa.ToLower().Contains(normalizedSearch) ||
                 c.Email!.Value.ToLower().Contains(normalizedSearch)))
            .CountAsync();
    }

    public async Task<bool> ExistsByEmailAsync(Guid companyId, string email, Guid? excludeId = null)
    {
        var normalizedEmail = email.ToLower().Trim();

        return await _dbSet
            .AnyAsync(c => c.CompanyId == companyId &&
                c.Email != null && c.Email.Value == normalizedEmail &&
                (excludeId == null || c.Id != excludeId.Value));
    }

    public async Task<Client?> GetByIdWithTagsAsync(Guid companyId, Guid clientId)
    {
        return await _dbSet
            .FirstOrDefaultAsync(c => c.Id == clientId && c.CompanyId == companyId);
    }
}
