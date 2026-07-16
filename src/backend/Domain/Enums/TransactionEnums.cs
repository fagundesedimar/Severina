namespace Severina.Domain.Enums;

public enum TransactionType
{
    Receita = 0,
    Despesa = 1
}

public enum TransactionStatus
{
    Pending = 0,
    Approved = 1,
    Rejected = 2
}

public enum TransactionCategory
{
    Servicos = 0,
    Materiais = 1,
    Frente = 2,
    Impostos = 3,
    Outros = 4
}
