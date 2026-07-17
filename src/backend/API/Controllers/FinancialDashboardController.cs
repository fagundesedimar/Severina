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
public class FinancialDashboardController : ControllerBase
{
    private readonly IMediator _mediator;

    public FinancialDashboardController(IMediator mediator)
    {
        _mediator = mediator;
    }

    private Guid GetCompanyId() => Guid.Parse(User.FindFirstValue("company_id")!);

    [HttpGet]
    [ResponseCache(Duration = 300)]
    public async Task<ActionResult<FinancialDashboardResponse>> GetDashboard()
    {
        var companyId = GetCompanyId();
        var result = await _mediator.Send(new GetFinancialDashboardQuery(companyId));
        return Ok(result);
    }
}
