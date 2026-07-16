namespace Severina.Domain.Entities;

public class ExportJob : BaseEntity
{
    public Guid CompanyId { get; set; }
    public string FileName { get; set; } = string.Empty;
    public string Format { get; set; } = string.Empty;
    public int TotalRows { get; set; }
    public int ProcessedRows { get; set; }
    public string Status { get; set; } = "Processing";
    public string? FilePath { get; set; }
    public string? ErrorMessage { get; set; }
}
