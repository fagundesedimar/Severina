using Severina.Domain.Enums;

namespace Severina.Application.DTOs;

public record CompanyResponse(
    Guid Id,
    string Nome,
    string CnpjCpf,
    string Email,
    string? Telefone,
    TipoPessoa TipoPessoa,
    StatusEmpresa Status,
    string? Plano,
    DateTime CreatedAt);

public record CreateCompanyRequest(
    string Nome,
    string CnpjCpf,
    string Email,
    TipoPessoa TipoPessoa,
    string? Telefone);

public record UpdateCompanyRequest(
    string Nome,
    string Email,
    string? Telefone);
