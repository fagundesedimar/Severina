using Severina.Domain.Enums;
using Severina.Domain.Events;
using Severina.Domain.Interfaces;
using Severina.Domain.ValueObjects;

namespace Severina.Domain.Entities;

public class Client : BaseEntity
{
    private readonly List<ClientTag> _tags = new();
    private readonly List<ClientNote> _notes = new();
    private readonly List<Interaction> _interactions = new();
    private readonly List<IDomainEvent> _domainEvents = new();

    public Guid CompanyId { get; private set; }
    public string Nome { get; private set; } = string.Empty;
    public ClientEmail? Email { get; private set; }
    public string? Telefone { get; private set; }
    public string? Empresa { get; private set; }
    public StatusCliente Status { get; private set; }
    public IReadOnlyCollection<ClientTag> Tags => _tags.AsReadOnly();
    public IReadOnlyCollection<ClientNote> Notes => _notes.AsReadOnly();
    public IReadOnlyCollection<Interaction> Interactions => _interactions.AsReadOnly();
    public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    private Client() { }

    public Client(
        Guid companyId,
        string nome,
        string? email,
        string? telefone,
        string? empresa)
    {
        CompanyId = companyId;
        Nome = nome;
        Email = string.IsNullOrWhiteSpace(email) ? null : ClientEmail.Create(email);
        Telefone = telefone;
        Empresa = empresa;
        Status = StatusCliente.Ativo;

        AddDomainEvent(new ClientCreatedEvent(Id, CompanyId, Nome));
    }

    public void UpdateContactInfo(string nome, string? email, string? telefone, string? empresa)
    {
        Nome = nome;
        Email = string.IsNullOrWhiteSpace(email) ? null : ClientEmail.Create(email);
        Telefone = telefone;
        Empresa = empresa;
        UpdateTimestamp();
        AddDomainEvent(new ClientUpdatedEvent(Id, CompanyId));
    }

    public void AddTag(string tagName)
    {
        var tag = ClientTag.Create(tagName);
        if (_tags.Any(t => t.Equals(tag)))
            return;

        _tags.Add(tag);
        UpdateTimestamp();
        AddDomainEvent(new ClientTagAddedEvent(Id, CompanyId, tagName));
    }

    public void RemoveTag(string tagName)
    {
        var tag = _tags.FirstOrDefault(t => t.Name.Equals(tagName, StringComparison.OrdinalIgnoreCase));
        if (tag is not null)
        {
            _tags.Remove(tag);
            UpdateTimestamp();
        }
    }

    public void AddNote(string content, Guid authorId)
    {
        var note = ClientNote.Create(content, authorId);
        _notes.Add(note);
        UpdateTimestamp();
    }

    public void Deactivate()
    {
        Status = StatusCliente.Inativo;
        UpdateTimestamp();
    }

    public void Activate()
    {
        Status = StatusCliente.Ativo;
        UpdateTimestamp();
    }

    private void AddDomainEvent(IDomainEvent domainEvent)
    {
        _domainEvents.Add(domainEvent);
    }

    public void ClearDomainEvents()
    {
        _domainEvents.Clear();
    }
}
