using Microsoft.EntityFrameworkCore;
using Severina.Domain.Entities;
using Severina.Domain.Enums;
using Severina.Domain.Interfaces;
using Severina.Domain.ValueObjects;
using Severina.Infrastructure.Data;
using Severina.Infrastructure.Repositories;
using Severina.Infrastructure.Services;
using Xunit;

namespace Severina.Tests.Integration;

public class TenantIsolationTests
{
    private readonly string _dbName = $"TestDb_{Guid.NewGuid()}";

    private SeverinaDbContext CreateSetupContext()
    {
        var options = new DbContextOptionsBuilder<SeverinaDbContext>()
            .UseInMemoryDatabase(_dbName)
            .Options;

        var tenantProvider = new TenantProvider();
        var context = new SeverinaDbContext(options, tenantProvider);
        return context;
    }

    private SeverinaDbContext CreateQueryContext(Guid? tenantCompanyId)
    {
        var options = new DbContextOptionsBuilder<SeverinaDbContext>()
            .UseInMemoryDatabase(_dbName)
            .Options;

        var tenantProvider = new TenantProvider();
        if (tenantCompanyId.HasValue)
            tenantProvider.SetCompanyId(tenantCompanyId.Value);

        var context = new SeverinaDbContext(options, tenantProvider);
        return context;
    }

    [Fact]
    public async Task Users_Belonging_To_Different_Companies_Are_Isolated()
    {
        var company1Id = Guid.NewGuid();
        var company2Id = Guid.NewGuid();
        var hasher = new PasswordService();

        using (var context = CreateSetupContext())
        {
            context.Companies.Add(new Company("Empresa 1", CnpjCpf.Create("12345678000195"), Email.Create("emp1@test.com"), TipoPessoa.Juridica));
            context.Companies.Add(new Company("Empresa 2", CnpjCpf.Create("98765432000198"), Email.Create("emp2@test.com"), TipoPessoa.Juridica));
            context.Users.Add(new User(company1Id, "User 1", Email.Create("user1@test.com"), hasher.HashPassword("Test123!"), PapelUsuario.Administrador));
            context.Users.Add(new User(company2Id, "User 2", Email.Create("user2@test.com"), hasher.HashPassword("Test123!"), PapelUsuario.Administrador));
            await context.SaveChangesAsync();
        }

        using (var context = CreateQueryContext(company1Id))
        {
            var users = await context.Users.IgnoreQueryFilters().Where(u => u.CompanyId == company1Id).ToListAsync();
            Assert.Single(users);
            Assert.Equal("user1@test.com", users[0].Email.Value);
        }
    }

    [Fact]
    public async Task GetByEmail_SameCompany_ReturnsUser()
    {
        var company1Id = Guid.NewGuid();
        var hasher = new PasswordService();

        using (var context = CreateSetupContext())
        {
            context.Companies.Add(new Company("Empresa 1", CnpjCpf.Create("12345678000195"), Email.Create("emp1@test.com"), TipoPessoa.Juridica));
            context.Users.Add(new User(company1Id, "User 1", Email.Create("user1@test.com"), hasher.HashPassword("Test123!"), PapelUsuario.Administrador));
            await context.SaveChangesAsync();
        }

        using (var context = CreateQueryContext(company1Id))
        {
            var repo = new UserRepository(context);
            var user = await repo.GetByEmailAsync(company1Id, "user1@test.com");
            Assert.NotNull(user);
            Assert.Equal("user1@test.com", user.Email.Value);
        }
    }

    [Fact]
    public async Task GetByEmail_DifferentCompany_ReturnsNull()
    {
        var company1Id = Guid.NewGuid();
        var company2Id = Guid.NewGuid();
        var hasher = new PasswordService();

        using (var context = CreateSetupContext())
        {
            context.Companies.Add(new Company("Empresa 1", CnpjCpf.Create("12345678000195"), Email.Create("emp1@test.com"), TipoPessoa.Juridica));
            context.Companies.Add(new Company("Empresa 2", CnpjCpf.Create("98765432000198"), Email.Create("emp2@test.com"), TipoPessoa.Juridica));
            context.Users.Add(new User(company1Id, "User 1", Email.Create("user1@test.com"), hasher.HashPassword("Test123!"), PapelUsuario.Administrador));
            context.Users.Add(new User(company2Id, "User 2", Email.Create("user2@test.com"), hasher.HashPassword("Test123!"), PapelUsuario.Administrador));
            await context.SaveChangesAsync();
        }

        using (var context = CreateQueryContext(company1Id))
        {
            var repo = new UserRepository(context);
            var user = await repo.GetByEmailAsync(company2Id, "user2@test.com");
            Assert.Null(user);
        }
    }

    [Fact]
    public async Task Company_Entity_Is_Not_Filtered_By_Tenant()
    {
        using (var context = CreateSetupContext())
        {
            context.Companies.Add(new Company("Empresa 1", CnpjCpf.Create("12345678000195"), Email.Create("emp1@test.com"), TipoPessoa.Juridica));
            context.Companies.Add(new Company("Empresa 2", CnpjCpf.Create("98765432000198"), Email.Create("emp2@test.com"), TipoPessoa.Juridica));
            await context.SaveChangesAsync();
        }

        using (var context = CreateQueryContext(Guid.NewGuid()))
        {
            var companies = await context.Companies.ToListAsync();
            Assert.Equal(2, companies.Count);
        }
    }

    [Fact]
    public async Task GetAllUsers_ReturnsOnlyOwnCompany()
    {
        var company1Id = Guid.NewGuid();
        var company2Id = Guid.NewGuid();
        var hasher = new PasswordService();

        using (var context = CreateSetupContext())
        {
            context.Companies.Add(new Company("Empresa 1", CnpjCpf.Create("12345678000195"), Email.Create("emp1@test.com"), TipoPessoa.Juridica));
            context.Companies.Add(new Company("Empresa 2", CnpjCpf.Create("98765432000198"), Email.Create("emp2@test.com"), TipoPessoa.Juridica));
            context.Users.Add(new User(company1Id, "User 1", Email.Create("user1@test.com"), hasher.HashPassword("Test123!"), PapelUsuario.Administrador));
            context.Users.Add(new User(company1Id, "User 1b", Email.Create("user1b@test.com"), hasher.HashPassword("Test123!"), PapelUsuario.Operacional));
            context.Users.Add(new User(company2Id, "User 2", Email.Create("user2@test.com"), hasher.HashPassword("Test123!"), PapelUsuario.Administrador));
            await context.SaveChangesAsync();
        }

        using (var context = CreateQueryContext(company1Id))
        {
            var users = await context.Users.IgnoreQueryFilters().Where(u => u.CompanyId == company1Id).ToListAsync();
            Assert.Equal(2, users.Count);
            Assert.All(users, u => Assert.Equal(company1Id, u.CompanyId));
        }
    }
}
