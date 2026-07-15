using MediatR;
using Severina.Application.DTOs;
using Severina.Domain.Interfaces;

namespace Severina.Application.Commands.Clients;

public record AddClientTagCommand(
    Guid CompanyId,
    Guid ClientId,
    string TagName) : IRequest<ClientResponse>;

public class AddClientTagCommandHandler : IRequestHandler<AddClientTagCommand, ClientResponse>
{
    private readonly IUnitOfWork _unitOfWork;

    public AddClientTagCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<ClientResponse> Handle(AddClientTagCommand request, CancellationToken cancellationToken)
    {
        var client = await _unitOfWork.Clients.GetByIdAsync(request.ClientId);

        if (client is null || client.CompanyId != request.CompanyId)
            throw new KeyNotFoundException("Cliente não encontrado");

        client.AddTag(request.TagName);
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
