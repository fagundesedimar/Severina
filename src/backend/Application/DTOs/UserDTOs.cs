using Severina.Domain.Enums;

namespace Severina.Application.DTOs;

public record UserResponse(
    Guid Id,
    string Nome,
    string Email,
    PapelUsuario Papel,
    StatusUsuario Status,
    DateTime CreatedAt);

public record InviteUserRequest(
    string Email,
    PapelUsuario Papel);

public record UpdateUserRoleRequest(
    PapelUsuario Papel);

public record UpdateUserProfileRequest(
    string Nome,
    string? Telefone);
