using Microsoft.EntityFrameworkCore;
using Severina.Domain.Entities;
using Severina.Domain.Interfaces;
using Severina.Infrastructure.Data;

namespace Severina.Infrastructure.Repositories;

public class ExportJobRepository : Repository<ExportJob>, IExportJobRepository
{
    public ExportJobRepository(SeverinaDbContext context) : base(context) { }

    public async Task<ExportJob?> GetByJobIdAsync(Guid companyId, Guid jobId)
    {
        return await _dbSet
            .FirstOrDefaultAsync(j => j.Id == jobId && j.CompanyId == companyId);
    }
}
