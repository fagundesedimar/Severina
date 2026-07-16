using Microsoft.EntityFrameworkCore;
using Severina.Domain.Entities;
using Severina.Domain.Interfaces;
using Severina.Infrastructure.Data;

namespace Severina.Infrastructure.Repositories;

public class CompanyRepository : Repository<Company>, ICompanyRepository
{
    public CompanyRepository(SeverinaDbContext context) : base(context) { }

    public async Task<Company?> GetByCnpjCpfAsync(string cnpjCpf)
    {
        return await _dbSet.FirstOrDefaultAsync(c => c.CnpjCpf == cnpjCpf);
    }

    public async Task<Company?> GetByEmailAsync(string email)
    {
        return await _dbSet.FirstOrDefaultAsync(c => c.Email == email);
    }
}

public class UserRepository : Repository<User>, IUserRepository
{
    public UserRepository(SeverinaDbContext context) : base(context) { }

    public async Task<User?> GetByEmailAsync(Guid companyId, string email)
    {
        return await _dbSet.FirstOrDefaultAsync(u => u.CompanyId == companyId && u.Email == email);
    }

    public async Task<User?> GetByEmailAsync(string email)
    {
        return await _dbSet.IgnoreQueryFilters().FirstOrDefaultAsync(u => u.Email == email);
    }

    public async Task<IReadOnlyList<User>> GetByCompanyIdAsync(Guid companyId)
    {
        return await _dbSet.Where(u => u.CompanyId == companyId).ToListAsync();
    }
}
