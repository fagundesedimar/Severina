using MediatR;
using Severina.Domain.Enums;
using Severina.Domain.Interfaces;

namespace Severina.Application.Commands.Users;

public record UpdateUserRoleCommand(Guid CompanyId, Guid UserId, PapelUsuario Papel) : IRequest<Unit>;

public class UpdateUserRoleCommandHandler : IRequestHandler<UpdateUserRoleCommand, Unit>
{
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateUserRoleCommandHandler(
        IUserRepository userRepository,
        IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Unit> Handle(UpdateUserRoleCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(request.UserId)
            ?? throw new InvalidOperationException("Usuário não encontrado");

        if (user.CompanyId != request.CompanyId)
            throw new InvalidOperationException("Usuário não pertence a esta empresa");

        user.ChangeRole(request.Papel);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
