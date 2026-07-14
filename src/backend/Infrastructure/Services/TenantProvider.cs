using System.Diagnostics.CodeAnalysis;
using Severina.Application.Interfaces;

namespace Severina.Infrastructure.Services;

public class TenantProvider : ITenantProvider
{
    private Guid? _companyId;

    public Guid? CompanyId => _companyId;

    public void SetCompanyId(Guid companyId)
    {
        _companyId = companyId;
    }
}
