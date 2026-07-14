using Severina.Application.Interfaces;
using Severina.Infrastructure.Data;

namespace Severina.API.Middlewares;

public class TenantMiddleware
{
    private readonly RequestDelegate _next;

    public TenantMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, ITenantProvider tenantProvider)
    {
        var companyIdClaim = context.User?.FindFirst("company_id")?.Value;

        if (Guid.TryParse(companyIdClaim, out var companyId))
        {
            tenantProvider.SetCompanyId(companyId);
        }

        await _next(context);
    }
}
