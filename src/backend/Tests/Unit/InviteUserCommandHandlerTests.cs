using Severina.Application.Commands.Users;
using Severina.Application.DTOs;
using Severina.Application.Interfaces;
using Severina.Domain.Enums;
using Severina.Domain.Interfaces;
using Xunit;

namespace Severina.Tests.Unit;

public class InviteUserCommandHandlerTests
{
    private class FakeUserRepository : IUserRepository
    {
        private readonly Dictionary<(Guid, string), Domain.Entities.User> _users = new();

        public void AddUser(Guid companyId, string email, Domain.Entities.User user)
        {
            _users[(companyId, email)] = user;
        }

        public Task<Domain.Entities.User?> GetByEmailAsync(Guid companyId, string email)
        {
            _users.TryGetValue((companyId, email), out var user);
            return Task.FromResult(user);
        }

        public Task<Domain.Entities.User?> GetByEmailAsync(string email) =>
            Task.FromResult<Domain.Entities.User?>(null);
        public Task<IReadOnlyList<Domain.Entities.User>> GetByCompanyIdAsync(Guid companyId) =>
            Task.FromResult<IReadOnlyList<Domain.Entities.User>>(Array.Empty<Domain.Entities.User>());
        public Task<Domain.Entities.User?> GetByIdAsync(Guid id) =>
            Task.FromResult<Domain.Entities.User?>(null);
        public Task<IReadOnlyList<Domain.Entities.User>> GetAllAsync() =>
            Task.FromResult<IReadOnlyList<Domain.Entities.User>>(Array.Empty<Domain.Entities.User>());
        public Task<Domain.Entities.User> AddAsync(Domain.Entities.User entity) =>
            Task.FromResult(entity);
        public Task UpdateAsync(Domain.Entities.User entity) => Task.CompletedTask;
        public Task DeleteAsync(Domain.Entities.User entity) => Task.CompletedTask;
        public Task<int> SaveChangesAsync() => Task.FromResult(0);
    }

    private class FakeInviteCacheService : IInviteCacheService
    {
        private readonly Dictionary<string, InviteData> _invites = new();
        public List<(string Code, InviteData Invite)> SavedInvites { get; } = new();

        public Task SaveInviteAsync(string code, InviteData invite)
        {
            _invites[code] = invite;
            SavedInvites.Add((code, invite));
            return Task.CompletedTask;
        }

        public Task<InviteData?> GetInviteAsync(string code)
        {
            _invites.TryGetValue(code, out var invite);
            return Task.FromResult(invite);
        }

        public Task DeleteInviteAsync(string code)
        {
            _invites.Remove(code);
            return Task.CompletedTask;
        }

        public Task<List<InviteData>> GetPendingInvitesAsync(Guid companyId)
        {
            var result = _invites.Values.Where(i => i.CompanyId == companyId).ToList();
            return Task.FromResult(result);
        }
    }

    private class FakeEmailService : IEmailService
    {
        public List<(string Email, string Code, string BaseUrl)> SentEmails { get; } = new();
        public bool ShouldFail { get; set; }

        public Task SendInviteEmailAsync(string email, string code, string baseUrl)
        {
            if (ShouldFail)
                throw new Exception("Resend API error");
            SentEmails.Add((email, code, baseUrl));
            return Task.CompletedTask;
        }
    }

