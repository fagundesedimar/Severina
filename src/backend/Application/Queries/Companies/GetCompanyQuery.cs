using MediatR;
using Severina.Application.DTOs;
using Severina.Domain.Interfaces;

namespace Severina.Application.Queries.Companies;

public record GetCompanyQuery(Guid Id) : IRequest<CompanyResponse?>;

public class GetCompanyQueryHandler : IRequestHandler<GetCompanyQuery, CompanyResponse?>
{
    private readonly ICompanyRepository _companyRepository;

    public GetCompanyQueryHandler(ICompanyRepository companyRepository)
    {
        _companyRepository = companyRepository;
    }

    public async Task<CompanyResponse?> Handle(GetCompanyQuery request, CancellationToken cancellationToken)
    {
        var company = await _companyRepository.GetByIdAsync(request.Id);

        if (company == null)
            return null;

        return new CompanyResponse(
            company.Id,
            company.Nome,
            company.CnpjCpf.Formatted,
            company.Email.Value,
            company.Telefone?.Formatted,
            company.TipoPessoa,
            company.Status,
            company.Plano,
            company.CreatedAt);
    }
}
