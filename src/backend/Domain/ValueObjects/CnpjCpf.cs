using System.Text.RegularExpressions;

namespace Severina.Domain.ValueObjects;

public sealed class CnpjCpf : IEquatable<CnpjCpf>
{
    public string Value { get; }

    private CnpjCpf(string value)
    {
        Value = value;
    }

    public static CnpjCpf Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("CPF/CNPJ é obrigatório.");

        var digits = Regex.Replace(value, @"\D", "");

        if (digits.Length == 11 && IsValidCpf(digits))
            return new CnpjCpf(digits);

        if (digits.Length == 14 && IsValidCnpj(digits))
            return new CnpjCpf(digits);

        throw new ArgumentException("CPF/CNPJ inválido.");
    }

    public static bool TryCreate(string value, out CnpjCpf? result)
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
        ? $"{Value[..3]}.{Value[3..6]}.{Value[6..9]}-{Value[9..]}"
        : $"{Value[..2]}.{Value[2..5]}.{Value[5..8]}/{Value[8..12]}-{Value[12..]}";

    public bool IsCpf => Value.Length == 11;
    public bool IsCnpj => Value.Length == 14;

    private static bool IsValidCpf(string cpf)
    {
        if (new string(cpf[0], 11) == cpf) return false;

        var sum = 0;
        for (int i = 0; i < 9; i++)
            sum += (cpf[i] - '0') * (10 - i);

        var remainder = sum % 11;
        var digit1 = remainder < 2 ? 0 : 11 - remainder;

        if (cpf[9] - '0' != digit1) return false;

        sum = 0;
        for (int i = 0; i < 10; i++)
            sum += (cpf[i] - '0') * (11 - i);

        remainder = sum % 11;
        var digit2 = remainder < 2 ? 0 : 11 - remainder;

        return cpf[10] - '0' == digit2;
    }

    private static bool IsValidCnpj(string cnpj)
    {
        if (new string(cnpj[0], 14) == cnpj) return false;

        var weights1 = new[] { 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };
        var weights2 = new[] { 6, 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };

        var sum = 0;
        for (int i = 0; i < 12; i++)
            sum += (cnpj[i] - '0') * weights1[i];

        var remainder = sum % 11;
        var digit1 = remainder < 2 ? 0 : 11 - remainder;

        if (cnpj[12] - '0' != digit1) return false;

        sum = 0;
        for (int i = 0; i < 13; i++)
            sum += (cnpj[i] - '0') * weights2[i];

        remainder = sum % 11;
        var digit2 = remainder < 2 ? 0 : 11 - remainder;

        return cnpj[13] - '0' == digit2;
    }

    public bool Equals(CnpjCpf? other)
    {
        if (other is null) return false;
        return Value == other.Value;
    }

    public override bool Equals(object? obj) => Equals(obj as CnpjCpf);
    public override int GetHashCode() => Value.GetHashCode();
    public override string ToString() => Formatted;

    public static bool operator ==(CnpjCpf? left, CnpjCpf? right) => Equals(left, right);
    public static bool operator !=(CnpjCpf? left, CnpjCpf? right) => !Equals(left, right);

    public static implicit operator string(CnpjCpf cnpjCpf) => cnpjCpf.Value;
}
