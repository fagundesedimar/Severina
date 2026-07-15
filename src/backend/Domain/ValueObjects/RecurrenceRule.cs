using System.Text.Json;

namespace Severina.Domain.ValueObjects;

public sealed class RecurrenceRule : IEquatable<RecurrenceRule>
{
    public string Tipo { get; }
    public int Intervalo { get; }
    public int[] DiasDaSemana { get; }
    public string FimTipo { get; }
    public DateTime? DataFim { get; }
    public int? ContagemFim { get; }

    private RecurrenceRule(string tipo, int intervalo, int[] diasDaSemana, string fimTipo, DateTime? dataFim, int? contagemFim)
    {
        Tipo = tipo;
        Intervalo = intervalo;
        DiasDaSemana = diasDaSemana;
        FimTipo = fimTipo;
        DataFim = dataFim;
        ContagemFim = contagemFim;
    }

    public static RecurrenceRule Create(string tipo, int intervalo = 1, int[]? diasDaSemana = null, string fimTipo = "date", DateTime? dataFim = null, int? contagemFim = null)
    {
        if (string.IsNullOrWhiteSpace(tipo))
            throw new ArgumentException("Tipo de recorrência é obrigatório.");

        if (intervalo < 1)
            throw new ArgumentException("Intervalo deve ser maior que zero.");

        if (fimTipo != "date" && fimTipo != "count")
            throw new ArgumentException("FimTipo deve ser 'date' ou 'count'.");

        if (fimTipo == "date" && dataFim is null)
            throw new ArgumentException("DataFim é obrigatória quando FimTipo é 'date'.");

        if (fimTipo == "count" && (contagemFim is null || contagemFim < 1))
            throw new ArgumentException("ContagemFim é obrigatória e deve ser maior que zero quando FimTipo é 'count'.");

        if (tipo == "weekly" && (diasDaSemana is null || diasDaSemana.Length == 0))
            throw new ArgumentException("DiasDaSemana é obrigatório para recorrência semanal.");

        return new RecurrenceRule(tipo, intervalo, diasDaSemana ?? Array.Empty<int>(), fimTipo, dataFim, contagemFim);
    }

    public string ToJson() => JsonSerializer.Serialize(this);

    public static RecurrenceRule FromJson(string json)
    {
        var data = JsonSerializer.Deserialize<RecurrenceRuleDto>(json);
        if (data is null)
            throw new ArgumentException("JSON de recorrência inválido.");

        return Create(data.Tipo, data.Intervalo, data.DiasDaSemana, data.FimTipo, data.DataFim, data.ContagemFim);
    }

    public bool Equals(RecurrenceRule? other)
    {
        if (other is null) return false;
        return ToJson() == other.ToJson();
    }

    public override bool Equals(object? obj) => Equals(obj as RecurrenceRule);
    public override int GetHashCode() => ToJson().GetHashCode();
    public override string ToString() => ToJson();

    private record RecurrenceRuleDto
    {
        public string Tipo { get; init; } = "none";
        public int Intervalo { get; init; } = 1;
        public int[] DiasDaSemana { get; init; } = Array.Empty<int>();
        public string FimTipo { get; init; } = "date";
        public DateTime? DataFim { get; init; }
        public int? ContagemFim { get; init; }
    }
}
