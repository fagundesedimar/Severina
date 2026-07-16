using Severina.Domain.Entities;

namespace Severina.Domain.Interfaces;

public interface IExportJobRepository : IRepository<ExportJob>
{
    Task<ExportJob?> GetByJobIdAsync(Guid companyId, Guid jobId);
}
