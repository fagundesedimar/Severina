## ADDED Requirements

### Requirement: Role-based access control
The system SHALL enforce RBAC with two roles: Administrador and Operacional.

#### Scenario: Administrador has full access
- **WHEN** user has role "administrador"
- **THEN** user can access all endpoints including user management and company settings

#### Scenario: Operacional has limited access
- **WHEN** user has role "operacional"
- **THEN** user can access only operational endpoints (clients, appointments, messages) but not user management or company settings

### Requirement: Role is included in JWT
The system SHALL include user role in JWT claims.

#### Scenario: JWT contains role
- **WHEN** user authenticates
- **THEN** JWT payload includes "role" claim with value "administrador" or "operacional"

#### Scenario: Role change requires re-login
- **WHEN** user role is changed by Admin
- **THEN** existing JWT remains valid until expiration; new JWT reflects updated role

### Requirement: Admin can change user roles
The system SHALL allow Administrador to change roles of company users.

#### Scenario: Successful role change
- **WHEN** Administrador sends PUT /api/v1/companies/{companyId}/users/{userId}/role with valid role
- **THEN** system updates user role and returns 200 OK

#### Scenario: Cannot change own role
- **WHEN** Administrador tries to change own role
- **THEN** system returns 400 Bad Request with error "Administrador não pode alterar seu próprio papel"

#### Scenario: Operacional tries to change role
- **WHEN** Usuário Operacional tries to change any role
- **THEN** system returns 403 Forbidden

### Requirement: Endpoint authorization by role
The system SHALL restrict endpoint access based on user role.

#### Scenario: Operacional accesses admin endpoint
- **WHEN** Usuário Operacional sends request to admin-only endpoint
- **THEN** system returns 403 Forbidden

#### Scenario: Administrador accesses any endpoint
- **WHEN** Administrador sends request to any endpoint
- **THEN** system allows access (within company scope)

### Requirement: Multi-tenant isolation
The system SHALL ensure all data access is filtered by company_id.

#### Scenario: Query filters by company_id
- **WHEN** any user queries data
- **THEN** results are filtered by user's company_id

#### Scenario: Cross-tenant access blocked
- **WHEN** user tries to access data from another company
- **THEN** system returns 404 Not Found or empty results
