using Severina.Application.DTOs;
using Severina.Application.Interfaces;
using Severina.Domain.Enums;
using Severina.Domain.Interfaces;

namespace Severina.Application.Services;

public class DashboardService : IDashboardService
{
    private readonly IAppointmentRepository _appointmentRepository;
    private readonly IUserRepository _userRepository;

    public DashboardService(
        IAppointmentRepository appointmentRepository,
        IUserRepository userRepository)
    {
        _appointmentRepository = appointmentRepository;
        _userRepository = userRepository;
    }

    public async Task<DashboardResponse> GetDashboardAsync(Guid companyId, CancellationToken cancellationToken = default)
    {
        var today = DateTime.UtcNow.Date;
        var monthStart = new DateTime(today.Year, today.Month, 1, 0, 0, 0, DateTimeKind.Utc);
        var previousMonthStart = monthStart.AddMonths(-1);
        var previousMonthEnd = monthStart.AddDays(-1);
        var next7Days = today.AddDays(7);

        var kpis = await GetKpisAsync(companyId, today, monthStart, previousMonthStart, previousMonthEnd, next7Days, cancellationToken);
        var charts = await GetChartsAsync(companyId, today, cancellationToken);
        var activities = await GetActivitiesAsync(companyId, cancellationToken);
        var pendingTasks = await GetPendingTasksAsync(companyId, today, next7Days, cancellationToken);

        return new DashboardResponse(kpis, charts, activities, pendingTasks);
    }

    private async Task<KpisDto> GetKpisAsync(
        Guid companyId, DateTime today, DateTime monthStart,
        DateTime previousMonthStart, DateTime previousMonthEnd,
        DateTime next7Days, CancellationToken cancellationToken)
    {
        var todayAppointments = await SafeExecuteAsync(
            ct => _appointmentRepository.GetByDateRangeAsync(companyId, today, today.AddDays(1)),
            "appointments-today", new List<Domain.Entities.Appointment>(), cancellationToken);

        var monthAppointments = await SafeExecuteAsync(
            ct => _appointmentRepository.GetByDateRangeAsync(companyId, monthStart, today.AddDays(1)),
            "appointments-month", new List<Domain.Entities.Appointment>(), cancellationToken);

        var previousMonthAppointments = await SafeExecuteAsync(
            ct => _appointmentRepository.GetByDateRangeAsync(companyId, previousMonthStart, previousMonthEnd),
            "appointments-prev-month", new List<Domain.Entities.Appointment>(), cancellationToken);

        var pendingAppointments = todayAppointments
            .Count(a => a.Status == StatusCompromisso.Scheduled || a.Status == StatusCompromisso.Confirmed);

        var completedAppointments = monthAppointments
            .Count(a => a.Status == StatusCompromisso.Completed);

        var previousCompleted = previousMonthAppointments
            .Count(a => a.Status == StatusCompromisso.Completed);

        var totalMonth = monthAppointments.Count;
        var conversionRate = totalMonth > 0 ? (decimal)completedAppointments / totalMonth * 100 : 0;

        var appointmentsTrend = CalculateTrend(completedAppointments, previousCompleted);

        var newClientsThisMonth = await SafeExecuteAsync(
            ct => _userRepository.GetByCompanyIdAsync(companyId),
            "users-count", new List<Domain.Entities.User>(), cancellationToken);

        var newClients = newClientsThisMonth.Count;

        var upcomingAppointments = await SafeExecuteAsync(
            ct => _appointmentRepository.GetByDateRangeAsync(companyId, DateTime.UtcNow, next7Days),
            "appointments-upcoming", new List<Domain.Entities.Appointment>(), cancellationToken);

        var compromissosHoje = todayAppointments.Count;

        return new KpisDto(
            AtendimentosHoje: todayAppointments.Count,
            AtendimentosPendentes: pendingAppointments,
            TaxaConversao: Math.Round(conversionRate, 1),
            TempoMedioResposta: 0,
            ClientesAtivos: newClients,
            NovosClientes: newClients,
            Faturamento: 0,
            Despesas: 0,
            Saldo: 0,
            CompromissosHoje: compromissosHoje,
            AtendimentosTrend: appointmentsTrend,
            FaturamentoTrend: null,
            ClientesTrend: null);
    }

