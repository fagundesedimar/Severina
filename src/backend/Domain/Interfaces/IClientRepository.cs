using Severina.Domain.Entities;

namespace Severina.Domain.Interfaces;

public interface IClientRepository : IRepository<Client>
{
    Task<IReadOnlyList<Client>> SearchAsync(Guid companyId, string searchTerm, int skip, int take);
    Task<int> CountSearchAsync(Guid companyId, string searchTerm);
    Task<bool> ExistsByEmailAsync(Guid companyId, string email, Guid? excludeId = null);
    Task<Client?> GetByIdWithTagsAsync(Guid companyId, Guid clientId);
}

public interface IInteractionRepository : IRepository<Interaction>
{
    Task<IReadOnlyList<Interaction>> GetByClientIdAsync(Guid companyId, Guid clientId, int skip, int take);
    Task<int> CountByClientIdAsync(Guid companyId, Guid clientId);
    Task<IReadOnlyList<Interaction>> GetByClientIdAndTypeAsync(Guid companyId, Guid clientId, Enums.InteractionType type, int skip, int take);
    Task<int> CountByClientIdAndTypeAsync(Guid companyId, Guid clientId, Enums.InteractionType type);
}

public interface IImportJobRepository : IRepository<ImportJob>
{
    Task<ImportJob?> GetByJobIdAsync(Guid companyId, Guid jobId);
    Task<IReadOnlyList<ImportJob>> GetByCompanyIdAsync(Guid companyId);
}
