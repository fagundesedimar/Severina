# Multi-Tenant

## Purpose

Enforce data isolation between companies using company_id in JWT, EF Core Global Query Filters, and anti-IDOR protections.

## Requirements

### Requirement: Data isolation by company_id
The system SHALL isolate data by company_id in all database queries.

#### Scenario: Query filters by company_id
- **WHEN** authenticated user queries any entity
- **THEN** results are filtered by user's company_id automatically

#### Scenario: Cross-tenant access blocked
- **WHEN** user tries to access data from another company
- **THEN** system returns 404 Not Found or empty results

### Requirement: company_id in JWT claims
The system SHALL include company_id in JWT tokens.

#### Scenario: Login includes company_id
- **WHEN** user authenticates
- **THEN** JWT payload contains company_id claim

#### Scenario: company_id extracted from JWT
- **WHEN** backend receives authenticated request
- **THEN** company_id is extracted from JWT claims

### Requirement: EF Core Global Query Filter
The system SHALL use EF Core Global Query Filter for automatic tenant filtering.

#### Scenario: Global filter applied
- **WHEN** any entity query is executed
- **THEN** WHERE clause includes company_id filter automatically

#### Scenario: Filter cannot be bypassed
- **WHEN** developer tries to query without filter
- **THEN** Global Query Filter ensures company_id is always included

### Requirement: Multi-tenant entities have company_id
The system SHALL require company_id on all domain entities.

#### Scenario: Entity creation includes company_id
- **WHEN** new entity is created
- **THEN** company_id is set from JWT claims

#### Scenario: Entity update validates company_id
- **WHEN** entity is updated
- **THEN** system verifies entity belongs to user's company

### Requirement: Anti-IDOR protection
The system SHALL prevent Insecure Direct Object References across tenants.

#### Scenario: Direct ID access blocked
- **WHEN** user requests /api/v1/clients/{id} where id belongs to another company
- **THEN** system returns 404 Not Found

#### Scenario: IDOR test passes
- **WHEN** security test attempts cross-tenant access
- **THEN** all attempts are blocked
