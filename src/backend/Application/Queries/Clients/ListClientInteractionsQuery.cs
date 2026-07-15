using MediatR;
using Severina.Application.DTOs;
using Severina.Domain.Enums;
using Severina.Domain.Interfaces;

namespace Severina.Application.Queries.Clients;

public record ListClientInteractionsQuery(
    Guid CompanyId,
    Guid ClientId,
    InteractionType? Type = null,
    int Page = 1,
    int PageSize = 20) : IRequest<PagedInteractionResponse>;

public class ListClientInteractionsQueryHandler : IRequestHandler<ListClientInteractionsQuery, PagedInteractionResponse>
{
    private readonly IUnitOfWork _unitOfWork;

    public ListClientInteractionsQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<PagedInteractionResponse> Handle(ListClientInteractionsQuery request, CancellationToken cancellationToken)
    {
        var client = await _unitOfWork.Clients.GetByIdAsync(request.ClientId);
        if (client is null || client.CompanyId != request.CompanyId)
            throw new KeyNotFoundException("Cliente não encontrado");

        var skip = (request.Page - 1) * request.PageSize;
        IReadOnlyList<Domain.Entities.Interaction> interactions;
        int totalCount;

        if (request.Type.HasValue)
        {
            interactions = await _unitOfWork.Interactions.GetByClientIdAndTypeAsync(
                request.CompanyId, request.ClientId, request.Type.Value, skip, request.PageSize);
            totalCount = await _unitOfWork.Interactions.CountByClientIdAndTypeAsync(
                request.CompanyId, request.ClientId, request.Type.Value);
        }
        else
        {
            interactions = await _unitOfWork.Interactions.GetByClientIdAsync(
                request.CompanyId, request.ClientId, skip, request.PageSize);
            totalCount = await _unitOfWork.Interactions.CountByClientIdAsync(
                request.CompanyId, request.ClientId);
        }

        var items = interactions.Select(i => new InteractionResponse(
            i.Id, i.ClientId, i.Type, i.Content,
            i.Metadata?.Direction, i.Metadata?.DurationSeconds, i.Metadata?.Status,
            i.ConversationId, i.CreatedAt)).ToList();

        return new PagedInteractionResponse(items, totalCount, request.Page, request.PageSize);
    }
}
