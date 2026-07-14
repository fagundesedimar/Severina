using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Severina.Infrastructure.Data;

namespace Severina.API.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
public class UsersController : ControllerBase
{
    private readonly SeverinaDbContext _context;
    private readonly ILogger<UsersController> _logger;

    public UsersController(SeverinaDbContext context, ILogger<UsersController> logger)
    {
        _context = context;
        _logger = logger;
    }

    [HttpPut("preferences")]
    public async Task<IActionResult> UpdatePreferences([FromBody] UpdatePreferencesRequest request)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!Guid.TryParse(userIdClaim, out var userId))
            return Unauthorized();

        var preference = await _context.UserPreferences
            .FirstOrDefaultAsync(p => p.UserId == userId && p.Key == request.Key);

        if (preference == null)
        {
            preference = new Severina.Domain.Entities.UserPreference(userId, request.Key, request.Value);
            _context.UserPreferences.Add(preference);
        }
        else
        {
            preference.UpdateValue(request.Value);
        }

        await _context.SaveChangesAsync();
        return Ok(new { key = preference.Key, value = preference.Value });
    }

    [HttpGet("preferences/{key}")]
    public async Task<IActionResult> GetPreference(string key)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!Guid.TryParse(userIdClaim, out var userId))
            return Unauthorized();

        var preference = await _context.UserPreferences
            .FirstOrDefaultAsync(p => p.UserId == userId && p.Key == key);

        if (preference == null)
            return NotFound();

        return Ok(new { key = preference.Key, value = preference.Value });
    }
}

public record UpdatePreferencesRequest(string Key, string Value);
