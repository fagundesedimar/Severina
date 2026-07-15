namespace Severina.Domain.ValueObjects;

public sealed class ClientNote
{
    public Guid Id { get; }
    public string Content { get; }
    public Guid AuthorId { get; }
    public DateTime CreatedAt { get; }

    private ClientNote(Guid id, string content, Guid authorId, DateTime createdAt)
    {
        Id = id;
        Content = content;
        AuthorId = authorId;
        CreatedAt = createdAt;
    }

    public static ClientNote Create(string content, Guid authorId)
    {
        if (string.IsNullOrWhiteSpace(content))
            throw new ArgumentException("Conteúdo da nota é obrigatório");

        if (content.Length > 2000)
            throw new ArgumentException("Nota deve ter no máximo 2000 caracteres");

        return new ClientNote(Guid.NewGuid(), content.Trim(), authorId, DateTime.UtcNow);
    }
}
