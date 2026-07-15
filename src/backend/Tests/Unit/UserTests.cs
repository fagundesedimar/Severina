using Severina.Domain.Entities;
using Severina.Domain.Enums;
using Severina.Domain.ValueObjects;
using Xunit;

namespace Severina.Tests.Unit;

public class UserTests
{
    private static User CreateValidUser()
    {
        return new User(
            Guid.NewGuid(),
            "Usuário Teste",
            Email.Create("teste@empresa.com"),
            "senha_hash",
            PapelUsuario.Operacional);
    }

    [Fact]
    public void Create_ValidUser_SetsStatusAtivo()
    {
        var user = CreateValidUser();
        Assert.Equal(StatusUsuario.Ativo, user.Status);
    }

    [Fact]
    public void Activate_InactiveUser_SetsStatusAtivo()
    {
        var user = CreateValidUser();
        user.Deactivate();
        user.Activate();
        Assert.Equal(StatusUsuario.Ativo, user.Status);
    }

    [Fact]
    public void Deactivate_ActiveUser_SetsStatusInativo()
    {
        var user = CreateValidUser();
        user.Deactivate();
        Assert.Equal(StatusUsuario.Inativo, user.Status);
    }

    [Fact]
    public void ChangeRole_ChangesPapel()
    {
        var user = CreateValidUser();
        user.ChangeRole(PapelUsuario.Administrador);
        Assert.Equal(PapelUsuario.Administrador, user.Papel);
    }

    [Fact]
    public void IsAdministrador_AdminUser_ReturnsTrue()
    {
        var user = CreateValidUser();
        user.ChangeRole(PapelUsuario.Administrador);
        Assert.True(user.IsAdministrador);
    }

    [Fact]
    public void IsAdministrador_OperacionalUser_ReturnsFalse()
    {
        var user = CreateValidUser();
        Assert.False(user.IsAdministrador);
    }

    [Fact]
    public void IsActive_ActiveUser_ReturnsTrue()
    {
        var user = CreateValidUser();
        Assert.True(user.IsActive);
    }

    [Fact]
    public void IsActive_InactiveUser_ReturnsFalse()
    {
        var user = CreateValidUser();
        user.Deactivate();
        Assert.False(user.IsActive);
    }

    [Fact]
    public void UpdateProfile_UpdatesNome()
    {
        var user = CreateValidUser();
        user.UpdateProfile("Novo Nome", null);
        Assert.Equal("Novo Nome", user.Nome);
    }

    [Fact]
    public void Create_SetsCorrectCompanyId()
    {
        var companyId = Guid.NewGuid();
        var user = new User(companyId, "Usuário", Email.Create("teste@empresa.com"), "hash", PapelUsuario.Operacional);
        Assert.Equal(companyId, user.CompanyId);
    }
}
