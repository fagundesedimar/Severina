using FluentValidation;
using MediatR;
using Severina.Application.DTOs;
using Severina.Application.Interfaces;
using Severina.Domain.Enums;
using Severina.Domain.Interfaces;

namespace Severina.Application.Commands.Users;

public record InviteUserCommand(
    Guid CompanyId,
    string Email,
    PapelUsuario Papel,
    string AppUrl = "http://localhost:3000") : IRequest<UserResponse>;

public class InviteUserCommandValidator : AbstractValidator<InviteUserCommand>
{
    public InviteUserCommandValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email é obrigatório")
            .EmailAddress().WithMessage("Formato de email inválido");
    }
}

public class InviteUserCommandHandler : IRequestHandler<InviteUserCommand, UserResponse>
{
    private readonly IUserRepository _userRepository;
    private readonly IInviteCacheService _inviteCacheService;
    private readonly IEmailService _emailService;
    private readonly IUnitOfWork _unitOfWork;

    public InviteUserCommandHandler(
        IUserRepository userRepository,
        IInviteCacheService inviteCacheService,
        IEmailService emailService,
        IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _inviteCacheService = inviteCacheService;
        _emailService = emailService;
        _unitOfWork = unitOfWork;
    }

    public async Task<UserResponse> Handle(InviteUserCommand request, CancellationToken cancellationToken)
    {
        var existingUser = await _userRepository.GetByEmailAsync(request.CompanyId, request.Email);
        if (existingUser != null)
            throw new InvalidOperationException("Usuário já pertence à empresa");

        var code = Guid.NewGuid().ToString("N")[..32];
        var inviteData = new InviteData(
            code,
            request.CompanyId,
            request.Email,
            request.Papel,
            DateTime.UtcNow,
            DateTime.UtcNow.AddDays(7));

        await _inviteCacheService.SaveInviteAsync(code, inviteData);

        try
        {
            await _emailService.SendInviteEmailAsync(
                request.Email,
                code,
                request.AppUrl);
        }
        catch
        {
            // Email failure is non-fatal
        }

        return new UserResponse(
            Guid.Empty,
            "",
            request.Email,
            request.Papel,
            StatusUsuario.Pendente,
            DateTime.UtcNow);
    }
}

public record InviteData(
    string Code,
    Guid CompanyId,
    string Email,
    PapelUsuario Papel,
    DateTime CreatedAt,
    DateTime ExpiresAt);
