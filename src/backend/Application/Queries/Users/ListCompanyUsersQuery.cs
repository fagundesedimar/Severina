using MediatR;
using Severina.Application.DTOs;
using Severina.Domain.Interfaces;

namespace Severina.Application.Queries.Users;

public record ListCompanyUsersQuery(Guid CompanyId) : IRequest<List<UserResponse>>;

public class ListCompanyUsersQueryHandler : IRequestHandler<ListCompanyUsersQuery, List<UserResponse>>
{
    private readonly IUserRepository _userRepository;

    public ListCompanyUsersQueryHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<List<UserResponse>> Handle(ListCompanyUsersQuery request, CancellationToken cancellationToken)
    {
        var users = await _userRepository.GetByCompanyIdAsync(request.CompanyId);

        return users.Select(u => new UserResponse(
            u.Id,
            u.Nome,
            u.Email.Value,
            u.Papel,
            u.Status,
            u.CreatedAt)).ToList();
    }
}
