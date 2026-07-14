using Severina.Domain.Interfaces;
using Severina.Infrastructure.Data;

namespace Severina.Infrastructure.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly SeverinaDbContext _context;
    private ICompanyRepository? _companies;
    private IUserRepository? _users;

    public UnitOfWork(SeverinaDbContext context)
    {
        _context = context;
    }

    public ICompanyRepository Companies =>
        _companies ??= new CompanyRepository(_context);

    public IUserRepository Users =>
        _users ??= new UserRepository(_context);

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}