    private async Task<ChartsDto> GetChartsAsync(Guid companyId, DateTime today, CancellationToken cancellationToken)
    {
        var barData = new List<BarChartItem>();
        var pieData = new List<PieChartItem>();
        var lineData = new List<LineChartItem>();

        for (int i = 29; i >= 0; i--)
        {
            var date = today.AddDays(-i);
            var dayStart = date;
            var dayEnd = date.AddDays(1);

            var dayAppointments = await SafeExecuteAsync(
                ct => _appointmentRepository.GetByDateRangeAsync(companyId, dayStart, dayEnd),
                $"chart-bar-{i}", new List<Domain.Entities.Appointment>(), cancellationToken);

            var label = date.ToString("dd/MM");
            var count = dayAppointments.Count;
            barData.Add(new BarChartItem(label, count));
            lineData.Add(new LineChartItem(label, count, null));
        }

        pieData.Add(new PieChartItem("Reunião", 40, "#2563eb"));
        pieData.Add(new PieChartItem("Follow-up", 25, "#22c55e"));
        pieData.Add(new PieChartItem("Lembrete", 20, "#f59e0b"));
        pieData.Add(new PieChartItem("Outro", 15, "#6b7280"));

        return new ChartsDto(barData, pieData, lineData);
    }

    private async Task<IReadOnlyList<ActivityDto>> GetActivitiesAsync(Guid companyId, CancellationToken cancellationToken)
    {
        var activities = new List<ActivityDto>();

        var recentAppointments = await SafeExecuteAsync(
            ct => _appointmentRepository.GetByDateRangeAsync(companyId, DateTime.UtcNow.AddDays(-7), DateTime.UtcNow.AddDays(1)),
            "activities-appointments", new List<Domain.Entities.Appointment>(), cancellationToken);

        foreach (var appointment in recentAppointments.OrderByDescending(a => a.CreatedAt).Take(10))
        {
            activities.Add(new ActivityDto(
                Id: appointment.Id,
                Type: "appointment",
                Description: $"Compromisso: {appointment.Titulo}",
                Timestamp: appointment.CreatedAt,
                SourceUrl: "/agenda"));
        }

        return activities.OrderByDescending(a => a.Timestamp).Take(10).ToList();
    }

    private async Task<IReadOnlyList<PendingTaskDto>> GetPendingTasksAsync(
        Guid companyId, DateTime today, DateTime next7Days, CancellationToken cancellationToken)
    {
        var tasks = new List<PendingTaskDto>();

        var upcomingAppointments = await SafeExecuteAsync(
            ct => _appointmentRepository.GetByDateRangeAsync(companyId, today, next7Days),
            "pending-appointments", new List<Domain.Entities.Appointment>(), cancellationToken);

        foreach (var appointment in upcomingAppointments
            .Where(a => a.Status == StatusCompromisso.Scheduled)
            .OrderBy(a => a.DataHoraInicio))
        {
            var priority = appointment.DataHoraInicio <= DateTime.UtcNow.AddHours(1)
                ? TaskPriority.Overdue
                : appointment.DataHoraInicio <= DateTime.UtcNow.AddHours(24)
                    ? TaskPriority.Pending
                    : TaskPriority.Upcoming;

            tasks.Add(new PendingTaskDto(
                Id: appointment.Id,
                Type: "appointment",
                Title: $"Compromisso: {appointment.Titulo}",
                Priority: priority,
                DueDate: appointment.DataHoraInicio,
                SourceUrl: "/agenda"));
        }

        return tasks.OrderBy(t => t.Priority).ThenBy(t => t.DueDate).ToList();
    }

    private static TrendDto? CalculateTrend(int current, int previous)
    {
        if (previous == 0 && current == 0) return null;
        if (previous == 0) return new TrendDto(100, TrendDirection.Up);

        var change = ((decimal)(current - previous) / previous) * 100;
        var direction = change > 0 ? TrendDirection.Up : change < 0 ? TrendDirection.Down : TrendDirection.Neutral;
        return new TrendDto(Math.Round(Math.Abs(change), 1), direction);
    }

    private static async Task<T> SafeExecuteAsync<T>(
        Func<CancellationToken, Task<T>> action,
        string moduleName,
        T fallback,
        CancellationToken cancellationToken)
    {
        try
        {
            using var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            cts.CancelAfter(TimeSpan.FromSeconds(2));
            return await action(cts.Token);
        }
        catch (OperationCanceledException)
        {
            return fallback;
        }
        catch
        {
            return fallback;
        }
    }
}
