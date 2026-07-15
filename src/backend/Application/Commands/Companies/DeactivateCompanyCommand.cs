using MediatR;
using Severina.Domain.Interfaces;

namespace Severina.Application.Commands.Companies;

public record DeactivateCompanyCommand(Guid Id) : IRequest<Unit>;

public class DeactivateCompanyCommandHandler : IRequestHandler<DeactivateCompanyCommand, Unit>
{
    private readonly ICompanyRepository _companyRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DeactivateCompanyCommandHandler(
        ICompanyRepository companyRepository,
        IUnitOfWork unitOfWork)
    {
        _companyRepository = companyRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Unit> Handle(DeactivateCompanyCommand request, CancellationToken cancellationToken)
    {
        var company = await _companyRepository.GetByIdAsync(request.Id)
            ?? throw new InvalidOperationException("Empresa não encontrada");

        company.Deactivate();

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
