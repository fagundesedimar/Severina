using MediatR;
using Severina.Application.DTOs;
using Severina.Domain.Interfaces;

namespace Severina.Application.Commands.Users;

public record UpdateUserProfileCommand(
    Guid UserId,
    string Nome,
    string? Telefone) : IRequest<UserResponse>;

public class UpdateUserProfileCommandHandler : IRequestHandler<UpdateUserProfileCommand, UserResponse>
{
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateUserProfileCommandHandler(
        IUserRepository userRepository,
        IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<UserResponse> Handle(UpdateUserProfileCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(request.UserId)
            ?? throw new InvalidOperationException("Usuário não encontrado");

        user.UpdateProfile(request.Nome, request.Telefone);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new UserResponse(
            user.Id,
            user.Nome,
            user.Email.Value,
            user.Papel,
            user.Status,
            user.CreatedAt);
    }
}
