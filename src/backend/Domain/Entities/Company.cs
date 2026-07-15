using System.Text.Json;
using Severina.Domain.Enums;
using Severina.Domain.Events;
using Severina.Domain.ValueObjects;

namespace Severina.Domain.Entities;

public class Company : BaseEntity
{
    public string Nome { get; private set; } = string.Empty;
    public CnpjCpf CnpjCpf { get; private set; } = null!;
    public Email Email { get; private set; } = null!;
    public Telefone? Telefone { get; private set; }
    public TipoPessoa TipoPessoa { get; private set; }
    public StatusEmpresa Status { get; private set; }
    public string? Plano { get; private set; }
    public string Configuracoes { get; private set; } = "{}";

    private readonly List<User> _users = new();
    public IReadOnlyCollection<User> Users => _users.AsReadOnly();

    private Company() { }

    public Company(string nome, CnpjCpf cnpjCpf, Email email, TipoPessoa tipoPessoa, Telefone? telefone = null)
    {
        Nome = nome;
        CnpjCpf = cnpjCpf;
        Email = email;
        TipoPessoa = tipoPessoa;
        Telefone = telefone;
        Status = StatusEmpresa.Ativa;
    }

    public void Update(string nome, Email email, Telefone? telefone)
    {
        Nome = nome;
        Email = email;
        Telefone = telefone;
        UpdateTimestamp();
    }

    public void SetConfiguracoes(Dictionary<string, object> configuracoes)
    {
        Configuracoes = JsonSerializer.Serialize(configuracoes);
        UpdateTimestamp();
    }

    public Dictionary<string, object> GetConfiguracoes()
    {
        return JsonSerializer.Deserialize<Dictionary<string, object>>(Configuracoes) ?? new();
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
