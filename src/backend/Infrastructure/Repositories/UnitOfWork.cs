using Severina.Domain.Interfaces;
using Severina.Infrastructure.Data;

namespace Severina.Infrastructure.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly SeverinaDbContext _context;
    private ICompanyRepository? _companies;
    private IUserRepository? _users;
    private IAppointmentRepository? _appointments;
    private IClientRepository? _clients;
    private IInteractionRepository? _interactions;
    private IImportJobRepository? _importJobs;
    private ITransactionRepository? _transactions;
    private IInvoiceRepository? _invoices;
    private IExportJobRepository? _exportJobs;

    public UnitOfWork(SeverinaDbContext context)
    {
        _context = context;
    }

    public ICompanyRepository Companies =>
        _companies ??= new CompanyRepository(_context);

    public IUserRepository Users =>
        _users ??= new UserRepository(_context);

    public IAppointmentRepository Appointments =>
        _appointments ??= new AppointmentRepository(_context);

    public IClientRepository Clients =>
        _clients ??= new ClientRepository(_context);

    public IInteractionRepository Interactions =>
        _interactions ??= new InteractionRepository(_context);

    public IImportJobRepository ImportJobs =>
        _importJobs ??= new ImportJobRepository(_context);

    public ITransactionRepository Transactions =>
        _transactions ??= new TransactionRepository(_context);

    public IInvoiceRepository Invoices =>
        _invoices ??= new InvoiceRepository(_context);

    public IExportJobRepository ExportJobs =>
        _exportJobs ??= new ExportJobRepository(_context);

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}
