using Severina.Domain.Entities;
using Severina.Domain.Enums;

namespace Severina.Application.DTOs;

public record ClientResponse(
    Guid Id,
    Guid CompanyId,
    string Nome,
    string? Email,
    string? Telefone,
    string? Empresa,
    StatusCliente Status,
    IReadOnlyList<string> Tags,
    IReadOnlyList<ClientNoteResponse> Notes,
    DateTime CreatedAt,
    DateTime UpdatedAt);

public record ClientNoteResponse(
    Guid Id,
    string Content,
    Guid AuthorId,
    DateTime CreatedAt);

public record CreateClientRequest(
    string Nome,
    string? Email,
    string? Telefone,
    string? Empresa);

public record UpdateClientRequest(
    string Nome,
    string? Email,
    string? Telefone,
    string? Empresa);

public record AddClientTagRequest(string TagName);

public record AddClientNoteRequest(string Content);

public record PagedClientResponse(
    IReadOnlyList<ClientResponse> Items,
    int TotalCount,
    int Page,
    int PageSize);

public record InteractionResponse(
    Guid Id,
    Guid ClientId,
    InteractionType Type,
    string Content,
    string? MetadataDirection,
    int? MetadataDurationSeconds,
    string? MetadataStatus,
    Guid? ConversationId,
    DateTime CreatedAt);

public record CreateInteractionRequest(
    InteractionType Type,
    string Content,
    Guid? ConversationId);

public record PagedInteractionResponse(
    IReadOnlyList<InteractionResponse> Items,
    int TotalCount,
    int Page,
    int PageSize);

public record ImportJobResponse(
    Guid Id,
    string FileName,
    int TotalRows,
    int ProcessedRows,
    int ImportedRows,
    int SkippedRows,
    int ErrorRows,
    ImportJobStatus Status,
    string? ErrorMessage,
    DateTime CreatedAt);
