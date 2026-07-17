using MediatR;
using Severina.Domain.Interfaces;

namespace Severina.Application.Commands.Users;

public record ActivateUserCommand(Guid CompanyId, Guid UserId) : IRequest<Unit>;

public class ActivateUserCommandHandler : IRequestHandler<ActivateUserCommand, Unit>
{
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;

    public ActivateUserCommandHandler(
        IUserRepository userRepository,
        IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Unit> Handle(ActivateUserCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(request.UserId)
            ?? throw new InvalidOperationException("Usuário não encontrado");

        if (user.CompanyId != request.CompanyId)
            throw new InvalidOperationException("Usuário não pertence a esta empresa");

        user.Activate();

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
