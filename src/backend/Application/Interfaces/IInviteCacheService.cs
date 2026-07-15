using Severina.Application.Commands.Users;

namespace Severina.Application.Interfaces;

public interface IInviteCacheService
{
    Task SaveInviteAsync(string code, InviteData invite);
    Task<InviteData?> GetInviteAsync(string code);
    Task DeleteInviteAsync(string code);
    Task<List<InviteData>> GetPendingInvitesAsync(Guid companyId);
}
