using MediatR;
using Severina.Application.DTOs;
using Severina.Application.Interfaces;
using Severina.Domain.Entities;
using Severina.Domain.Interfaces;

namespace Severina.Application.Commands.Clients;

public record ImportClientsCommand(
    Guid CompanyId,
    Stream FileStream,
    string FileName) : IRequest<ImportJobResponse>;

public class ImportClientsCommandHandler : IRequestHandler<ImportClientsCommand, ImportJobResponse>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IImportService _importService;

    public ImportClientsCommandHandler(IUnitOfWork unitOfWork, IImportService importService)
    {
        _unitOfWork = unitOfWork;
        _importService = importService;
    }

    public async Task<ImportJobResponse> Handle(ImportClientsCommand request, CancellationToken cancellationToken)
    {
        var result = await _importService.ProcessCsvAsync(request.FileStream, request.CompanyId, cancellationToken);

        var job = new ImportJob(request.CompanyId, request.FileName, result.TotalRows);
        await _unitOfWork.ImportJobs.AddAsync(job);

        if (result.ErrorRows > 0 || result.SkippedRows > 0)
        {
            var errors = string.Join("; ", result.Errors.Take(5).Select(e => $"Linha {e.Row}: {e.Message}"));
            if (result.ErrorRows > 0)
                job.Fail(errors);
            else
                job.Complete();
        }
        else
        {
            job.Complete();
        }

        job.UpdateProgress(result.TotalRows, result.ImportedRows, result.SkippedRows, result.ErrorRows);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new ImportJobResponse(
            job.Id, job.FileName, job.TotalRows, job.ProcessedRows,
            job.ImportedRows, job.SkippedRows, job.ErrorRows,
            job.Status, job.ErrorMessage, job.CreatedAt);
    }
}
