using Severina.Domain.Enums;
using Severina.Domain.Events;
using Severina.Domain.ValueObjects;

namespace Severina.Domain.Entities;

public class User : BaseEntity
{
    public Guid CompanyId { get; private set; }
    public string Nome { get; private set; } = string.Empty;
    public Email Email { get; private set; } = null!;
    public string SenhaHash { get; private set; } = string.Empty;
    public PapelUsuario Papel { get; private set; }
    public StatusUsuario Status { get; private set; }

    public Company? Company { get; private set; }

    private readonly List<UserPreference> _preferences = new();
    public IReadOnlyCollection<UserPreference> Preferences => _preferences.AsReadOnly();

    private User() { }

    public User(Guid companyId, string nome, Email email, string senhaHash, PapelUsuario papel)
    {
        CompanyId = companyId;
        Nome = nome;
        Email = email;
        SenhaHash = senhaHash;
        Papel = papel;
        Status = StatusUsuario.Ativo;
    }

    public void UpdateProfile(string nome, string? telefone)
    {
        Nome = nome;
        UpdateTimestamp();
    }

    public void ChangeRole(PapelUsuario novoPapel)
    {
        Papel = novoPapel;
        UpdateTimestamp();
    }

    public void Activate()
    {
        Status = StatusUsuario.Ativo;
        UpdateTimestamp();
    }

    public void Deactivate()
    {
        Status = StatusUsuario.Inativo;
        UpdateTimestamp();
    }

    public bool IsAdministrador => Papel == PapelUsuario.Administrador;
    public bool IsActive => Status == StatusUsuario.Ativo;
}
