using FluentValidation;
using MediatR;
using Severina.Application.DTOs;
using Severina.Application.Interfaces;
using Severina.Domain.Entities;
using Severina.Domain.Enums;
using Severina.Domain.Interfaces;
using Severina.Domain.ValueObjects;

namespace Severina.Application.Commands.Users;

public record AcceptInviteCommand(
    string Code,
    string Nome,
    string Senha) : IRequest<UserResponse>;

public class AcceptInviteCommandValidator : AbstractValidator<AcceptInviteCommand>
{
    public AcceptInviteCommandValidator()
    {
        RuleFor(x => x.Code)
            .NotEmpty().WithMessage("Código do convite é obrigatório");

        RuleFor(x => x.Nome)
            .NotEmpty().WithMessage("Nome é obrigatório")
            .MaximumLength(200).WithMessage("Nome deve ter no máximo 200 caracteres");

        RuleFor(x => x.Senha)
            .NotEmpty().WithMessage("Senha é obrigatória")
            .MinimumLength(8).WithMessage("Senha deve ter no mínimo 8 caracteres");
    }
}

public class AcceptInviteCommandHandler : IRequestHandler<AcceptInviteCommand, UserResponse>
{
    private readonly IUserRepository _userRepository;
    private readonly IInviteCacheService _inviteCacheService;
    private readonly IPasswordService _passwordService;
    private readonly IUnitOfWork _unitOfWork;

    public AcceptInviteCommandHandler(
        IUserRepository userRepository,
        IInviteCacheService inviteCacheService,
        IPasswordService passwordService,
        IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _inviteCacheService = inviteCacheService;
        _passwordService = passwordService;
        _unitOfWork = unitOfWork;
    }

    public async Task<UserResponse> Handle(AcceptInviteCommand request, CancellationToken cancellationToken)
    {
        var inviteData = await _inviteCacheService.GetInviteAsync(request.Code)
            ?? throw new KeyNotFoundException("Convite não encontrado");

        if (inviteData.ExpiresAt < DateTime.UtcNow)
        {
            await _inviteCacheService.DeleteInviteAsync(request.Code);
            throw new InvalidOperationException("Convite expirado");
        }

        var email = Email.Create(inviteData.Email);
        var senhaHash = _passwordService.HashPassword(request.Senha);

        var user = new User(
            inviteData.CompanyId,
            request.Nome,
            email,
            senhaHash,
            inviteData.Papel);

        await _userRepository.AddAsync(user);
        await _inviteCacheService.DeleteInviteAsync(request.Code);
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
