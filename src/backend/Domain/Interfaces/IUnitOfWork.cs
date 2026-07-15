namespace Severina.Domain.Interfaces;

public interface IUnitOfWork : IDisposable
{
    ICompanyRepository Companies { get; }
    IUserRepository Users { get; }
    IAppointmentRepository Appointments { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
