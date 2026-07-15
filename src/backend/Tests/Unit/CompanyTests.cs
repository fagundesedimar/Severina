using Severina.Domain.Entities;
using Severina.Domain.Enums;
using Severina.Domain.ValueObjects;
using Xunit;

namespace Severina.Tests.Unit;

public class CompanyTests
{
    private static Company CreateValidCompany()
    {
        return new Company(
            "Empresa Teste",
            CnpjCpf.Create("52998224725"),
            Email.Create("teste@empresa.com"),
            TipoPessoa.Fisica,
            Telefone.Create("11999998888"));
    }

    [Fact]
    public void Create_ValidCompany_SetsStatusAtiva()
    {
        var company = CreateValidCompany();
        Assert.Equal(StatusEmpresa.Ativa, company.Status);
    }

    [Fact]
    public void Activate_InactiveCompany_SetsStatusAtiva()
    {
        var company = CreateValidCompany();
        company.Deactivate();
        company.Activate();
        Assert.Equal(StatusEmpresa.Ativa, company.Status);
    }

    [Fact]
    public void Deactivate_ActiveCompany_SetsStatusInativa()
    {
        var company = CreateValidCompany();
        company.Deactivate();
        Assert.Equal(StatusEmpresa.Inativa, company.Status);
    }

    [Fact]
    public void Suspend_ActiveCompany_SetsStatusSuspensa()
    {
        var company = CreateValidCompany();
        company.Suspend();
        Assert.Equal(StatusEmpresa.Suspensa, company.Status);
    }

    [Fact]
    public void Update_UpdatesProperties()
    {
        var company = CreateValidCompany();
        var newEmail = Email.Create("novo@empresa.com");
        var newTelefone = Telefone.Create("11888887777");

        company.Update("Nova Empresa", newEmail, newTelefone);

        Assert.Equal("Nova Empresa", company.Nome);
        Assert.Equal(newEmail.Value, company.Email.Value);
        Assert.Equal(newTelefone.Value, company.Telefone?.Value);
    }

    [Fact]
    public void AddUser_AddsToCollection()
    {
        var company = CreateValidCompany();
        var user = new User(company.Id, "Usuário", Email.Create("user@test.com"), "hash", PapelUsuario.Operacional);

        company.AddUser(user);

        Assert.Single(company.Users);
    }

    [Fact]
    public void RemoveUser_RemovesFromCollection()
    {
        var company = CreateValidCompany();
        var user = new User(company.Id, "Usuário", Email.Create("user@test.com"), "hash", PapelUsuario.Operacional);

        company.AddUser(user);
        company.RemoveUser(user);

        Assert.Empty(company.Users);
    }

    [Fact]
    public void SetConfiguracoes_StoresConfigurations()
    {
        var company = CreateValidCompany();
        var config = new Dictionary<string, object> { { "theme", "dark" }, { "notifications", true } };

        company.SetConfiguracoes(config);

        var retrieved = company.GetConfiguracoes();
        Assert.Equal("dark", retrieved["theme"].ToString());
    }
}
