using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Severina.Application.Commands.Clients;
using Severina.Application.DTOs;
using Severina.Application.Queries.Clients;
using Severina.Domain.Enums;

namespace Severina.API.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
public class ClientsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<ClientsController> _logger;

    public ClientsController(IMediator mediator, ILogger<ClientsController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<PagedClientResponse>> List(
        [FromQuery] string? search,
        [FromQuery] string? status,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        var companyId = GetCompanyId();

        if (!string.IsNullOrWhiteSpace(search))
        {
            var searchResult = await _mediator.Send(new SearchClientsQuery(companyId, search, page, pageSize));
            return Ok(searchResult);
        }

        var result = await _mediator.Send(new ListClientsQuery(companyId, page, pageSize, status));
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ClientResponse>> GetById(Guid id)
    {
        var companyId = GetCompanyId();
        var result = await _mediator.Send(new GetClientByIdQuery(companyId, id));

        if (result is null)
            return NotFound();

        return Ok(result);
    }

    [HttpPost]
    public async Task<ActionResult<ClientResponse>> Create([FromBody] CreateClientRequest request)
    {
        var companyId = GetCompanyId();
        var command = new CreateClientCommand(companyId, request.Nome, request.Email, request.Telefone, request.Empresa);
        var result = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<ClientResponse>> Update(Guid id, [FromBody] UpdateClientRequest request)
    {
        var companyId = GetCompanyId();
        var command = new UpdateClientCommand(companyId, id, request.Nome, request.Email, request.Telefone, request.Empresa);
        var result = await _mediator.Send(command);
        return Ok(result);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var companyId = GetCompanyId();
        await _mediator.Send(new DeleteClientCommand(companyId, id));
        return NoContent();
    }

    [HttpPost("{id:guid}/tags")]
    public async Task<ActionResult<ClientResponse>> AddTag(Guid id, [FromBody] AddClientTagRequest request)
    {
        var companyId = GetCompanyId();
        var result = await _mediator.Send(new AddClientTagCommand(companyId, id, request.TagName));
        return Ok(result);
    }

    [HttpDelete("{id:guid}/tags/{tagName}")]
    public async Task<ActionResult<ClientResponse>> RemoveTag(Guid id, string tagName)
    {
        var companyId = GetCompanyId();
        var result = await _mediator.Send(new RemoveClientTagCommand(companyId, id, tagName));
        return Ok(result);
    }

    [HttpPost("{id:guid}/notes")]
    public async Task<ActionResult<ClientNoteResponse>> AddNote(Guid id, [FromBody] AddClientNoteRequest request)
    {
        var companyId = GetCompanyId();
        var authorId = GetUserId();
        var result = await _mediator.Send(new AddClientNoteCommand(companyId, id, authorId, request.Content));
        return CreatedAtAction(nameof(GetById), new { id }, result);
    }

    [HttpGet("{id:guid}/interactions")]
    public async Task<ActionResult<PagedInteractionResponse>> ListInteractions(
        Guid id,
        [FromQuery] InteractionType? type = null,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        var companyId = GetCompanyId();
        var result = await _mediator.Send(new ListClientInteractionsQuery(companyId, id, type, page, pageSize));
        return Ok(result);
    }

    [HttpPost("{id:guid}/interactions")]
    public async Task<ActionResult<InteractionResponse>> CreateInteraction(Guid id, [FromBody] CreateInteractionRequest request)
    {
        var companyId = GetCompanyId();
        var command = new CreateInteractionCommand(companyId, id, request.Type, request.Content, request.ConversationId);
        var result = await _mediator.Send(command);
        return CreatedAtAction(nameof(ListInteractions), new { id }, result);
    }

    private Guid GetCompanyId()
    {
        var claim = User.FindFirst("company_id")?.Value;
        return Guid.TryParse(claim, out var companyId) ? companyId : Guid.Empty;
    }

    private Guid GetUserId()
    {
        var claim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return Guid.TryParse(claim, out var userId) ? userId : Guid.Empty;
    }
}
