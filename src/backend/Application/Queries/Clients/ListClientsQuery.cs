using MediatR;
using Severina.Application.DTOs;
using Severina.Domain.Interfaces;

namespace Severina.Application.Queries.Clients;

public record ListClientsQuery(
    Guid CompanyId,
    int Page = 1,
    int PageSize = 20) : IRequest<PagedClientResponse>;

public class ListClientsQueryHandler : IRequestHandler<ListClientsQuery, PagedClientResponse>
{
    private readonly IUnitOfWork _unitOfWork;

    public ListClientsQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<PagedClientResponse> Handle(ListClientsQuery request, CancellationToken cancellationToken)
    {
        var allClients = await _unitOfWork.Clients.GetAllAsync();
        var companyClients = allClients.Where(c => c.CompanyId == request.CompanyId).ToList();

        var totalCount = companyClients.Count;
        var items = companyClients
            .OrderByDescending(c => c.CreatedAt)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(c => new ClientResponse(
                c.Id, c.CompanyId, c.Nome,
                c.Email?.Value, c.Telefone, c.Empresa,
                c.Status,
                c.Tags.Select(t => t.Name).ToList(),
                c.Notes.Select(n => new ClientNoteResponse(n.Id, n.Content, n.AuthorId, n.CreatedAt)).ToList(),
                c.CreatedAt, c.UpdatedAt))
            .ToList();

        return new PagedClientResponse(items, totalCount, request.Page, request.PageSize);
    }
}
