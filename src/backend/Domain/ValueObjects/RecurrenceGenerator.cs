using Severina.Domain.Enums;

namespace Severina.Domain.ValueObjects;

public static class RecurrenceGenerator
{
    public static List<DateTime> GenerateInstances(
        DateTime dataHoraInicio,
        DateTime dataHoraFim,
        RecurrenceRule rule,
        DateTime? rangeStart = null,
        DateTime? rangeEnd = null)
    {
        var duracao = dataHoraFim - dataHoraInicio;
        var instances = new List<DateTime>();
        var effectiveStart = rangeStart ?? dataHoraInicio;
        var effectiveEnd = rangeEnd ?? rule.DataFim ?? DateTime.UtcNow.AddYears(1);
        var maxCount = rule.ContagemFim ?? int.MaxValue;

        var current = dataHoraInicio;
        var count = 0;

        while (current <= effectiveEnd && count < maxCount)
        {
            if (current >= effectiveStart)
            {
                instances.Add(current);
            }

            count++;
            var next = GetNextOccurrence(current, rule);

            if (next is null)
                break;

            current = next.Value;
        }

        return instances;
    }

    private static DateTime? GetNextOccurrence(DateTime current, RecurrenceRule rule)
    {
        return rule.Tipo.ToLowerInvariant() switch
        {
            "daily" => current.AddDays(rule.Intervalo),
            "weekly" => GetNextWeeklyOccurrence(current, rule),
            "monthly" => GetNextMonthlyOccurrence(current, rule),
            "custom" => GetNextCustomOccurrence(current, rule),
            _ => null
        };
    }

    private static DateTime? GetNextWeeklyOccurrence(DateTime current, RecurrenceRule rule)
    {
        if (rule.DiasDaSemana.Length == 0)
            return null;

        var nextDate = current.AddDays(1);
        var weekStart = current.Date;
        var weekEnd = weekStart.AddDays(7 * rule.Intervalo);

        for (var date = nextDate; date < weekEnd; date = date.AddDays(1))
        {
            if (Array.Exists(rule.DiasDaSemana, d => d == (int)date.DayOfWeek))
            {
                return date + current.TimeOfDay;
            }
        }

        return weekEnd + current.TimeOfDay;
    }

    private static DateTime? GetNextMonthlyOccurrence(DateTime current, RecurrenceRule rule)
    {
        var nextMonth = current.AddMonths(rule.Intervalo);
        var day = Math.Min(rule.DiasDaSemana.Length > 0 ? rule.DiasDaSemana[0] : current.Day, DateTime.DaysInMonth(nextMonth.Year, nextMonth.Month));
        return new DateTime(nextMonth.Year, nextMonth.Month, day, current.Hour, current.Minute, 0, DateTimeKind.Utc);
    }

    private static DateTime? GetNextCustomOccurrence(DateTime current, RecurrenceRule rule)
    {
        return rule.Tipo.ToLowerInvariant() switch
        {
            "days" => current.AddDays(rule.Intervalo),
            "weeks" => current.AddDays(7 * rule.Intervalo),
            "months" => current.AddMonths(rule.Intervalo),
            _ => current.AddDays(rule.Intervalo)
        };
    }

    public static bool HasConflict(DateTime dataHoraInicio, DateTime dataHoraFim, List<(DateTime Inicio, DateTime Fim)> existingAppointments)
    {
        return existingAppointments.Any(a =>
            dataHoraInicio < a.Fim && dataHoraFim > a.Inicio);
    }

    public static List<(DateTime Inicio, DateTime Fim)> GenerateInstanceRanges(
        DateTime dataHoraInicio,
        DateTime dataHoraFim,
        RecurrenceRule rule,
        DateTime? rangeStart = null,
        DateTime? rangeEnd = null)
    {
        var instances = GenerateInstances(dataHoraInicio, dataHoraFim, rule, rangeStart, rangeEnd);
        var duracao = dataHoraFim - dataHoraInicio;

        return instances.Select(i => (i, i + duracao)).ToList();
    }
}
