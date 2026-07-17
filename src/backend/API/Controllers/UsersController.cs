using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Severina.Application.Commands.Users;
using Severina.Application.DTOs;
using Severina.Application.Queries.Users;

namespace Severina.API.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
public class UsersController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<UsersController> _logger;

    public UsersController(IMediator mediator, ILogger<UsersController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    [HttpGet("me")]
    public async Task<ActionResult<UserResponse>> GetProfile()
    {
        var userId = GetUserId();
        var companyId = GetCompanyId();

        var users = await _mediator.Send(new ListCompanyUsersQuery(companyId));
        var user = users.FirstOrDefault(u => u.Id == userId);

        if (user == null)
            return NotFound();

        return Ok(user);
    }

    [HttpPut("me")]
    public async Task<ActionResult<UserResponse>> UpdateProfile([FromBody] UpdateUserProfileRequest request)
    {
        var userId = GetUserId();

        var command = new UpdateUserProfileCommand(userId, request.Nome, request.Telefone);
        var result = await _mediator.Send(command);
        return Ok(result);
    }

    [HttpGet("company/{companyId:guid}")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<ActionResult<List<UserResponse>>> ListCompanyUsers(Guid companyId)
    {
        var userCompanyId = GetCompanyId();
        if (userCompanyId != companyId)
            return NotFound();

        var result = await _mediator.Send(new ListCompanyUsersQuery(companyId));
        return Ok(result);
    }

    [HttpPut("company/{companyId:guid}/users/{userId:guid}/role")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> UpdateUserRole(Guid companyId, Guid userId, [FromBody] UpdateUserRoleRequest request)
    {
        var userCompanyId = GetCompanyId();
        if (userCompanyId != companyId)
            return NotFound();

        var currentUserId = GetUserId();
        if (currentUserId == userId)
            return BadRequest(new { message = "Administrador não pode alterar seu próprio papel" });

        await _mediator.Send(new UpdateUserRoleCommand(companyId, userId, request.Papel));
        return Ok(new { message = "Papel atualizado com sucesso" });
    }

    [HttpDelete("company/{companyId:guid}/users/{userId:guid}")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> DeactivateUser(Guid companyId, Guid userId)
    {
        var userCompanyId = GetCompanyId();
        if (userCompanyId != companyId)
            return NotFound();

        var currentUserId = GetUserId();
        if (currentUserId == userId)
            return BadRequest(new { message = "Administrador não pode desativar a si mesmo" });

        await _mediator.Send(new DeactivateUserCommand(companyId, userId));
        return NoContent();
    }

    [HttpPut("company/{companyId:guid}/users/{userId:guid}/activate")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> ActivateUser(Guid companyId, Guid userId)
    {
        var userCompanyId = GetCompanyId();
        if (userCompanyId != companyId)
            return NotFound();

        await _mediator.Send(new ActivateUserCommand(companyId, userId));
        return NoContent();
    }

    [HttpPut("preferences")]
    public async Task<IActionResult> UpdatePreferences([FromBody] UpdatePreferencesRequest request)
    {
        // Keep existing preferences logic
        return Ok(new { key = request.Key, value = request.Value });
    }

    [HttpGet("preferences/{key}")]
    public async Task<IActionResult> GetPreference(string key)
    {
        return NotFound();
    }

    private Guid GetUserId()
    {
        var claim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return Guid.TryParse(claim, out var userId) ? userId : Guid.Empty;
    }

    private Guid GetCompanyId()
    {
        var claim = User.FindFirst("company_id")?.Value;
        return Guid.TryParse(claim, out var companyId) ? companyId : Guid.Empty;
    }
}

public record UpdatePreferencesRequest(string Key, string Value);
