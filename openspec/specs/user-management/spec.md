# User Management

## Purpose

CRUD de usuários dentro de uma empresa: listagem, convites, aceite, desativação, e atualização de perfil.

## Requirements

### Requirement: Admin can list company users
The system SHALL allow Administrador to list all users in their company.

#### Scenario: Successful list
- **WHEN** Administrador sends GET /api/v1/companies/{companyId}/users
- **THEN** system returns 200 OK with list of users (excluding password hash)

#### Scenario: Operacional tries to list
- **WHEN** Usuário Operacional sends GET /api/v1/companies/{companyId}/users
- **THEN** system returns 403 Forbidden

### Requirement: Admin can invite user
The system SHALL allow Administrador to invite new users via email.

#### Scenario: Successful invite
- **WHEN** Administrador sends POST /api/v1/invites with valid email and role
- **THEN** system creates invite code in Redis (TTL 7 days) and sends email with invitation link

#### Scenario: Invite to existing user
- **WHEN** Administrador tries to invite email that already belongs to company
- **THEN** system returns 409 Conflict with error "Usuário já pertence à empresa"

#### Scenario: Operacional tries to invite
- **WHEN** Usuário Operacional sends POST /api/v1/invites
- **THEN** system returns 403 Forbidden

### Requirement: User can accept invite
The system SHALL allow invited users to accept invitation and join company.

#### Scenario: Successful accept
- **WHEN** user sends POST /api/v1/invites/{code}/accept with valid code
- **THEN** system creates user account with assigned role and company_id

#### Scenario: Expired invite
- **WHEN** user sends POST /api/v1/invites/{code}/accept with expired code
- **THEN** system returns 410 Gone with error "Convite expirado"

#### Scenario: Invalid invite
- **WHEN** user sends POST /api/v1/invites/{code}/accept with invalid code
- **THEN** system returns 404 Not Found

### Requirement: Admin can deactivate user
The system SHALL allow Administrador to deactivate user accounts.

#### Scenario: Successful deactivation
- **WHEN** Administrador sends DELETE /api/v1/companies/{companyId}/users/{userId}
- **THEN** user status changes to "inactive" and user is logged out

#### Scenario: Cannot deactivate self
- **WHEN** Administrador tries to deactivate themselves
- **THEN** system returns 400 Bad Request with error "Administrador não pode desativar a si mesmo"

### Requirement: User can update own profile
The system SHALL allow users to update their own profile data.

#### Scenario: Successful update
- **WHEN** user sends PUT /api/v1/users/me with valid data (nome, telefone)
- **THEN** system returns 200 OK with updated user data

#### Scenario: Cannot change own role
- **WHEN** user tries to update own role via PUT /api/v1/users/me
- **THEN** system ignores role field (only Admin can change roles)
