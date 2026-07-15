using MediatR;
using Severina.Application.DTOs;
using Severina.Domain.Enums;
using Severina.Domain.Interfaces;
using Severina.Domain.ValueObjects;

namespace Severina.Application.Commands.Clients;

public record CreateInteractionCommand(
    Guid CompanyId,
    Guid ClientId,
    InteractionType Type,
    string Content,
    Guid? ConversationId) : IRequest<InteractionResponse>;

public class CreateInteractionCommandHandler : IRequestHandler<CreateInteractionCommand, InteractionResponse>
{
    private readonly IUnitOfWork _unitOfWork;

    public CreateInteractionCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<InteractionResponse> Handle(CreateInteractionCommand request, CancellationToken cancellationToken)
    {
        var client = await _unitOfWork.Clients.GetByIdAsync(request.ClientId);

        if (client is null || client.CompanyId != request.CompanyId)
            throw new KeyNotFoundException("Cliente não encontrado");

        var interaction = Domain.Entities.Interaction.Create(
            request.ClientId, request.CompanyId, request.Type,
            request.Content, null, request.ConversationId);

        await _unitOfWork.Interactions.AddAsync(interaction);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return MapToResponse(interaction);
    }

    private static InteractionResponse MapToResponse(Domain.Entities.Interaction i)
    {
        return new InteractionResponse(
            i.Id, i.ClientId, i.Type, i.Content,
            i.Metadata?.Direction, i.Metadata?.DurationSeconds, i.Metadata?.Status,
            i.ConversationId, i.CreatedAt);
    }
}
