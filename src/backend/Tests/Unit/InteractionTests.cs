using Severina.Domain.Entities;
using Severina.Domain.Enums;
using Severina.Domain.ValueObjects;
using Xunit;

namespace Severina.Tests.Unit;

public class InteractionTests
{
    private static Interaction CreateValidInteraction(
        Guid? clientId = null,
        Guid? companyId = null,
        InteractionType type = InteractionType.Message,
        string content = "Mensagem de teste")
    {
        return Interaction.Create(
            clientId ?? Guid.NewGuid(),
            companyId ?? Guid.NewGuid(),
            type,
            content);
    }

    [Fact]
    public void Create_ValidInteraction_SetsProperties()
    {
        var clientId = Guid.NewGuid();
        var companyId = Guid.NewGuid();

        var interaction = Interaction.Create(
            clientId,
            companyId,
            InteractionType.Call,
            "Chamada de teste",
            InteractionMetadata.Create("inbound", 120, "completed"),
            Guid.NewGuid());

        Assert.Equal(clientId, interaction.ClientId);
        Assert.Equal(companyId, interaction.CompanyId);
        Assert.Equal(InteractionType.Call, interaction.Type);
        Assert.Equal("Chamada de teste", interaction.Content);
        Assert.NotNull(interaction.Metadata);
        Assert.Equal("inbound", interaction.Metadata.Direction);
        Assert.Equal(120, interaction.Metadata.DurationSeconds);
        Assert.Equal("completed", interaction.Metadata.Status);
    }

    [Fact]
    public void Create_Interaction_WithNullMetadata_Succeeds()
    {
        var interaction = Interaction.Create(
            Guid.NewGuid(),
            Guid.NewGuid(),
            InteractionType.Message,
            "Mensagem",
            null);
        Assert.Null(interaction.Metadata);
    }

    [Fact]
    public void Create_EmptyContent_ThrowsException()
    {
        Assert.Throws<ArgumentException>(() =>
            Interaction.Create(Guid.NewGuid(), Guid.NewGuid(), InteractionType.Message, ""));
    }

    [Fact]
    public void Create_WhitespaceContent_ThrowsException()
    {
        Assert.Throws<ArgumentException>(() =>
            Interaction.Create(Guid.NewGuid(), Guid.NewGuid(), InteractionType.Message, "   "));
    }

    [Fact]
    public void Create_ContentWithSpaces_TrimsContent()
    {
        var interaction = CreateValidInteraction(content: "  Mensagem de teste  ");
        Assert.Equal("Mensagem de teste", interaction.Content);
    }

    [Fact]
    public void Create_MessageType_SetsType()
    {
        var interaction = CreateValidInteraction(type: InteractionType.Message);
        Assert.Equal(InteractionType.Message, interaction.Type);
    }

    [Fact]
    public void Create_CallType_SetsType()
    {
        var interaction = CreateValidInteraction(type: InteractionType.Call);
        Assert.Equal(InteractionType.Call, interaction.Type);
    }

    [Fact]
    public void Create_EmailType_SetsType()
    {
        var interaction = CreateValidInteraction(type: InteractionType.Email);
        Assert.Equal(InteractionType.Email, interaction.Type);
    }

    [Fact]
    public void Create_NoteType_SetsType()
    {
        var interaction = CreateValidInteraction(type: InteractionType.Note);
        Assert.Equal(InteractionType.Note, interaction.Type);
    }

    [Fact]
    public void Create_AppointmentType_SetsType()
    {
        var interaction = CreateValidInteraction(type: InteractionType.Appointment);
        Assert.Equal(InteractionType.Appointment, interaction.Type);
    }

    [Fact]
    public void Create_WithConversationId_SetsConversationId()
    {
        var conversationId = Guid.NewGuid();
        var interaction = Interaction.Create(
            Guid.NewGuid(),
            Guid.NewGuid(),
            InteractionType.Message,
            "Mensagem",
            null,
            conversationId);
        Assert.Equal(conversationId, interaction.ConversationId);
    }

    [Fact]
    public void Create_WithoutConversationId_ConversationIdIsNull()
    {
        var interaction = Interaction.Create(
            Guid.NewGuid(),
            Guid.NewGuid(),
            InteractionType.Message,
            "Mensagem");
        Assert.Null(interaction.ConversationId);
    }
}

public class InteractionMetadataTests
{
    [Fact]
    public void Create_WithAllParameters_SetsProperties()
    {
        var metadata = InteractionMetadata.Create(
            direction: "inbound",
            durationSeconds: 120,
            status: "completed",
            contentPreview: "Preview do conteúdo",
            readStatus: true);

        Assert.Equal("inbound", metadata.Direction);
        Assert.Equal(120, metadata.DurationSeconds);
        Assert.Equal("completed", metadata.Status);
        Assert.Equal("Preview do conteúdo", metadata.ContentPreview);
        Assert.True(metadata.ReadStatus);
    }

    [Fact]
    public void Create_WithDefaults_AllPropertiesNull()
    {
        var metadata = InteractionMetadata.Create();

        Assert.Null(metadata.Direction);
        Assert.Null(metadata.DurationSeconds);
        Assert.Null(metadata.Status);
        Assert.Null(metadata.ContentPreview);
        Assert.Null(metadata.ReadStatus);
    }

    [Fact]
    public void Create_WithPartialParameters_SetsOnlyProvided()
    {
        var metadata = InteractionMetadata.Create(direction: "outbound", durationSeconds: 60);

        Assert.Equal("outbound", metadata.Direction);
        Assert.Equal(60, metadata.DurationSeconds);
        Assert.Null(metadata.Status);
        Assert.Null(metadata.ContentPreview);
        Assert.Null(metadata.ReadStatus);
    }
}
