# Dashboard Cache

## Purpose

Redis caching layer, invalidation strategies, and performance requirements for dashboard data.

## Requirements

### Requirement: Dashboard data is cached in Redis
The system SHALL cache dashboard data for performance.

#### Scenario: Cache hit
- **WHEN** user requests dashboard within 5 minutes of last request
- **THEN** system returns cached data without querying database

#### Scenario: Cache miss
- **WHEN** user requests dashboard after 5 minutes
- **THEN** system queries all modules, aggregates data, and caches result

#### Scenario: Cache TTL
- **WHEN** cache is set
- **THEN** system sets TTL of 5 minutes (300 seconds)

### Requirement: Dashboard cache is invalidated on mutations
The system SHALL invalidate cache when data changes.

#### Scenario: Transaction created
- **WHEN** new transaction is created
- **THEN** system invalidates dashboard cache for that company

#### Scenario: Client created
- **WHEN** new client is created
- **THEN** system invalidates dashboard cache for that company

#### Scenario: Appointment created
- **WHEN** new appointment is created
- **THEN** system invalidates dashboard cache for that company

#### Scenario: Conversation status changed
- **WHEN** conversation status changes
- **THEN** system invalidates dashboard cache for that company

### Requirement: Dashboard loads in under 200ms
The system SHALL meet performance requirements.

#### Scenario: Cold load
- **WHEN** user requests dashboard with no cache
- **THEN** system responds within 500ms (first load)

#### Scenario: Warm load
- **WHEN** user requests dashboard with cache hit
- **THEN** system responds within 200ms (95th percentile)

#### Scenario: Cache warming
- **WHEN** system starts
- **THEN** background job warms cache for all active companies

### Requirement: Dashboard handles partial failures
The system SHALL gracefully handle failures in individual modules.

#### Scenario: Module unavailable
- **WHEN** one module (e.g., Billing) is unavailable
- **THEN** system returns available KPIs and shows "Indisponível" for failed module

#### Scenario: Timeout
- **WHEN** module query takes > 2 seconds
- **THEN** system times out and returns partial data

#### Scenario: Error logging
- **WHEN** module fails
- **THEN** system logs error with module name and failure reason

### Requirement: Dashboard is multi-tenant isolated
The system SHALL ensure all dashboard data is scoped to user's company.

#### Scenario: Company isolation
- **WHEN** user requests dashboard
- **THEN** system returns only data belonging to user's company_id

#### Scenario: Cache key includes company
- **WHEN** cache is set
- **THEN** system uses key "dashboard:{companyId}" to isolate data

#### Scenario: Cross-tenant access
- **WHEN** user tries to access dashboard with invalid company
- **THEN** system returns empty KPIs with zero values
