using System.Globalization;
using System.Text;
using CsvHelper;
using CsvHelper.Configuration;
using Severina.Application.Interfaces;
using Severina.Domain.Entities;
using Severina.Domain.Interfaces;

namespace Severina.Infrastructure.Services;

public class CsvImportService : IImportService
{
    private readonly IUnitOfWork _unitOfWork;
    private const int MaxRows = 1000;

    public CsvImportService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<ImportResult> ProcessCsvAsync(Stream fileStream, Guid companyId, CancellationToken cancellationToken = default)
    {
        var errors = new List<ImportRowError>();
        int imported = 0, skipped = 0, total = 0;

        using var reader = new StreamReader(fileStream, Encoding.UTF8);
        using var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            HasHeaderRecord = true,
            MissingFieldFound = null,
            BadDataFound = null
        });

        await csv.ReadAsync();
        csv.ReadHeader();

        var clients = new List<Domain.Entities.Client>();

        while (await csv.ReadAsync())
        {
            total++;
            if (total > MaxRows)
            {
                errors.Add(new ImportRowError(total, "Arquivo excede limite de 1000 registros"));
                break;
            }

            var nome = csv.GetField("Nome")?.Trim();
            var email = csv.GetField("Email")?.Trim();
            var telefone = csv.GetField("Telefone")?.Trim();
            var empresa = csv.GetField("Empresa")?.Trim();

            if (string.IsNullOrWhiteSpace(nome))
            {
                errors.Add(new ImportRowError(total, "Nome é obrigatório"));
                continue;
            }

            if (!string.IsNullOrWhiteSpace(email))
            {
                var exists = await _unitOfWork.Clients.ExistsByEmailAsync(companyId, email);
                if (exists)
                {
                    skipped++;
                    continue;
                }
            }

            try
            {
                var client = new Domain.Entities.Client(companyId, nome, email, telefone, empresa);
                clients.Add(client);
                imported++;
            }
            catch (Exception ex)
            {
                errors.Add(new ImportRowError(total, ex.Message));
            }
        }

        if (clients.Count > 0)
        {
            foreach (var client in clients)
            {
                await _unitOfWork.Clients.AddAsync(client);
            }
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        return new ImportResult(total, imported, skipped, errors.Count(e => e.Message.Contains("obrigatório") || e.Message.Contains("inválido")), errors);
    }
}
