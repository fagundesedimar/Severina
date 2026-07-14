using Microsoft.EntityFrameworkCore;
using Severina.Domain.Entities;
using Severina.Domain.Enums;
using Severina.Domain.Interfaces;
using Severina.Infrastructure.Data;
using Severina.Infrastructure.Repositories;
using Severina.Infrastructure.Services;
using Xunit;

namespace Severina.Tests.Integration;

public class TenantIsolationTests
{
    private readonly string _dbName = $"TestDb_{Guid.NewGuid()}";

    private SeverinaDbContext CreateContext(Guid? tenantCompanyId = null)
    {
        var options = new DbContextOptionsBuilder<SeverinaDbContext>()
            .UseInMemoryDatabase(_dbName)
            .Options;

        var tenantProvider = new TenantProvider();
        if (tenantCompanyId.HasValue)
            tenantProvider.SetCompanyId(tenantCompanyId.Value);

        var context = new SeverinaDbContext(options, tenantProvider);
        if (tenantCompanyId.HasValue)
            context.SetTenantCompanyId(tenantCompanyId.Value);
        return context;
    }

    [Fact]
    public async Task Users_Belonging_To_Different_Companies_Are_Isolated()
    {
        var company1Id = Guid.NewGuid();
        var company2Id = Guid.NewGuid();
        var hasher = new PasswordService();

        using (var context = CreateContext())
        {
            context.Companies.Add(new Company("Empresa 1", "12345678000190", "emp1@test.com", TipoPessoa.Juridica));
            context.Companies.Add(new Company("Empresa 2", "98765432000190", "emp2@test.com", TipoPessoa.Juridica));
            context.Users.Add(new User(company1Id, "User 1", "user1@test.com", hasher.HashPassword("Test123!"), PapelUsuario.Administrador));
            context.Users.Add(new User(company2Id, "User 2", "user2@test.com", hasher.HashPassword("Test123!"), PapelUsuario.Administrador));
            await context.SaveChangesAsync();
        }

        using (var context = CreateContext(company1Id))
        {
            var users = await context.Users.ToListAsync();
            Assert.Single(users);
            Assert.Equal("user1@test.com", users[0].Email);
        }
    }

    [Fact]
    public async Task GetByEmail_SameCompany_ReturnsUser()
    {
        var company1Id = Guid.NewGuid();
        var hasher = new PasswordService();

        using (var context = CreateContext())
        {
            context.Companies.Add(new Company("Empresa 1", "12345678000190", "emp1@test.com", TipoPessoa.Juridica));
            context.Users.Add(new User(company1Id, "User 1", "user1@test.com", hasher.HashPassword("Test123!"), PapelUsuario.Administrador));
            await context.SaveChangesAsync();
        }

        using (var context = CreateContext(company1Id))
        {
            var repo = new UserRepository(context);
            var user = await repo.GetByEmailAsync("user1@test.com");
            Assert.NotNull(user);
            Assert.Equal("user1@test.com", user.Email);
        }
    }

    [Fact]
    public async Task GetByEmail_DifferentCompany_ReturnsNull()
    {
        var company1Id = Guid.NewGuid();
        var company2Id = Guid.NewGuid();
        var hasher = new PasswordService();

        using (var context = CreateContext())
        {
            context.Companies.Add(new Company("Empresa 1", "12345678000190", "emp1@test.com", TipoPessoa.Juridica));
            context.Companies.Add(new Company("Empresa 2", "98765432000190", "emp2@test.com", TipoPessoa.Juridica));
            context.Users.Add(new User(company1Id, "User 1", "user1@test.com", hasher.HashPassword("Test123!"), PapelUsuario.Administrador));
            context.Users.Add(new User(company2Id, "User 2", "user2@test.com", hasher.HashPassword("Test123!"), PapelUsuario.Administrador));
            await context.SaveChangesAsync();
        }

        using (var context = CreateContext(company1Id))
        {
            var repo = new UserRepository(context);
            var user = await repo.GetByEmailAsync("user2@test.com");
            Assert.Null(user);
        }
    }

    [Fact]
    public async Task Company_Entity_Is_Not_Filtered_By_Tenant()
    {
        using (var context = CreateContext())
        {
            context.Companies.Add(new Company("Empresa 1", "12345678000190", "emp1@test.com", TipoPessoa.Juridica));
            context.Companies.Add(new Company("Empresa 2", "98765432000190", "emp2@test.com", TipoPessoa.Juridica));
            await context.SaveChangesAsync();
        }

        using (var context = CreateContext(Guid.NewGuid()))
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

        using (var context = CreateContext())
        {
            context.Companies.Add(new Company("Empresa 1", "12345678000190", "emp1@test.com", TipoPessoa.Juridica));
            context.Companies.Add(new Company("Empresa 2", "98765432000190", "emp2@test.com", TipoPessoa.Juridica));
            context.Users.Add(new User(company1Id, "User 1", "user1@test.com", hasher.HashPassword("Test123!"), PapelUsuario.Administrador));
            context.Users.Add(new User(company1Id, "User 1b", "user1b@test.com", hasher.HashPassword("Test123!"), PapelUsuario.Operacional));
            context.Users.Add(new User(company2Id, "User 2", "user2@test.com", hasher.HashPassword("Test123!"), PapelUsuario.Administrador));
            await context.SaveChangesAsync();
        }

        using (var context = CreateContext(company1Id))
        {
            var repo = new UserRepository(context);
            var users = await repo.GetByCompanyIdAsync(company1Id);
            Assert.Equal(2, users.Count);
            Assert.All(users, u => Assert.Equal(company1Id, u.CompanyId));
        }
    }
}
