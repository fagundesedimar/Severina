using Severina.Domain.Entities;
using Severina.Domain.Enums;
using Severina.Domain.ValueObjects;
using Xunit;

namespace Severina.Tests.Unit;

public class ClientTests
{
    private static Client CreateValidClient(
        Guid? companyId = null,
        string nome = "João Silva",
        string? email = "joao@teste.com",
        string? telefone = "11999998888",
        string? empresa = "Empresa Teste")
    {
        return new Client(
            companyId ?? Guid.NewGuid(),
            nome,
            email,
            telefone,
            empresa);
    }

    [Fact]
    public void Create_ValidClient_SetsProperties()
    {
        var companyId = Guid.NewGuid();
        var client = CreateValidClient(companyId: companyId);

        Assert.Equal(companyId, client.CompanyId);
        Assert.Equal("João Silva", client.Nome);
        Assert.Equal("joao@teste.com", client.Email!.Value);
        Assert.Equal("11999998888", client.Telefone);
        Assert.Equal("Empresa Teste", client.Empresa);
        Assert.Equal(StatusCliente.Ativo, client.Status);
    }

    [Fact]
    public void Create_Client_RaisesDomainEvent()
    {
        var client = CreateValidClient();
        Assert.Contains(client.DomainEvents, e => e is Severina.Domain.Events.ClientCreatedEvent);
    }

    [Fact]
    public void Create_Client_WithoutEmail_SetsEmailNull()
    {
        var client = CreateValidClient(email: null);
        Assert.Null(client.Email);
    }

    [Fact]
    public void Create_Client_WithInvalidEmail_ThrowsException()
    {
        Assert.Throws<ArgumentException>(() => CreateValidClient(email: "invalido"));
    }

    [Fact]
    public void AddTag_NewTag_AddsToCollection()
    {
        var client = CreateValidClient();
        client.ClearDomainEvents();

        client.AddTag("VIP");

        Assert.Single(client.Tags);
        Assert.Equal("VIP", client.Tags.First().Name);
    }

    [Fact]
    public void AddTag_DuplicateTag_DoesNotAdd()
    {
        var client = CreateValidClient();
        client.AddTag("VIP");
        client.ClearDomainEvents();

        client.AddTag("VIP");

        Assert.Single(client.Tags);
    }

    [Fact]
    public void AddTag_DifferentCase_DoesNotAdd()
    {
        var client = CreateValidClient();
        client.AddTag("VIP");
        client.ClearDomainEvents();

        client.AddTag("vip");

        Assert.Single(client.Tags);
    }

    [Fact]
    public void AddTag_RaisesDomainEvent()
    {
        var client = CreateValidClient();
        client.ClearDomainEvents();

        client.AddTag("VIP");

        Assert.Contains(client.DomainEvents, e => e is Severina.Domain.Events.ClientTagAddedEvent);
    }

    [Fact]
    public void RemoveTag_ExistingTag_RemovesFromCollection()
    {
        var client = CreateValidClient();
        client.AddTag("VIP");
        client.AddTag("Premium");
        client.ClearDomainEvents();

        client.RemoveTag("VIP");

        Assert.Single(client.Tags);
        Assert.Equal("Premium", client.Tags.First().Name);
    }

    [Fact]
    public void RemoveTag_NonExistingTag_DoesNothing()
    {
        var client = CreateValidClient();
        client.AddTag("VIP");
        client.ClearDomainEvents();

        client.RemoveTag("Inexistente");

        Assert.Single(client.Tags);
    }

    [Fact]
    public void RemoveTag_DifferentCase_RemovesTag()
    {
        var client = CreateValidClient();
        client.AddTag("VIP");
        client.ClearDomainEvents();

        client.RemoveTag("vip");

        Assert.Empty(client.Tags);
    }

    [Fact]
    public void AddNote_ValidContent_AddsToCollection()
    {
        var client = CreateValidClient();
        var authorId = Guid.NewGuid();
        client.ClearDomainEvents();

        client.AddNote("Nota de teste", authorId);

        Assert.Single(client.Notes);
        Assert.Equal("Nota de teste", client.Notes.First().Content);
        Assert.Equal(authorId, client.Notes.First().AuthorId);
    }

