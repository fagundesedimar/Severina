# Client CRUD

## Purpose

Create, read, update, delete operations for client management.

## Requirements

### Requirement: User can create client
The system SHALL allow authenticated users to create new clients.

#### Scenario: Successful creation
- **WHEN** user sends POST /api/v1/clients with valid data (nome, email, telefone, empresa)
- **THEN** system returns 201 Created with client data including id and createdAt

#### Scenario: Duplicate email within company
- **WHEN** user sends POST /api/v1/clients with email that already exists in same company
- **THEN** system returns 409 Conflict with error "Cliente com este email já existe"

#### Scenario: Invalid email format
- **WHEN** user sends POST /api/v1/clients with invalid email format
- **THEN** system returns 400 Bad Request with validation error

#### Scenario: Missing required fields
- **WHEN** user sends POST /api/v1/clients without nome or telefone
- **THEN** system returns 400 Bad Request with validation errors for each missing field

### Requirement: User can list clients
The system SHALL return paginated list of clients for the user's company.

#### Scenario: Successful list
- **WHEN** user sends GET /api/v1/clients with optional page and pageSize
- **THEN** system returns 200 OK with paginated list (default: page 1, pageSize 20)

#### Scenario: Empty list
- **WHEN** user sends GET /api/v1/clients with no clients in company
- **THEN** system returns 200 OK with empty array and total 0

#### Scenario: Cross-tenant isolation
- **WHEN** user sends GET /api/v1/clients
- **THEN** system returns only clients belonging to user's company_id

### Requirement: User can view client details
The system SHALL return full client data including tags and notes.

#### Scenario: Successful view
- **WHEN** user sends GET /api/v1/clients/{id} where id belongs to user's company
- **THEN** system returns 200 OK with full client data

#### Scenario: Client not found
- **WHEN** user sends GET /api/v1/clients/{id} with invalid id
- **THEN** system returns 404 Not Found

#### Scenario: Cross-tenant access
- **WHEN** user sends GET /api/v1/clients/{id} where id belongs to another company
- **THEN** system returns 404 Not Found

### Requirement: User can update client
The system SHALL allow users to update client data.

#### Scenario: Successful update
- **WHEN** user sends PUT /api/v1/clients/{id} with valid data
- **THEN** system returns 200 OK with updated client data

#### Scenario: Email conflict on update
- **WHEN** user sends PUT /api/v1/clients/{id} with email that belongs to another client in same company
- **THEN** system returns 409 Conflict

### Requirement: User can delete client
The system SHALL allow users to soft-delete clients.

#### Scenario: Successful deletion
- **WHEN** user sends DELETE /api/v1/clients/{id}
- **THEN** system sets deletedAt timestamp and returns 204 No Content

#### Scenario: Client with active conversations
- **WHEN** user tries to delete client with active conversations
- **THEN** system returns 409 Conflict with error "Cliente possui conversas ativas"

### Requirement: User can manage client tags
The system SHALL allow users to add and remove tags from clients.

#### Scenario: Add tag
- **WHEN** user sends POST /api/v1/clients/{id}/tags with tag name
- **THEN** system adds tag to client and returns 200 OK

#### Scenario: Remove tag
- **WHEN** user sends DELETE /api/v1/clients/{id}/tags/{tagName}
- **THEN** system removes tag from client and returns 200 OK

#### Scenario: Duplicate tag
- **WHEN** user tries to add tag that already exists on client
- **THEN** system returns 200 OK (idempotent operation)

### Requirement: User can add client notes
The system SHALL allow users to add notes to client profile.

#### Scenario: Add note
- **WHEN** user sends POST /api/v1/clients/{id}/notes with content
- **THEN** system creates note with timestamp and author and returns 201 Created

#### Scenario: Empty note content
- **WHEN** user sends POST /api/v1/clients/{id}/notes with empty content
- **THEN** system returns 400 Bad Request
