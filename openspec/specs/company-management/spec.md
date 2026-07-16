# Company Management

## Purpose

CRUD de empresas com suporte a PF (Pessoa Física) e PJ (Pessoa Jurídica), validação de CPF/CNPJ, e ciclo de vida de status.

## Requirements

### Requirement: User can register company (PF or PJ)
The system SHALL allow users to register a new company with PF (Pessoa Física) or PJ (Pessoa Jurídica) data.

#### Scenario: Successful PF registration
- **WHEN** user sends POST /api/v1/companies with valid PF data (nome, cpf, email, telefone)
- **THEN** system returns 201 Created with company data and sets user as Administrador

#### Scenario: Successful PJ registration
- **WHEN** user sends POST /api/v1/companies with valid PJ data (razao_social, cnpj, email, telefone)
- **THEN** system returns 201 Created with company data and sets user as Administrador

#### Scenario: Invalid CPF
- **WHEN** user sends POST /api/v1/companies with invalid CPF (wrong check digits)
- **THEN** system returns 400 Bad Request with error "CPF inválido"

#### Scenario: Invalid CNPJ
- **WHEN** user sends POST /api/v1/companies with invalid CNPJ (wrong check digits)
- **THEN** system returns 400 Bad Request with error "CNPJ inválido"

#### Scenario: Duplicate CNPJ/CPF
- **WHEN** user sends POST /api/v1/companies with CNPJ/CPF that already exists
- **THEN** system returns 409 Conflict with error "Empresa já cadastrada"

### Requirement: User can view company details
The system SHALL allow authenticated users to view their company details.

#### Scenario: Successful view
- **WHEN** user sends GET /api/v1/companies/{id} where id matches user's company_id
- **THEN** system returns 200 OK with company data

#### Scenario: Cross-tenant access
- **WHEN** user sends GET /api/v1/companies/{id} where id doesn't match user's company_id
- **THEN** system returns 404 Not Found

### Requirement: User can update company data
The system SHALL allow Administrador to update company data.

#### Scenario: Admin updates company
- **WHEN** Administrador sends PUT /api/v1/companies/{id} with valid data
- **THEN** system returns 200 OK with updated company data

#### Scenario: Operacional tries to update
- **WHEN** Usuário Operacional sends PUT /api/v1/companies/{id}
- **THEN** system returns 403 Forbidden

### Requirement: Company has status lifecycle
The system SHALL manage company status (active, inactive, suspended).

#### Scenario: New company is active
- **WHEN** company is created
- **THEN** status is set to "active"

#### Scenario: Admin deactivates company
- **WHEN** Administrador sends DELETE /api/v1/companies/{id}
- **THEN** company status changes to "inactive" and all users are logged out

#### Scenario: Suspended company cannot access
- **WHEN** user from suspended company tries to access any endpoint
- **THEN** system returns 403 Forbidden with error "Empresa suspensa"
