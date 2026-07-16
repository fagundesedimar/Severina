using Severina.Domain.Enums;

namespace Severina.Domain.ValueObjects;

public record Money
{
    public decimal Value { get; }

    public Money(decimal value)
    {
        if (value < 0)
            throw new ArgumentException("Valor deve ser positivo.", nameof(value));

        Value = Math.Round(value, 2);
    }

    public static Money Zero => new(0);

    public static Money operator +(Money a, Money b) => new(a.Value + b.Value);
    public static Money operator -(Money a, Money b) => new(a.Value - b.Value);

    public override string ToString() => Value.ToString("C2");
}
