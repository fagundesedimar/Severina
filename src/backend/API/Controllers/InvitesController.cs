using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Severina.Application.Commands.Users;
using Severina.Application.DTOs;
using Severina.Application.Interfaces;
using Severina.Domain.Enums;

namespace Severina.API.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
[Authorize(Policy = "AdminOnly")]
[EnableRateLimiting("invite")]
public class InvitesController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IInviteCacheService _inviteCacheService;
    private readonly ILogger<InvitesController> _logger;

    public InvitesController(
        IMediator mediator,
        IInviteCacheService inviteCacheService,
        ILogger<InvitesController> logger)
    {
        _mediator = mediator;
        _inviteCacheService = inviteCacheService;
        _logger = logger;
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] InviteUserRequest request)
    {
        try
        {
            var companyId = GetCompanyId();
            var appUrl = HttpContext.RequestServices.GetRequiredService<IConfiguration>()["AppUrl"] ?? "http://localhost:3000";
            var command = new InviteUserCommand(companyId, request.Email, request.Papel, appUrl);
            var result = await _mediator.Send(command);
            return Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
    }

    [HttpGet]
    public async Task<IActionResult> ListPending()
    {
        var companyId = GetCompanyId();
        var invites = await _inviteCacheService.GetPendingInvitesAsync(companyId);
        return Ok(invites);
    }

    [HttpDelete("{code}")]
    public async Task<IActionResult> Revoke(string code)
    {
        await _inviteCacheService.DeleteInviteAsync(code);
        return Ok(new { message = "Convite revogado com sucesso" });
    }

    private Guid GetCompanyId()
    {
        var claim = User.FindFirst("company_id")?.Value;
        return Guid.TryParse(claim, out var companyId) ? companyId : Guid.Empty;
    }
}

[ApiController]
[Route("api/v1/[controller]")]
[AllowAnonymous]
public class InviteAcceptController : ControllerBase
{
    private readonly IMediator _mediator;

    public InviteAcceptController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("{code}")]
    public async Task<ActionResult<UserResponse>> Accept(string code, [FromBody] AcceptInviteRequest request)
    {
        try
        {
            var command = new AcceptInviteCommand(code, request.Nome, request.Senha);
            var result = await _mediator.Send(command);
            return Ok(result);
        }
        catch (KeyNotFoundException)
        {
            return NotFound(new { message = "Convite não encontrado" });
        }
        catch (InvalidOperationException ex) when (ex.Message.Contains("expirado"))
        {
            return StatusCode(410, new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("{code}/validate")]
    public async Task<IActionResult> Validate(string code)
    {
        var inviteCacheService = HttpContext.RequestServices.GetRequiredService<IInviteCacheService>();
        var invite = await inviteCacheService.GetInviteAsync(code);

        if (invite == null)
            return NotFound(new { message = "Convite não encontrado ou expirado" });

        return Ok(new { email = invite.Email, expiresAt = invite.ExpiresAt });
    }
}

public record AcceptInviteRequest(string Nome, string Senha);
