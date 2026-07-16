## ADDED Requirements

### Requirement: Invite system sends email with code
The system SHALL generate unique invite codes and send them via email.

#### Scenario: Code generation
- **WHEN** Admin creates invite
- **THEN** system generates 32-char alphanumeric code and stores in Redis with key "invite:{code}" and TTL 7 days

#### Scenario: Email sent
- **WHEN** invite is created
- **THEN** system sends email to invited address with link "https://app.severina.ai/convite/{code}"

#### Scenario: Email service failure
- **WHEN** email service fails to send
- **THEN** invite is still created in Redis but system returns warning "Convite criado, mas falha no envio de email"

### Requirement: Invite has expiration
The system SHALL enforce invite expiration after 7 days.

#### Scenario: Valid invite
- **WHEN** user accesses invite within 7 days
- **THEN** system validates code and allows acceptance

#### Scenario: Expired invite
- **WHEN** user accesses invite after 7 days
- **THEN** system returns 410 Gone and removes code from Redis

### Requirement: Invite is single-use
The system SHALL allow each invite code to be used only once.

#### Scenario: First use succeeds
- **WHEN** user accepts invite for first time
- **THEN** system creates account and removes code from Redis

#### Scenario: Second use fails
- **WHEN** user tries to accept same invite code again
- **THEN** system returns 404 Not Found (code already removed)

### Requirement: Admin can list pending invites
The system SHALL allow Administrador to view pending invites.

#### Scenario: Successful list
- **WHEN** Administrador sends GET /api/v1/invites
- **THEN** system returns 200 OK with list of pending invites (email, role, createdAt, expiresAt)

#### Scenario: No pending invites
- **WHEN** Administrador sends GET /api/v1/invites with no pending invites
- **THEN** system returns 200 OK with empty array

### Requirement: Admin can revoke invite
The system SHALL allow Administrador to revoke pending invites.

#### Scenario: Successful revocation
- **WHEN** Administrador sends DELETE /api/v1/invites/{code}
- **THEN** system removes code from Redis and returns 200 OK

#### Scenario: Already accepted invite
- **WHEN** Administrador tries to revoke already accepted invite
- **THEN** system returns 404 Not Found
