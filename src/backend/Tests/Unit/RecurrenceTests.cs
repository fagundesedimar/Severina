using Severina.Domain.Enums;
using Severina.Domain.ValueObjects;
using Xunit;

namespace Severina.Tests.Unit;

public class RecurrenceRuleTests
{
    [Fact]
    public void Create_DailyRule_SetsProperties()
    {
        var rule = RecurrenceRule.Create("daily", 1, fimTipo: "date", dataFim: DateTime.UtcNow.AddDays(30));

        Assert.Equal("daily", rule.Tipo);
        Assert.Equal(1, rule.Intervalo);
        Assert.Equal("date", rule.FimTipo);
        Assert.NotNull(rule.DataFim);
    }

    [Fact]
    public void Create_WeeklyRuleWithDays_SetsProperties()
    {
        var days = new[] { 1, 3, 5 };
        var rule = RecurrenceRule.Create("weekly", 1, days, "date", DateTime.UtcNow.AddDays(30));

        Assert.Equal("weekly", rule.Tipo);
        Assert.Equal(days, rule.DiasDaSemana);
    }

    [Fact]
    public void Create_MonthlyRule_SetsProperties()
    {
        var rule = RecurrenceRule.Create("monthly", 1, fimTipo: "count", contagemFim: 12);

        Assert.Equal("monthly", rule.Tipo);
        Assert.Equal(12, rule.ContagemFim);
    }

    [Fact]
    public void Create_EmptyTipo_ThrowsException()
    {
        Assert.Throws<ArgumentException>(() =>
            RecurrenceRule.Create("", 1, fimTipo: "date", dataFim: DateTime.UtcNow.AddDays(30)));
    }

    [Fact]
    public void Create_InvalidInterval_ThrowsException()
    {
        Assert.Throws<ArgumentException>(() =>
            RecurrenceRule.Create("daily", 0, fimTipo: "date", dataFim: DateTime.UtcNow.AddDays(30)));
    }

    [Fact]
    public void Create_DateEndTypeWithoutDate_ThrowsException()
    {
        Assert.Throws<ArgumentException>(() =>
            RecurrenceRule.Create("daily", 1, fimTipo: "date", dataFim: null));
    }

    [Fact]
    public void Create_CountEndTypeWithoutCount_ThrowsException()
    {
        Assert.Throws<ArgumentException>(() =>
            RecurrenceRule.Create("daily", 1, fimTipo: "count", contagemFim: null));
    }

    [Fact]
    public void Create_WeeklyWithoutDays_ThrowsException()
    {
        Assert.Throws<ArgumentException>(() =>
            RecurrenceRule.Create("weekly", 1, diasDaSemana: null, fimTipo: "date", dataFim: DateTime.UtcNow.AddDays(30)));
    }

    [Fact]
    public void ToJson_And_FromJson_RoundTrips()
    {
        var rule = RecurrenceRule.Create("weekly", 2, new[] { 1, 3, 5 }, "date", DateTime.UtcNow.AddDays(60));
        var json = rule.ToJson();
        var restored = RecurrenceRule.FromJson(json);

        Assert.Equal(rule.Tipo, restored.Tipo);
        Assert.Equal(rule.Intervalo, restored.Intervalo);
        Assert.Equal(rule.DiasDaSemana, restored.DiasDaSemana);
        Assert.Equal(rule.FimTipo, restored.FimTipo);
    }
}

public class RecurrenceGeneratorTests
{
    [Fact]
    public void GenerateInstances_Daily_GeneratesCorrectCount()
    {
        var inicio = new DateTime(2026, 7, 14, 9, 0, 0, DateTimeKind.Utc);
        var fim = new DateTime(2026, 7, 14, 10, 0, 0, DateTimeKind.Utc);
        var rule = RecurrenceRule.Create("daily", 1, fimTipo: "count", contagemFim: 5);

        var instances = RecurrenceGenerator.GenerateInstances(inicio, fim, rule);

        Assert.Equal(5, instances.Count);
        Assert.Equal(inicio, instances[0]);
        Assert.Equal(inicio.AddDays(1), instances[1]);
        Assert.Equal(inicio.AddDays(4), instances[4]);
    }

    [Fact]
    public void GenerateInstances_Weekly_GeneratesCorrectCount()
    {
        var inicio = new DateTime(2026, 7, 13, 9, 0, 0, DateTimeKind.Utc);
        var fim = new DateTime(2026, 7, 13, 10, 0, 0, DateTimeKind.Utc);
        var rule = RecurrenceRule.Create("weekly", 1, new[] { 1, 3, 5 }, "count", contagemFim: 6);

        var instances = RecurrenceGenerator.GenerateInstances(inicio, fim, rule);

        Assert.Equal(6, instances.Count);
        Assert.Equal(inicio, instances[0]);
    }

    [Fact]
    public void GenerateInstances_Monthly_GeneratesCorrectMonths()
    {
        var inicio = new DateTime(2026, 7, 15, 9, 0, 0, DateTimeKind.Utc);
        var fim = new DateTime(2026, 7, 15, 10, 0, 0, DateTimeKind.Utc);
        var rule = RecurrenceRule.Create("monthly", 1, fimTipo: "count", contagemFim: 3);

        var instances = RecurrenceGenerator.GenerateInstances(inicio, fim, rule);

        Assert.Equal(3, instances.Count);
        Assert.Equal(7, instances[0].Month);
        Assert.Equal(8, instances[1].Month);
        Assert.Equal(9, instances[2].Month);
    }

    [Fact]
    public void HasConflict_Overlapping_ReturnsTrue()
    {
        var existing = new List<(DateTime Inicio, DateTime Fim)>
        {
            (new DateTime(2026, 7, 14, 10, 0, 0), new DateTime(2026, 7, 14, 11, 0, 0))
        };

        var result = RecurrenceGenerator.HasConflict(
            new DateTime(2026, 7, 14, 10, 30, 0),
            new DateTime(2026, 7, 14, 11, 30, 0),
            existing);

        Assert.True(result);
    }

    [Fact]
    public void HasConflict_NoOverlap_ReturnsFalse()
    {
        var existing = new List<(DateTime Inicio, DateTime Fim)>
        {
            (new DateTime(2026, 7, 14, 10, 0, 0), new DateTime(2026, 7, 14, 11, 0, 0))
        };

        var result = RecurrenceGenerator.HasConflict(
            new DateTime(2026, 7, 14, 11, 0, 0),
            new DateTime(2026, 7, 14, 12, 0, 0),
            existing);

        Assert.False(result);
    }
}
