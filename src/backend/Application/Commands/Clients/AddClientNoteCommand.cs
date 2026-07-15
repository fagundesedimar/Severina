using MediatR;
using Severina.Application.DTOs;
using Severina.Domain.Interfaces;

namespace Severina.Application.Commands.Clients;

public record AddClientNoteCommand(
    Guid CompanyId,
    Guid ClientId,
    Guid AuthorId,
    string Content) : IRequest<ClientNoteResponse>;

public class AddClientNoteCommandHandler : IRequestHandler<AddClientNoteCommand, ClientNoteResponse>
{
    private readonly IUnitOfWork _unitOfWork;

    public AddClientNoteCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<ClientNoteResponse> Handle(AddClientNoteCommand request, CancellationToken cancellationToken)
    {
        var client = await _unitOfWork.Clients.GetByIdAsync(request.ClientId);

        if (client is null || client.CompanyId != request.CompanyId)
            throw new KeyNotFoundException("Cliente não encontrado");

        client.AddNote(request.Content, request.AuthorId);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var note = client.Notes.Last();
        return new ClientNoteResponse(note.Id, note.Content, note.AuthorId, note.CreatedAt);
    }
}
