using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Severina.Domain.Enums;
using Severina.Domain.Interfaces;

namespace Severina.Infrastructure.Services;

public class OverdueInvoiceDetectionService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<OverdueInvoiceDetectionService> _logger;
    private readonly TimeSpan _interval = TimeSpan.FromHours(24);

    public OverdueInvoiceDetectionService(IServiceProvider serviceProvider, ILogger<OverdueInvoiceDetectionService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("OverdueInvoiceDetectionService started. Interval: {Interval}", _interval);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await using var scope = _serviceProvider.CreateAsyncScope();
                var invoiceRepo = scope.ServiceProvider.GetRequiredService<IInvoiceRepository>();
                var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

                var companies = await GetCompanyIdsAsync(invoiceRepo, stoppingToken);

                foreach (var companyId in companies)
                {
                    var overdueInvoices = await invoiceRepo.GetOverdueInvoicesAsync(companyId);
                    var changed = false;

                    foreach (var invoice in overdueInvoices)
                    {
                        if (invoice.Status == InvoiceStatus.Overdue)
                            continue;

                        invoice.MarkOverdue();
                        changed = true;
                        _logger.LogInformation("Invoice {InvoiceId} (Company {CompanyId}) marked overdue. Vencimento: {Vencimento}",
                            invoice.Id, companyId, invoice.DataVencimento);
                    }

                    if (changed)
                    {
                        await unitOfWork.SaveChangesAsync(stoppingToken);
                    }
                }
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                _logger.LogError(ex, "Error during overdue invoice detection");
            }

            await Task.Delay(_interval, stoppingToken);
        }
    }

    private static async Task<List<Guid>> GetCompanyIdsAsync(IInvoiceRepository invoiceRepo, CancellationToken ct)
    {
        var allInvoices = await invoiceRepo.GetAllAsync();
        return allInvoices.Select(i => i.CompanyId).Distinct().ToList();
    }
}
