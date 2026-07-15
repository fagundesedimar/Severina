using System.Text.RegularExpressions;

namespace Severina.Domain.ValueObjects;

public sealed class Email : IEquatable<Email>
{
    private static readonly Regex EmailRegex = new(
        @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$",
        RegexOptions.Compiled);

    public string Value { get; }

    private Email(string value)
    {
        Value = value;
    }

    public static Email Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Email é obrigatório.");

        var trimmed = value.Trim().ToLowerInvariant();

        if (!EmailRegex.IsMatch(trimmed))
            throw new ArgumentException("Formato de email inválido.");

        return new Email(trimmed);
    }

    public static bool TryCreate(string value, out Email? result)
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

    public bool Equals(Email? other)
    {
        if (other is null) return false;
        return Value == other.Value;
    }

    public override bool Equals(object? obj) => Equals(obj as Email);
    public override int GetHashCode() => Value.GetHashCode();
    public override string ToString() => Value;

    public static bool operator ==(Email? left, Email? right) => Equals(left, right);
    public static bool operator !=(Email? left, Email? right) => !Equals(left, right);

    public static implicit operator string(Email email) => email.Value;
}
