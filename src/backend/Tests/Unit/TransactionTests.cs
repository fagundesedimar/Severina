using Severina.Domain.Entities;
using Severina.Domain.Enums;
using Xunit;

namespace Severina.Tests.Unit;

public class TransactionTests
{
    private static Transaction CreateValidTransaction(
        Guid? companyId = null,
        TransactionType tipo = TransactionType.Receita,
        decimal valor = 100m)
    {
        return new Transaction(
            companyId ?? Guid.NewGuid(),
            tipo,
            valor,
            DateTime.UtcNow,
            TransactionCategory.Servicos);
    }

    [Fact]
    public void Create_ValidTransaction_SetsStatusPending()
    {
        var transaction = CreateValidTransaction();
        Assert.Equal(TransactionStatus.Pending, transaction.Status);
    }

    [Fact]
    public void Create_ValidTransaction_SetsProperties()
    {
        var companyId = Guid.NewGuid();
        var clientId = Guid.NewGuid();
        var data = DateTime.UtcNow;

        var transaction = new Transaction(
            companyId,
            TransactionType.Despesa,
            250.50m,
            data,
            TransactionCategory.Materiais,
            clientId,
            "Compra de material");

        Assert.Equal(companyId, transaction.CompanyId);
        Assert.Equal(TransactionType.Despesa, transaction.Tipo);
        Assert.Equal(250.50m, transaction.Valor);
        Assert.Equal(data, transaction.Data);
        Assert.Equal(TransactionCategory.Materiais, transaction.Categoria);
        Assert.Equal(clientId, transaction.ClientId);
        Assert.Equal("Compra de material", transaction.Descricao);
    }

    [Fact]
    public void Create_Transaction_RaisesDomainEvent()
    {
        var transaction = CreateValidTransaction();
        Assert.Single(transaction.DomainEvents);
        Assert.IsType<Severina.Domain.Events.TransactionCreatedEvent>(transaction.DomainEvents.First());
    }

    [Fact]
    public void Approve_PendingTransaction_SetsStatusApproved()
    {
        var transaction = CreateValidTransaction();
        transaction.ClearDomainEvents();
        transaction.Approve();
        Assert.Equal(TransactionStatus.Approved, transaction.Status);
    }

    [Fact]
    public void Approve_PendingTransaction_RaisesApprovedEvent()
    {
        var transaction = CreateValidTransaction();
        transaction.ClearDomainEvents();
        transaction.Approve();
        Assert.Contains(transaction.DomainEvents, e => e is Severina.Domain.Events.TransactionApprovedEvent);
    }

    [Fact]
    public void Approve_AlreadyApproved_ThrowsException()
    {
        var transaction = CreateValidTransaction();
        transaction.Approve();
        Assert.Throws<InvalidOperationException>(() => transaction.Approve());
    }

    [Fact]
    public void Approve_RejectedTransaction_ThrowsException()
    {
        var transaction = CreateValidTransaction();
        transaction.Reject();
        Assert.Throws<InvalidOperationException>(() => transaction.Approve());
    }

    [Fact]
    public void Reject_PendingTransaction_SetsStatusRejected()
    {
        var transaction = CreateValidTransaction();
        transaction.ClearDomainEvents();
        transaction.Reject();
        Assert.Equal(TransactionStatus.Rejected, transaction.Status);
    }

    [Fact]
    public void Reject_PendingTransaction_RaisesRejectedEvent()
    {
        var transaction = CreateValidTransaction();
        transaction.ClearDomainEvents();
        transaction.Reject();
        Assert.Contains(transaction.DomainEvents, e => e is Severina.Domain.Events.TransactionRejectedEvent);
    }

    [Fact]
    public void Reject_AlreadyRejected_ThrowsException()
    {
        var transaction = CreateValidTransaction();
        transaction.Reject();
        Assert.Throws<InvalidOperationException>(() => transaction.Reject());
    }

    [Fact]
    public void Reject_ApprovedTransaction_ThrowsException()
    {
        var transaction = CreateValidTransaction();
        transaction.Approve();
        Assert.Throws<InvalidOperationException>(() => transaction.Reject());
    }

    [Fact]
    public void Categorize_Transaction_UpdatesCategory()
    {
        var transaction = CreateValidTransaction();
        transaction.Categorize(TransactionCategory.Impostos);
        Assert.Equal(TransactionCategory.Impostos, transaction.Categoria);
    }

    [Fact]
    public void UpdateDetails_PendingTransaction_UpdatesFields()
    {
        var transaction = CreateValidTransaction();
        var newData = DateTime.UtcNow.AddDays(5);

        transaction.UpdateDetails(TransactionType.Despesa, 500m, newData, TransactionCategory.Frente, "Nova descrição");

        Assert.Equal(TransactionType.Despesa, transaction.Tipo);
        Assert.Equal(500m, transaction.Valor);
        Assert.Equal(newData, transaction.Data);
        Assert.Equal(TransactionCategory.Frente, transaction.Categoria);
        Assert.Equal("Nova descrição", transaction.Descricao);
    }

    [Fact]
    public void UpdateDetails_ApprovedTransaction_ThrowsException()
    {
        var transaction = CreateValidTransaction();
        transaction.Approve();
        Assert.Throws<InvalidOperationException>(() =>
            transaction.UpdateDetails(TransactionType.Despesa, 500m, DateTime.UtcNow, TransactionCategory.Frente, null));
    }

    [Fact]
    public void SoftDelete_Transaction_SetsDeletedAt()
    {
        var transaction = CreateValidTransaction();
        transaction.SoftDelete();
        Assert.NotNull(transaction.DeletedAt);
    }
}
