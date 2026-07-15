using Microsoft.EntityFrameworkCore;
using Severina.Domain.Entities;
using Severina.Domain.Interfaces;
using Severina.Infrastructure.Data;

namespace Severina.Infrastructure.Repositories;

public class ImportJobRepository : Repository<ImportJob>, IImportJobRepository
{
    public ImportJobRepository(SeverinaDbContext context) : base(context) { }

    public async Task<ImportJob?> GetByJobIdAsync(Guid companyId, Guid jobId)
    {
        return await _dbSet
            .FirstOrDefaultAsync(j => j.Id == jobId && j.CompanyId == companyId);
    }

    public async Task<IReadOnlyList<ImportJob>> GetByCompanyIdAsync(Guid companyId)
    {
        return await _dbSet
            .Where(j => j.CompanyId == companyId)
            .OrderByDescending(j => j.CreatedAt)
            .ToListAsync();
    }
}
