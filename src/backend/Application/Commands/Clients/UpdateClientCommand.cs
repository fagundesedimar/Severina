using FluentValidation;
using MediatR;
using Severina.Application.DTOs;
using Severina.Domain.Interfaces;

namespace Severina.Application.Commands.Clients;

public record UpdateClientCommand(
    Guid CompanyId,
    Guid ClientId,
    string Nome,
    string? Email,
    string? Telefone,
    string? Empresa) : IRequest<ClientResponse>;

public class UpdateClientCommandValidator : AbstractValidator<UpdateClientCommand>
{
    public UpdateClientCommandValidator()
    {
        RuleFor(x => x.Nome)
            .NotEmpty().WithMessage("Nome é obrigatório")
            .MaximumLength(200).WithMessage("Nome deve ter no máximo 200 caracteres");
    }
}

public class UpdateClientCommandHandler : IRequestHandler<UpdateClientCommand, ClientResponse>
{
    private readonly IUnitOfWork _unitOfWork;

    public UpdateClientCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<ClientResponse> Handle(UpdateClientCommand request, CancellationToken cancellationToken)
    {
        var client = await _unitOfWork.Clients.GetByIdAsync(request.ClientId);

        if (client is null || client.CompanyId != request.CompanyId)
            throw new KeyNotFoundException("Cliente não encontrado");

        if (!string.IsNullOrWhiteSpace(request.Email) && request.Email != client.Email?.Value)
        {
            var exists = await _unitOfWork.Clients.ExistsByEmailAsync(request.CompanyId, request.Email, request.ClientId);
            if (exists)
                throw new InvalidOperationException("Cliente com este email já existe");
        }

        client.UpdateContactInfo(request.Nome, request.Email, request.Telefone, request.Empresa);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new ClientResponse(
            client.Id, client.CompanyId, client.Nome,
            client.Email?.Value, client.Telefone, client.Empresa,
            client.Status,
            client.Tags.Select(t => t.Name).ToList(),
            client.Notes.Select(n => new ClientNoteResponse(n.Id, n.Content, n.AuthorId, n.CreatedAt)).ToList(),
            client.CreatedAt, client.UpdatedAt);
    }
}
