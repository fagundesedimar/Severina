using Severina.Domain.Enums;

namespace Severina.Domain.Entities;

public class Company : BaseEntity
{
    public string Nome { get; private set; } = string.Empty;
    public string CnpjCpf { get; private set; } = string.Empty;
    public string Email { get; private set; } = string.Empty;
    public string? Telefone { get; private set; }
    public TipoPessoa TipoPessoa { get; private set; }
    public StatusEmpresa Status { get; private set; }
    public string? Plano { get; private set; }

    private readonly List<User> _users = new();
    public IReadOnlyCollection<User> Users => _users.AsReadOnly();

    private Company() { }

    public Company(string nome, string cnpjCpf, string email, TipoPessoa tipoPessoa, string? telefone = null)
    {
        Nome = nome;
        CnpjCpf = cnpjCpf;
        Email = email;
        TipoPessoa = tipoPessoa;
        Telefone = telefone;
        Status = StatusEmpresa.Ativa;
    }

    public void Update(string nome, string email, string? telefone)
    {
        Nome = nome;
        Email = email;
        Telefone = telefone;
        UpdateTimestamp();
    }

    public void Activate()
    {
        Status = StatusEmpresa.Ativa;
        UpdateTimestamp();
    }

    public void Deactivate()
    {
        Status = StatusEmpresa.Inativa;
        UpdateTimestamp();
    }

    public void Suspend()
    {
        Status = StatusEmpresa.Suspensa;
        UpdateTimestamp();
    }

    public void AddUser(User user)
    {
        _users.Add(user);
        UpdateTimestamp();
    }

    public void RemoveUser(User user)
    {
        _users.Remove(user);
        UpdateTimestamp();
    }
}
