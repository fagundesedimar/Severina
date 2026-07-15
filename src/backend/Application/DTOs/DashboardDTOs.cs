namespace Severina.Application.DTOs;

public record DashboardResponse(
    KpisDto Kpis,
    ChartsDto Charts,
    IReadOnlyList<ActivityDto> Activities,
    IReadOnlyList<PendingTaskDto> PendingTasks);

public record KpisDto(
    int AtendimentosHoje,
    int AtendimentosPendentes,
    decimal TaxaConversao,
    decimal TempoMedioResposta,
    int ClientesAtivos,
    int NovosClientes,
    decimal Faturamento,
    decimal Despesas,
    decimal Saldo,
    int CompromissosHoje,
    TrendDto? AtendimentosTrend,
    TrendDto? FaturamentoTrend,
    TrendDto? ClientesTrend);

public record TrendDto(
    decimal Value,
    TrendDirection Direction);

public enum TrendDirection
{
    Up,
    Down,
    Neutral
}

public record ChartsDto(
    IReadOnlyList<BarChartItem> BarData,
    IReadOnlyList<PieChartItem> PieData,
    IReadOnlyList<LineChartItem> LineData);

public record BarChartItem(
    string Label,
    int Value);

public record PieChartItem(
    string Label,
    int Value,
    string Color);

public record LineChartItem(
    string Label,
    int Value,
    int? PreviousValue);

public record ActivityDto(
    Guid Id,
    string Type,
    string Description,
    DateTime Timestamp,
    string? SourceUrl);

public record PendingTaskDto(
    Guid Id,
    string Type,
    string Title,
    TaskPriority Priority,
    DateTime? DueDate,
    string? SourceUrl);

public enum TaskPriority
{
    Overdue,
    Pending,
    Upcoming
}
