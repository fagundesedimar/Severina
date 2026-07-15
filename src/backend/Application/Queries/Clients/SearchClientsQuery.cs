using MediatR;
using Severina.Application.DTOs;
using Severina.Domain.Interfaces;

namespace Severina.Application.Queries.Clients;

public record SearchClientsQuery(
    Guid CompanyId,
    string SearchTerm,
    int Page = 1,
    int PageSize = 20) : IRequest<PagedClientResponse>;

public class SearchClientsQueryHandler : IRequestHandler<SearchClientsQuery, PagedClientResponse>
{
    private readonly IUnitOfWork _unitOfWork;

    public SearchClientsQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<PagedClientResponse> Handle(SearchClientsQuery request, CancellationToken cancellationToken)
    {
        var skip = (request.Page - 1) * request.PageSize;
        var clients = await _unitOfWork.Clients.SearchAsync(request.CompanyId, request.SearchTerm, skip, request.PageSize);
        var totalCount = await _unitOfWork.Clients.CountSearchAsync(request.CompanyId, request.SearchTerm);

        var items = clients.Select(c => new ClientResponse(
            c.Id, c.CompanyId, c.Nome,
            c.Email?.Value, c.Telefone, c.Empresa,
            c.Status,
            c.Tags.Select(t => t.Name).ToList(),
            c.Notes.Select(n => new ClientNoteResponse(n.Id, n.Content, n.AuthorId, n.CreatedAt)).ToList(),
            c.CreatedAt, c.UpdatedAt)).ToList();

        return new PagedClientResponse(items, totalCount, request.Page, request.PageSize);
    }
}
