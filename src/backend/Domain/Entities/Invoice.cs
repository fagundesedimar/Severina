using Severina.Domain.Enums;
using Severina.Domain.Events;
using Severina.Domain.ValueObjects;

namespace Severina.Domain.Entities;

public class Invoice : BaseEntity
{
    private readonly List<IDomainEvent> _domainEvents = new();

    public Guid CompanyId { get; private set; }
    public Guid? ClientId { get; private set; }
    public string Numero { get; private set; } = string.Empty;
    public decimal Valor { get; private set; }
    public decimal ValorPago { get; private set; }
    public DateTime DataVencimento { get; private set; }
    public DateTime? DataPagamento { get; private set; }
    public string? Descricao { get; private set; }
    public InvoiceStatus Status { get; private set; }
    public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    private Invoice() { }

    public Invoice(Guid companyId, decimal valor, DateTime dataVencimento, Guid? clientId = null, string? descricao = null, string? numero = null)
    {
        CompanyId = companyId;
        Valor = valor;
        DataVencimento = dataVencimento;
        ClientId = clientId;
        Descricao = descricao;
        Numero = numero ?? string.Empty;
        ValorPago = 0;
        Status = InvoiceStatus.Pending;

        AddDomainEvent(new InvoiceCreatedEvent(Id, CompanyId, Valor, DataVencimento));
    }

    public void Pay(decimal valorPago, DateTime dataPagamento)
    {
        if (Status == InvoiceStatus.Paid)
            throw new InvalidOperationException("Cobrança paga não pode ser alterada.");

        if (Status == InvoiceStatus.Cancelled)
            throw new InvalidOperationException("Cobrança cancelada não pode ser paga.");

        if (valorPago > Valor)
            throw new ArgumentException("Valor pago excede valor da cobrança.");

        ValorPago += valorPago;
        DataPagamento = dataPagamento;

        if (ValorPago >= Valor)
        {
            Status = InvoiceStatus.Paid;
            AddDomainEvent(new InvoicePaidEvent(Id, CompanyId, Valor, ValorPago));
        }
        else
        {
            Status = InvoiceStatus.Partial;
        }

        UpdateTimestamp();
    }

    public void Cancel()
    {
        if (Status == InvoiceStatus.Paid)
            throw new InvalidOperationException("Cobrança paga não pode ser cancelada.");

        if (Status == InvoiceStatus.Cancelled)
            throw new InvalidOperationException("Cobrança já está cancelada.");

        Status = InvoiceStatus.Cancelled;
        UpdateTimestamp();
    }

    public void MarkOverdue()
    {
        if (Status != InvoiceStatus.Pending && Status != InvoiceStatus.Partial)
            return;

        Status = InvoiceStatus.Overdue;
        UpdateTimestamp();
        AddDomainEvent(new InvoiceOverdueEvent(Id, CompanyId, Valor, DataVencimento));
    }

    public void SetNumero(string numero)
    {
        Numero = numero;
    }

    public void UpdateDetails(decimal valor, DateTime dataVencimento, Guid? clientId, string? descricao)
    {
        if (Status == InvoiceStatus.Paid)
            throw new InvalidOperationException("Cobrança paga não pode ser alterada.");

        Valor = valor;
        DataVencimento = dataVencimento;
        ClientId = clientId;
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
