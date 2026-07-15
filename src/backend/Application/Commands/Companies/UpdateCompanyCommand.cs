using FluentValidation;
using MediatR;
using Severina.Application.DTOs;
using Severina.Domain.Interfaces;
using Severina.Domain.ValueObjects;

namespace Severina.Application.Commands.Companies;

public record UpdateCompanyCommand(
    Guid Id,
    string Nome,
    string Email,
    string? Telefone) : IRequest<CompanyResponse>;

public class UpdateCompanyCommandValidator : AbstractValidator<UpdateCompanyCommand>
{
    public UpdateCompanyCommandValidator()
    {
        RuleFor(x => x.Nome)
            .NotEmpty().WithMessage("Nome é obrigatório")
            .MaximumLength(200).WithMessage("Nome deve ter no máximo 200 caracteres");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email é obrigatório")
            .EmailAddress().WithMessage("Formato de email inválido");
    }
}

public class UpdateCompanyCommandHandler : IRequestHandler<UpdateCompanyCommand, CompanyResponse>
{
    private readonly ICompanyRepository _companyRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateCompanyCommandHandler(
        ICompanyRepository companyRepository,
        IUnitOfWork unitOfWork)
    {
        _companyRepository = companyRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<CompanyResponse> Handle(UpdateCompanyCommand request, CancellationToken cancellationToken)
    {
        var company = await _companyRepository.GetByIdAsync(request.Id)
            ?? throw new InvalidOperationException("Empresa não encontrada");

        var email = Email.Create(request.Email);
        var telefone = !string.IsNullOrEmpty(request.Telefone)
            ? Telefone.Create(request.Telefone)
            : null;

        company.Update(request.Nome, email, telefone);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

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
