namespace Severina.Domain.Interfaces;

public interface IUnitOfWork : IDisposable
{
    ICompanyRepository Companies { get; }
    IUserRepository Users { get; }
    IAppointmentRepository Appointments { get; }
    IClientRepository Clients { get; }
    IInteractionRepository Interactions { get; }
    IImportJobRepository ImportJobs { get; }
    ITransactionRepository Transactions { get; }
    IInvoiceRepository Invoices { get; }
    IExportJobRepository ExportJobs { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
