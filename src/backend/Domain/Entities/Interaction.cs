using Severina.Domain.Enums;
using Severina.Domain.ValueObjects;

namespace Severina.Domain.Entities;

public class Interaction : BaseEntity
{
    public Guid ClientId { get; private set; }
    public Guid CompanyId { get; private set; }
    public InteractionType Type { get; private set; }
    public string Content { get; private set; } = string.Empty;
    public InteractionMetadata? Metadata { get; private set; }
    public Guid? ConversationId { get; private set; }

    private Interaction() { }

    public Interaction(
        Guid clientId,
        Guid companyId,
        InteractionType type,
        string content,
        InteractionMetadata? metadata,
        Guid? conversationId)
    {
        ClientId = clientId;
        CompanyId = companyId;
        Type = type;
        Content = content;
        Metadata = metadata;
        ConversationId = conversationId;
    }

    public static Interaction Create(
        Guid clientId,
        Guid companyId,
        InteractionType type,
        string content,
        InteractionMetadata? metadata = null,
        Guid? conversationId = null)
    {
        if (string.IsNullOrWhiteSpace(content))
            throw new ArgumentException("Conteúdo da interação é obrigatório");

        return new Interaction(clientId, companyId, type, content.Trim(), metadata, conversationId);
    }
}
