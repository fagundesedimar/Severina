namespace Severina.Domain.ValueObjects;

public sealed class InteractionMetadata
{
    public string? Direction { get; }
    public int? DurationSeconds { get; }
    public string? Status { get; }
    public string? ContentPreview { get; }
    public bool? ReadStatus { get; }

    private InteractionMetadata(
        string? direction,
        int? durationSeconds,
        string? status,
        string? contentPreview,
        bool? readStatus)
    {
        Direction = direction;
        DurationSeconds = durationSeconds;
        Status = status;
        ContentPreview = contentPreview;
        ReadStatus = readStatus;
    }

    public static InteractionMetadata Create(
        string? direction = null,
        int? durationSeconds = null,
        string? status = null,
        string? contentPreview = null,
        bool? readStatus = null)
    {
        return new InteractionMetadata(direction, durationSeconds, status, contentPreview, readStatus);
    }
}