    private class FakeUnitOfWork : IUnitOfWork
    {
        public ICompanyRepository Companies => throw new NotImplementedException();
        public IUserRepository Users => throw new NotImplementedException();
        public IAppointmentRepository Appointments => throw new NotImplementedException();
        public IClientRepository Clients => throw new NotImplementedException();
        public IInteractionRepository Interactions => throw new NotImplementedException();
        public IImportJobRepository ImportJobs => throw new NotImplementedException();
        public ITransactionRepository Transactions => throw new NotImplementedException();
        public IInvoiceRepository Invoices => throw new NotImplementedException();
        public IExportJobRepository ExportJobs => throw new NotImplementedException();
        public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default) =>
            Task.FromResult(0);
        public void Dispose() { }
    }

    private readonly FakeUserRepository _userRepository;
    private readonly FakeInviteCacheService _inviteCacheService;
    private readonly FakeEmailService _emailService;
    private readonly InviteUserCommandHandler _handler;

    public InviteUserCommandHandlerTests()
    {
        _userRepository = new FakeUserRepository();
        _inviteCacheService = new FakeInviteCacheService();
        _emailService = new FakeEmailService();
        _handler = new InviteUserCommandHandler(
            _userRepository,
            _inviteCacheService,
            _emailService,
            new FakeUnitOfWork());
    }

    [Fact]
    public async Task Handle_ValidInvite_CreatesInviteAndSendsEmail()
    {
        var companyId = Guid.NewGuid();
        var command = new InviteUserCommand(companyId, "new@email.com", PapelUsuario.Operacional);

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.Equal("new@email.com", result.Email);
        Assert.Equal(PapelUsuario.Operacional, result.Papel);
        Assert.Equal(StatusUsuario.Pendente, result.Status);
        Assert.Single(_inviteCacheService.SavedInvites);
        Assert.Single(_emailService.SentEmails);
        Assert.Equal("new@email.com", _emailService.SentEmails[0].Email);
    }

    [Fact]
    public async Task Handle_ExistingUser_ThrowsInvalidOperationException()
    {
        var companyId = Guid.NewGuid();
        var existingUser = new Domain.Entities.User(
            companyId, "Existing",
            Domain.ValueObjects.Email.Create("existing@email.com"),
            "hash", PapelUsuario.Operacional);
        _userRepository.AddUser(companyId, "existing@email.com", existingUser);

        var command = new InviteUserCommand(companyId, "existing@email.com", PapelUsuario.Operacional);

        await Assert.ThrowsAsync<InvalidOperationException>(
            () => _handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_EmailServiceFails_StillCreatesInvite()
    {
        var companyId = Guid.NewGuid();
        _emailService.ShouldFail = true;

        var command = new InviteUserCommand(companyId, "fail@email.com", PapelUsuario.Administrador);

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.Equal("fail@email.com", result.Email);
        Assert.Equal(PapelUsuario.Administrador, result.Papel);
        Assert.Single(_inviteCacheService.SavedInvites);
        Assert.Empty(_emailService.SentEmails);
    }

    [Fact]
    public async Task Handle_CustomAppUrl_PassesUrlToEmailService()
    {
        var companyId = Guid.NewGuid();
        var command = new InviteUserCommand(companyId, "url@email.com", PapelUsuario.Operacional, "https://custom.app.com");

        await _handler.Handle(command, CancellationToken.None);

        Assert.Single(_emailService.SentEmails);
        Assert.Equal("https://custom.app.com", _emailService.SentEmails[0].BaseUrl);
    }

    [Fact]
    public async Task Handle_InviteDataHasCorrectExpiration()
    {
        var companyId = Guid.NewGuid();
        var command = new InviteUserCommand(companyId, "exp@email.com", PapelUsuario.Operacional);

        await _handler.Handle(command, CancellationToken.None);

        Assert.Single(_inviteCacheService.SavedInvites);
        var invite = _inviteCacheService.SavedInvites[0].Invite;
        Assert.Equal(companyId, invite.CompanyId);
        Assert.Equal("exp@email.com", invite.Email);
        Assert.True(invite.ExpiresAt > DateTime.UtcNow);
        Assert.True(invite.ExpiresAt <= DateTime.UtcNow.AddDays(7));
    }

    [Fact]
    public async Task Handle_GeneratesUniqueCode()
    {
        var companyId = Guid.NewGuid();
        var command1 = new InviteUserCommand(companyId, "user1@email.com", PapelUsuario.Operacional);
        var command2 = new InviteUserCommand(companyId, "user2@email.com", PapelUsuario.Operacional);

        await _handler.Handle(command1, CancellationToken.None);
        await _handler.Handle(command2, CancellationToken.None);

        var code1 = _inviteCacheService.SavedInvites[0].Code;
        var code2 = _inviteCacheService.SavedInvites[1].Code;
        Assert.NotEqual(code1, code2);
        Assert.Equal(32, code1.Length);
        Assert.Equal(32, code2.Length);
    }
}
