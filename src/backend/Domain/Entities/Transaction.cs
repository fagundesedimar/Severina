using Severina.Domain.Enums;
using Severina.Domain.Events;

namespace Severina.Domain.Entities;

public class Transaction : BaseEntity
{
    private readonly List<IDomainEvent> _domainEvents = new();

    public Guid CompanyId { get; private set; }
    public Guid? ClientId { get; private set; }
    public TransactionType Tipo { get; private set; }
    public decimal Valor { get; private set; }
    public DateTime Data { get; private set; }
    public TransactionCategory Categoria { get; private set; }
    public string? Descricao { get; private set; }
    public TransactionStatus Status { get; private set; }
    public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    private Transaction() { }

    public Transaction(Guid companyId, TransactionType tipo, decimal valor, DateTime data, TransactionCategory categoria, Guid? clientId = null, string? descricao = null)
    {
        CompanyId = companyId;
        Tipo = tipo;
        Valor = valor;
        Data = data;
        Categoria = categoria;
        ClientId = clientId;
        Descricao = descricao;
        Status = TransactionStatus.Pending;

        AddDomainEvent(new TransactionCreatedEvent(Id, CompanyId, Tipo, Valor, Data));
    }

    public void Approve()
    {
        if (Status == TransactionStatus.Approved)
            throw new InvalidOperationException("Transação já foi aprovada.");

        if (Status == TransactionStatus.Rejected)
            throw new InvalidOperationException("Transação rejeitada não pode ser aprovada.");

        var oldStatus = Status;
        Status = TransactionStatus.Approved;
        UpdateTimestamp();
        AddDomainEvent(new TransactionApprovedEvent(Id, CompanyId, oldStatus.ToString(), Status.ToString()));
    }

    public void Reject()
    {
        if (Status == TransactionStatus.Rejected)
            throw new InvalidOperationException("Transação já foi rejeitada.");

        if (Status == TransactionStatus.Approved)
            throw new InvalidOperationException("Transação aprovada não pode ser rejeitada.");

        var oldStatus = Status;
        Status = TransactionStatus.Rejected;
        UpdateTimestamp();
        AddDomainEvent(new TransactionRejectedEvent(Id, CompanyId, oldStatus.ToString(), Status.ToString()));
    }

    public void Categorize(TransactionCategory novaCategoria)
    {
        Categoria = novaCategoria;
        UpdateTimestamp();
    }

    public void UpdateDetails(TransactionType tipo, decimal valor, DateTime data, TransactionCategory categoria, string? descricao)
    {
        if (Status == TransactionStatus.Approved)
            throw new InvalidOperationException("Transação aprovada não pode ser alterada.");

        Tipo = tipo;
        Valor = valor;
        Data = data;
        Categoria = categoria;
        Descricao = descricao;
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