    [Fact]
    public void AddNote_EmptyContent_ThrowsException()
    {
        var client = CreateValidClient();
        Assert.Throws<ArgumentException>(() => client.AddNote("", Guid.NewGuid()));
    }

    [Fact]
    public void AddNote_ExceedsMaxLength_ThrowsException()
    {
        var client = CreateValidClient();
        var longContent = new string('A', 2001);
        Assert.Throws<ArgumentException>(() => client.AddNote(longContent, Guid.NewGuid()));
    }

    [Fact]
    public void AddNote_UpdatesTimestamp()
    {
        var client = CreateValidClient();
        var originalUpdatedAt = client.UpdatedAt;
        client.ClearDomainEvents();

        client.AddNote("Nota", Guid.NewGuid());

        Assert.True(client.UpdatedAt >= originalUpdatedAt);
    }

    [Fact]
    public void UpdateContactInfo_ValidInput_UpdatesFields()
    {
        var client = CreateValidClient();
        client.ClearDomainEvents();

        client.UpdateContactInfo("Maria Santos", "maria@teste.com", "11888887777", "Nova Empresa");

        Assert.Equal("Maria Santos", client.Nome);
        Assert.Equal("maria@teste.com", client.Email!.Value);
        Assert.Equal("11888887777", client.Telefone);
        Assert.Equal("Nova Empresa", client.Empresa);
    }

    [Fact]
    public void UpdateContactInfo_RaisesDomainEvent()
    {
        var client = CreateValidClient();
        client.ClearDomainEvents();

        client.UpdateContactInfo("Maria Santos", null, null, null);

        Assert.Contains(client.DomainEvents, e => e is Severina.Domain.Events.ClientUpdatedEvent);
    }

    [Fact]
    public void Deactivate_Client_SetsStatusInactive()
    {
        var client = CreateValidClient();
        client.Deactivate();

        Assert.Equal(StatusCliente.Inativo, client.Status);
    }

    [Fact]
    public void Activate_Client_SetsStatusActive()
    {
        var client = CreateValidClient();
        client.Deactivate();
        client.Activate();

        Assert.Equal(StatusCliente.Ativo, client.Status);
    }
}

public class ClientEmailTests
{
    [Fact]
    public void Create_ValidEmail_ReturnsEmail()
    {
        var email = ClientEmail.Create("teste@exemplo.com");
        Assert.Equal("teste@exemplo.com", email.Value);
    }

    [Fact]
    public void Create_EmailWithUpperCase_ConvertsToLower()
    {
        var email = ClientEmail.Create("TESTE@EXEMPLO.COM");
        Assert.Equal("teste@exemplo.com", email.Value);
    }

    [Fact]
    public void Create_EmailWithSpaces_Trims()
    {
        var email = ClientEmail.Create("  teste@exemplo.com  ");
        Assert.Equal("teste@exemplo.com", email.Value);
    }

    [Fact]
    public void Create_EmptyEmail_ThrowsException()
    {
        Assert.Throws<ArgumentException>(() => ClientEmail.Create(""));
    }

    [Fact]
    public void Create_InvalidFormat_ThrowsException()
    {
        Assert.Throws<ArgumentException>(() => ClientEmail.Create("invalido"));
    }

    [Fact]
    public void Create_EmailExceedsMaxLength_ThrowsException()
    {
        var longEmail = new string('a', 250) + "@exemplo.com";
        Assert.Throws<ArgumentException>(() => ClientEmail.Create(longEmail));
    }

    [Fact]
    public void TryCreate_ValidEmail_ReturnsTrue()
    {
        var result = ClientEmail.TryCreate("teste@exemplo.com", out var email);
        Assert.True(result);
        Assert.NotNull(email);
    }

