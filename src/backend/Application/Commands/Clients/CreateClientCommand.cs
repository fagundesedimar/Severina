using FluentValidation;
using MediatR;
using Severina.Application.DTOs;
using Severina.Domain.Interfaces;

namespace Severina.Application.Commands.Clients;

public record CreateClientCommand(
    Guid CompanyId,
    string Nome,
    string? Email,
    string? Telefone,
    string? Empresa) : IRequest<ClientResponse>;

public class CreateClientCommandValidator : AbstractValidator<CreateClientCommand>
{
    public CreateClientCommandValidator()
    {
        RuleFor(x => x.Nome)
            .NotEmpty().WithMessage("Nome é obrigatório")
            .MaximumLength(200).WithMessage("Nome deve ter no máximo 200 caracteres");

        RuleFor(x => x.Telefone)
            .MaximumLength(20).WithMessage("Telefone deve ter no máximo 20 caracteres");

        RuleFor(x => x.Empresa)
            .MaximumLength(200).WithMessage("Empresa deve ter no máximo 200 caracteres");
    }
}

public class CreateClientCommandHandler : IRequestHandler<CreateClientCommand, ClientResponse>
{
    private readonly IUnitOfWork _unitOfWork;

    public CreateClientCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<ClientResponse> Handle(CreateClientCommand request, CancellationToken cancellationToken)
    {
        if (!string.IsNullOrWhiteSpace(request.Email))
        {
            var exists = await _unitOfWork.Clients.ExistsByEmailAsync(request.CompanyId, request.Email);
            if (exists)
                throw new InvalidOperationException("Cliente com este email já existe");
        }

        var client = new Domain.Entities.Client(
            request.CompanyId, request.Nome, request.Email, request.Telefone, request.Empresa);

        await _unitOfWork.Clients.AddAsync(client);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return MapToResponse(client);
    }

    private static ClientResponse MapToResponse(Domain.Entities.Client c)
    {
        return new ClientResponse(
            c.Id, c.CompanyId, c.Nome,
            c.Email?.Value, c.Telefone, c.Empresa,
            c.Status,
            c.Tags.Select(t => t.Name).ToList(),
            c.Notes.Select(n => new ClientNoteResponse(n.Id, n.Content, n.AuthorId, n.CreatedAt)).ToList(),
            c.CreatedAt, c.UpdatedAt);
    }
}
