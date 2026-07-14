## ADDED Requirements

### Requirement: Full-text search by name, email, company
The system SHALL support full-text search across client name, email, and company fields.

#### Scenario: Search by name
- **WHEN** user sends GET /api/v1/clients?search=João
- **THEN** system returns clients where name matches "João" with relevance ranking

#### Scenario: Search by email
- **WHEN** user sends GET /api/v1/clients?search=joao@email.com
- **THEN** system returns clients where email matches

#### Scenario: Search by company
- **WHEN** user sends GET /api/v1/clients?search=Empresa XYZ
- **THEN** system returns clients where empresa field matches

#### Scenario: No results
- **WHEN** user sends GET /api/v1/clients?search=xyznonexistent
- **THEN** system returns 200 OK with empty array

### Requirement: Search is scoped to company
The system SHALL only return search results from user's company.

#### Scenario: Company isolation
- **WHEN** user sends GET /api/v1/clients?search=teste
- **THEN** system returns only clients where company_id matches user's company

### Requirement: Search supports pagination
The system SHALL paginate search results.

#### Scenario: Default pagination
- **WHEN** user sends GET /api/v1/clients?search=teste
- **THEN** system returns first 20 results with page=1, pageSize=20

#### Scenario: Custom pagination
- **WHEN** user sends GET /api/v1/clients?search=teste&page=2&pageSize=10
- **THEN** system returns results 11-20

#### Scenario: Large result set
- **WHEN** search returns more than 1000 results
- **THEN** system caps at 1000 results and includes warning in response

### Requirement: Search performs under 200ms
The system SHALL return search results in under 200ms for 95% of requests.

#### Scenario: Performance requirement
- **WHEN** user sends search request with up to 10,000 clients in company
- **THEN** response is returned within 200ms

### Requirement: Search highlights matching terms
The system SHALL highlight matching terms in search results.

#### Scenario: Highlighted results
- **WHEN** user searches for "João"
- **THEN** matching text in name/email/company is wrapped in `<mark>` tags
