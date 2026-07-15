namespace Severina.Domain.ValueObjects;

public sealed class ClientEmail : IEquatable<ClientEmail>
{
    public string Value { get; }

    private ClientEmail(string value)
    {
        Value = value;
    }

    public static ClientEmail Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Email é obrigatório");

        var trimmed = value.Trim().ToLowerInvariant();

        if (trimmed.Length > 256)
            throw new ArgumentException("Email deve ter no máximo 256 caracteres");

        try
        {
            var addr = new System.Net.Mail.MailAddress(trimmed);
            if (addr.Address != trimmed)
                throw new ArgumentException("Formato de email inválido");
        }
        catch (FormatException)
        {
            throw new ArgumentException("Formato de email inválido");
        }

        return new ClientEmail(trimmed);
    }

    public static bool TryCreate(string value, out ClientEmail? result)
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

    public bool Equals(ClientEmail? other)
    {
        if (other is null) return false;
        return Value == other.Value;
    }

    public override bool Equals(object? obj) => Equals(obj as ClientEmail);
    public override int GetHashCode() => Value.GetHashCode();
    public override string ToString() => Value;

    public static implicit operator string(ClientEmail email) => email.Value;

    public static bool operator ==(ClientEmail? left, ClientEmail? right) => Equals(left, right);
    public static bool operator !=(ClientEmail? left, ClientEmail? right) => !Equals(left, right);
}
