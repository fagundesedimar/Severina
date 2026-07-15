using System.Text.RegularExpressions;

namespace Severina.Domain.ValueObjects;

public sealed class Telefone : IEquatable<Telefone>
{
    private static readonly Regex PhoneRegex = new(
        @"^\d{10,11}$",
        RegexOptions.Compiled);

    public string Value { get; }

    private Telefone(string value)
    {
        Value = value;
    }

    public static Telefone Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Telefone é obrigatório.");

        var digits = Regex.Replace(value, @"\D", "");

        if (!PhoneRegex.IsMatch(digits))
            throw new ArgumentException("Formato de telefone inválido.");

        return new Telefone(digits);
    }

    public static bool TryCreate(string value, out Telefone? result)
    {
        try
        {
            result = Create(value);
            return true;
        }
        catch
        {
            result = null;
            return false;
        }
    }

    public string Formatted => Value.Length == 11
        ? $"({Value[..2]}) {Value[2..7]}-{Value[7..]}"
        : $"({Value[..2]}) {Value[2..6]}-{Value[6..]}";

    public bool Equals(Telefone? other)
    {
        if (other is null) return false;
        return Value == other.Value;
    }

    public override bool Equals(object? obj) => Equals(obj as Telefone);
    public override int GetHashCode() => Value.GetHashCode();
    public override string ToString() => Formatted;

    public static bool operator ==(Telefone? left, Telefone? right) => Equals(left, right);
    public static bool operator !=(Telefone? left, Telefone? right) => !Equals(left, right);

    public static implicit operator string(Telefone telefone) => telefone.Value;
}
