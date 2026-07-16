using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Severina.Application.Commands.Companies;
using Severina.Application.DTOs;
using Severina.Application.Queries.Companies;

namespace Severina.API.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
public class CompaniesController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<CompaniesController> _logger;

    public CompaniesController(IMediator mediator, ILogger<CompaniesController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    [HttpPost]
    [AllowAnonymous]
    public async Task<ActionResult<CompanyResponse>> Create([FromBody] CreateCompanyRequest request)
    {
        var command = new CreateCompanyCommand(
            request.Nome,
            request.CnpjCpf,
            request.Email,
            request.TipoPessoa,
            request.Telefone);

        var result = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<CompanyResponse>> GetById(Guid id)
    {
        var result = await _mediator.Send(new GetCompanyQuery(id));

        if (result == null)
            return NotFound();

        var companyId = GetCompanyId();
        if (companyId != id)
            return NotFound();

        return Ok(result);
    }

    [HttpPut("{id:guid}")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<ActionResult<CompanyResponse>> Update(Guid id, [FromBody] UpdateCompanyRequest request)
    {
        try
        {
            var companyId = GetCompanyId();
            if (companyId != id)
                return NotFound();

            var command = new UpdateCompanyCommand(id, request.Nome, request.Email, request.Telefone);
            var result = await _mediator.Send(command);
            return Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> Deactivate(Guid id)
    {
        try
        {
            var companyId = GetCompanyId();
            if (companyId != id)
                return NotFound();

            await _mediator.Send(new DeactivateCompanyCommand(id));
            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    private Guid GetCompanyId()
    {
        var claim = User.FindFirst("company_id")?.Value;
        return Guid.TryParse(claim, out var companyId) ? companyId : Guid.Empty;
    }
}
