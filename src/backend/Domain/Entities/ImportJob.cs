namespace Severina.Domain.Entities;

public class ImportJob : BaseEntity
{
    public Guid CompanyId { get; private set; }
    public string FileName { get; private set; } = string.Empty;
    public int TotalRows { get; private set; }
    public int ProcessedRows { get; private set; }
    public int ImportedRows { get; private set; }
    public int SkippedRows { get; private set; }
    public int ErrorRows { get; private set; }
    public ImportJobStatus Status { get; set; }
    public string? ErrorMessage { get; private set; }

    private ImportJob() { }

    public ImportJob(Guid companyId, string fileName, int totalRows)
    {
        CompanyId = companyId;
        FileName = fileName;
        TotalRows = totalRows;
        Status = ImportJobStatus.Processing;
    }

    public void UpdateProgress(int processed, int imported, int skipped, int errors)
    {
        ProcessedRows = processed;
        ImportedRows = imported;
        SkippedRows = skipped;
        ErrorRows = errors;
        UpdateTimestamp();
    }

    public void Complete()
    {
        Status = ImportJobStatus.Completed;
        UpdateTimestamp();
    }

    public void Fail(string errorMessage)
    {
        Status = ImportJobStatus.Failed;
        ErrorMessage = errorMessage;
        UpdateTimestamp();
    }
}

public enum ImportJobStatus
{
    Processing = 0,
    Completed = 1,
    Failed = 2
}
