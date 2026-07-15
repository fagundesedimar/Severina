namespace Severina.Domain.ValueObjects;

public sealed class ClientTag : IEquatable<ClientTag>
{
    public string Name { get; }

    private ClientTag(string name)
    {
        Name = name;
    }

    public static ClientTag Create(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Nome da tag é obrigatório");

        var trimmed = name.Trim();

        if (trimmed.Length > 50)
            throw new ArgumentException("Nome da tag deve ter no máximo 50 caracteres");

        return new ClientTag(trimmed);
    }

    public static bool TryCreate(string name, out ClientTag? result)
    {
        try
        {
            result = Create(name);
            return true;
        }
        catch
        {
            result = null;
            return false;
        }
    }

    public bool Equals(ClientTag? other)
    {
        if (other is null) return false;
        return string.Equals(Name, other.Name, StringComparison.OrdinalIgnoreCase);
    }

    public override bool Equals(object? obj) => Equals(obj as ClientTag);
    public override int GetHashCode() => Name.ToLowerInvariant().GetHashCode();
    public override string ToString() => Name;

    public static bool operator ==(ClientTag? left, ClientTag? right) => Equals(left, right);
    public static bool operator !=(ClientTag? left, ClientTag? right) => !Equals(left, right);
}
