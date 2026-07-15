using MediatR;
using Severina.Application.DTOs;
using Severina.Domain.Interfaces;

namespace Severina.Application.Queries.Clients;

public record GetClientByIdQuery(
    Guid CompanyId,
    Guid ClientId) : IRequest<ClientResponse?>;

public class GetClientByIdQueryHandler : IRequestHandler<GetClientByIdQuery, ClientResponse?>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetClientByIdQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<ClientResponse?> Handle(GetClientByIdQuery request, CancellationToken cancellationToken)
    {
        var client = await _unitOfWork.Clients.GetByIdAsync(request.ClientId);

        if (client is null || client.CompanyId != request.CompanyId)
            return null;

        return new ClientResponse(
            client.Id, client.CompanyId, client.Nome,
            client.Email?.Value, client.Telefone, client.Empresa,
            client.Status,
            client.Tags.Select(t => t.Name).ToList(),
            client.Notes.Select(n => new ClientNoteResponse(n.Id, n.Content, n.AuthorId, n.CreatedAt)).ToList(),
            client.CreatedAt, client.UpdatedAt);
    }
}
