namespace Severina.Domain.Interfaces;

public interface IUnitOfWork : IDisposable
{
    ICompanyRepository Companies { get; }
    IUserRepository Users { get; }
    IAppointmentRepository Appointments { get; }
    IClientRepository Clients { get; }
    IInteractionRepository Interactions { get; }
    IImportJobRepository ImportJobs { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
