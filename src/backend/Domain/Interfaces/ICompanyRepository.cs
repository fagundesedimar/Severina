using Severina.Domain.Entities;

namespace Severina.Domain.Interfaces;

public interface ICompanyRepository : IRepository<Company>
{
    Task<Company?> GetByCnpjCpfAsync(string cnpjCpf);
    Task<Company?> GetByEmailAsync(string email);
}

public interface IUserRepository : IRepository<User>
{
    Task<User?> GetByEmailAsync(Guid companyId, string email);
    Task<User?> GetByEmailAsync(string email);
    Task<IReadOnlyList<User>> GetByCompanyIdAsync(Guid companyId);
}
