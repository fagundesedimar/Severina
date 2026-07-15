using MediatR;
using Severina.Domain.Interfaces;

namespace Severina.Application.Commands.Users;

public record DeactivateUserCommand(Guid CompanyId, Guid UserId) : IRequest<Unit>;

public class DeactivateUserCommandHandler : IRequestHandler<DeactivateUserCommand, Unit>
{
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DeactivateUserCommandHandler(
        IUserRepository userRepository,
        IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Unit> Handle(DeactivateUserCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(request.UserId)
            ?? throw new InvalidOperationException("Usuário não encontrado");

        if (user.CompanyId != request.CompanyId)
            throw new InvalidOperationException("Usuário não pertence a esta empresa");

        if (user.IsAdministrador)
            throw new InvalidOperationException("Administrador não pode desativar a si mesmo");

        user.Deactivate();

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