    [Fact]
    public void TryCreate_InvalidEmail_ReturnsFalse()
    {
        var result = ClientEmail.TryCreate("invalido", out var email);
        Assert.False(result);
        Assert.Null(email);
    }

    [Fact]
    public void Equals_SameValue_ReturnsTrue()
    {
        var email1 = ClientEmail.Create("teste@exemplo.com");
        var email2 = ClientEmail.Create("teste@exemplo.com");
        Assert.True(email1.Equals(email2));
    }

    [Fact]
    public void Equals_DifferentValue_ReturnsFalse()
    {
        var email1 = ClientEmail.Create("teste@exemplo.com");
        var email2 = ClientEmail.Create("outro@exemplo.com");
        Assert.False(email1.Equals(email2));
    }

    [Fact]
    public void ToString_ReturnsValue()
    {
        var email = ClientEmail.Create("teste@exemplo.com");
        Assert.Equal("teste@exemplo.com", email.ToString());
    }
}

public class ClientTagTests
{
    [Fact]
    public void Create_ValidTag_ReturnsTag()
    {
        var tag = ClientTag.Create("VIP");
        Assert.Equal("VIP", tag.Name);
    }

    [Fact]
    public void Create_TagWithSpaces_Trims()
    {
        var tag = ClientTag.Create("  VIP  ");
        Assert.Equal("VIP", tag.Name);
    }

    [Fact]
    public void Create_EmptyTag_ThrowsException()
    {
        Assert.Throws<ArgumentException>(() => ClientTag.Create(""));
    }

    [Fact]
    public void Create_ExceedsMaxLength_ThrowsException()
    {
        var longTag = new string('A', 51);
        Assert.Throws<ArgumentException>(() => ClientTag.Create(longTag));
    }

    [Fact]
    public void Equals_SameName_ReturnsTrue()
    {
        var tag1 = ClientTag.Create("VIP");
        var tag2 = ClientTag.Create("VIP");
        Assert.True(tag1.Equals(tag2));
    }

    [Fact]
    public void Equals_DifferentCase_ReturnsTrue()
    {
        var tag1 = ClientTag.Create("VIP");
        var tag2 = ClientTag.Create("vip");
        Assert.True(tag1.Equals(tag2));
    }

    [Fact]
    public void Equals_DifferentName_ReturnsFalse()
    {
        var tag1 = ClientTag.Create("VIP");
        var tag2 = ClientTag.Create("Premium");
        Assert.False(tag1.Equals(tag2));
    }

    [Fact]
    public void TryCreate_ValidTag_ReturnsTrue()
    {
        var result = ClientTag.TryCreate("VIP", out var tag);
        Assert.True(result);
        Assert.NotNull(tag);
    }

    [Fact]
    public void TryCreate_EmptyTag_ReturnsFalse()
    {
        var result = ClientTag.TryCreate("", out var tag);
        Assert.False(result);
        Assert.Null(tag);
    }
}

public class ClientNoteTests
{
    [Fact]
    public void Create_ValidContent_ReturnsNote()
    {
        var authorId = Guid.NewGuid();
        var note = ClientNote.Create("Nota de teste", authorId);

        Assert.Equal("Nota de teste", note.Content);
        Assert.Equal(authorId, note.AuthorId);
        Assert.NotEqual(Guid.Empty, note.Id);
        Assert.True(note.CreatedAt <= DateTime.UtcNow);
    }

    [Fact]
    public void Create_EmptyContent_ThrowsException()
    {
        Assert.Throws<ArgumentException>(() => ClientNote.Create("", Guid.NewGuid()));
    }

    [Fact]
    public void Create_ExceedsMaxLength_ThrowsException()
    {
        var longContent = new string('A', 2001);
        Assert.Throws<ArgumentException>(() => ClientNote.Create(longContent, Guid.NewGuid()));
    }

    [Fact]
    public void Create_ContentWithSpaces_Trims()
    {
        var note = ClientNote.Create("  Nota de teste  ", Guid.NewGuid());
        Assert.Equal("Nota de teste", note.Content);
    }
}
