using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Severina.Application.DTOs;
using Severina.Application.Queries.Dashboard;

namespace Severina.API.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
[EnableRateLimiting("dashboard")]
public class DashboardController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<DashboardController> _logger;

    public DashboardController(IMediator mediator, ILogger<DashboardController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    [HttpGet]
    [ResponseCache(Duration = 300)]
    public async Task<ActionResult<DashboardResponse>> Get()
    {
        var companyId = GetCompanyId();
        var result = await _mediator.Send(new GetDashboardQuery(companyId));
        return Ok(result);
    }

    private Guid GetCompanyId()
    {
        var claim = User.FindFirst("company_id")?.Value;
        return Guid.TryParse(claim, out var companyId) ? companyId : Guid.Empty;
    }
}
