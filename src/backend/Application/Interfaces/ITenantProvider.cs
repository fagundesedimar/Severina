namespace Severina.Application.Interfaces;

public interface ITenantProvider
{
    Guid? CompanyId { get; }
    void SetCompanyId(Guid companyId);
}
