namespace Severina.Application.Interfaces;

public interface IImportService
{
    Task<ImportResult> ProcessCsvAsync(Stream fileStream, Guid companyId, CancellationToken cancellationToken = default);
}

public record ImportResult(
    int TotalRows,
    int ImportedRows,
    int SkippedRows,
    int ErrorRows,
    List<ImportRowError> Errors);

public record ImportRowError(int Row, string Message);
