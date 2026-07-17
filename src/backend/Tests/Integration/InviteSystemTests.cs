using Severina.Application.Commands.Users;
using Severina.Infrastructure.Services;
using Xunit;

namespace Severina.Tests.Integration;

public class InviteSystemTests : IDisposable
{
    private readonly InMemoryInviteCacheService _cacheService;

    public InviteSystemTests()
    {
        _cacheService = new InMemoryInviteCacheService();
    }

    public void Dispose()
    {
    }

    [Fact]
    public async Task SaveInvite_StoresInvite()
    {
        var invite = new InviteData(
            Guid.NewGuid(),
            "test@email.com",
            Severina.Domain.Enums.PapelUsuario.Operacional,
            DateTime.UtcNow,
            DateTime.UtcNow.AddDays(7));

        await _cacheService.SaveInviteAsync("test-code", invite);

        var result = await _cacheService.GetInviteAsync("test-code");
        Assert.NotNull(result);
        Assert.Equal("test@email.com", result.Email);
    }

    [Fact]
    public async Task GetInvite_ExpiredInvite_ReturnsNull()
    {
        var invite = new InviteData(
            Guid.NewGuid(),
            "test@email.com",
            Severina.Domain.Enums.PapelUsuario.Operacional,
            DateTime.UtcNow.AddDays(-10),
            DateTime.UtcNow.AddDays(-3));

        await _cacheService.SaveInviteAsync("expired-code", invite);

        var result = await _cacheService.GetInviteAsync("expired-code");
        Assert.Null(result);
    }

    [Fact]
    public async Task DeleteInvite_RemovesInvite()
    {
        var invite = new InviteData(
            Guid.NewGuid(),
            "test@email.com",
            Severina.Domain.Enums.PapelUsuario.Operacional,
            DateTime.UtcNow,
            DateTime.UtcNow.AddDays(7));

        await _cacheService.SaveInviteAsync("delete-code", invite);
        await _cacheService.DeleteInviteAsync("delete-code");

        var result = await _cacheService.GetInviteAsync("delete-code");
        Assert.Null(result);
    }

    [Fact]
    public async Task GetPendingInvites_ReturnsCompanyInvites()
    {
        var companyId = Guid.NewGuid();
        var invite1 = new InviteData(companyId, "user1@email.com", Severina.Domain.Enums.PapelUsuario.Operacional, DateTime.UtcNow, DateTime.UtcNow.AddDays(7));
        var invite2 = new InviteData(companyId, "user2@email.com", Severina.Domain.Enums.PapelUsuario.Administrador, DateTime.UtcNow, DateTime.UtcNow.AddDays(7));
        var invite3 = new InviteData(Guid.NewGuid(), "user3@email.com", Severina.Domain.Enums.PapelUsuario.Operacional, DateTime.UtcNow, DateTime.UtcNow.AddDays(7));

        await _cacheService.SaveInviteAsync("code1", invite1);
        await _cacheService.SaveInviteAsync("code2", invite2);
        await _cacheService.SaveInviteAsync("code3", invite3);

        var result = await _cacheService.GetPendingInvitesAsync(companyId);
        Assert.Equal(2, result.Count);
    }

    [Fact]
    public async Task SaveInvite_StoresCorrectRole()
    {
        var invite = new InviteData(
            Guid.NewGuid(),
            "admin@email.com",
            Severina.Domain.Enums.PapelUsuario.Administrador,
            DateTime.UtcNow,
            DateTime.UtcNow.AddDays(7));

        await _cacheService.SaveInviteAsync("admin-code", invite);

        var result = await _cacheService.GetInviteAsync("admin-code");
        Assert.NotNull(result);
        Assert.Equal(Severina.Domain.Enums.PapelUsuario.Administrador, result.Papel);
    }

    [Fact]
    public async Task SaveInvite_StoresCompanyId()
    {
        var companyId = Guid.NewGuid();
        var invite = new InviteData(
            companyId,
            "company@email.com",
            Severina.Domain.Enums.PapelUsuario.Operacional,
            DateTime.UtcNow,
            DateTime.UtcNow.AddDays(7));

        await _cacheService.SaveInviteAsync("company-code", invite);

        var result = await _cacheService.GetInviteAsync("company-code");
        Assert.NotNull(result);
        Assert.Equal(companyId, result.CompanyId);
    }
}
