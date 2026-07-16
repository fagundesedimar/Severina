using FluentValidation;
using MediatR;
using Severina.Application.DTOs;
using Severina.Application.Interfaces;
using Severina.Domain.Entities;
using Severina.Domain.Enums;
using Severina.Domain.Interfaces;
using Severina.Domain.ValueObjects;

namespace Severina.Application.Commands.Companies;

public record CreateCompanyCommand(
    string Nome,
    string CnpjCpf,
    string Email,
    TipoPessoa TipoPessoa,
    string? Telefone) : IRequest<CompanyResponse>;

public class CreateCompanyCommandValidator : AbstractValidator<CreateCompanyCommand>
{
    public CreateCompanyCommandValidator()
    {
        RuleFor(x => x.Nome)
            .NotEmpty().WithMessage("Nome é obrigatório")
            .MaximumLength(200).WithMessage("Nome deve ter no máximo 200 caracteres");

        RuleFor(x => x.CnpjCpf)
            .NotEmpty().WithMessage("CPF/CNPJ é obrigatório")
            .Must(v => CnpjCpf.TryCreate(v, out _)).WithMessage("CPF/CNPJ inválido");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email é obrigatório")
            .EmailAddress().WithMessage("Formato de email inválido");

        RuleFor(x => x.Telefone)
            .MaximumLength(20).WithMessage("Telefone deve ter no máximo 20 caracteres");
    }
}

public class CreateCompanyCommandHandler : IRequestHandler<CreateCompanyCommand, CompanyResponse>
{
    private readonly ICompanyRepository _companyRepository;
    private readonly IUserRepository _userRepository;
    private readonly IPasswordService _passwordService;
    private readonly IUnitOfWork _unitOfWork;

    public CreateCompanyCommandHandler(
        ICompanyRepository companyRepository,
        IUserRepository userRepository,
        IPasswordService passwordService,
        IUnitOfWork unitOfWork)
    {
        _companyRepository = companyRepository;
        _userRepository = userRepository;
        _passwordService = passwordService;
        _unitOfWork = unitOfWork;
    }

    public async Task<CompanyResponse> Handle(CreateCompanyCommand request, CancellationToken cancellationToken)
    {
        var cnpjCpf = CnpjCpf.Create(request.CnpjCpf);
        var email = Email.Create(request.Email);

        var existingCompany = await _companyRepository.GetByCnpjCpfAsync(cnpjCpf.Value);
        if (existingCompany != null)
            throw new InvalidOperationException("Empresa já cadastrada");

        var existingEmail = await _companyRepository.GetByEmailAsync(email.Value);
        if (existingEmail != null)
            throw new InvalidOperationException("Empresa já cadastrada");

        var telefone = !string.IsNullOrEmpty(request.Telefone)
            ? Telefone.Create(request.Telefone)
            : null;

        var company = new Company(request.Nome, cnpjCpf, email, request.TipoPessoa, telefone);

        await _companyRepository.AddAsync(company);

        var adminPassword = _passwordService.HashPassword("Temp@123");
        var adminUser = new User(
            company.Id,
            request.Nome,
            email,
            adminPassword,
            PapelUsuario.Administrador);

        await _userRepository.AddAsync(adminUser);

        company.AddUser(adminUser);

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
