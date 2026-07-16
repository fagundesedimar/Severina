## ADDED Requirements

### Requirement: User can create appointment
The system SHALL allow authenticated users to create new appointments.

#### Scenario: Successful creation
- **WHEN** user sends POST /api/v1/appointments with valid data (titulo, data_hora_inicio, data_hora_fim, cliente_id, tipo)
- **THEN** system returns 201 Created with appointment data including id and status "scheduled"

#### Scenario: Appointment in the past
- **WHEN** user sends POST /api/v1/appointments with data_hora_inicio in the past
- **THEN** system returns 400 Bad Request with error "Não é possível agendar no passado"

#### Scenario: End time before start time
- **WHEN** user sends POST /api/v1/appointments with data_hora_fim before data_hora_inicio
- **THEN** system returns 400 Bad Request with error "Horário de fim deve ser após horário de início"

#### Scenario: Time slot conflict
- **WHEN** user sends POST /api/v1/appointments with time that overlaps existing appointment
- **THEN** system returns 409 Conflict with error "Horário conflita com appointment existente"

#### Scenario: Missing required fields
- **WHEN** user sends POST /api/v1/appointments without titulo or data_hora_inicio
- **THEN** system returns 400 Bad Request with validation errors

### Requirement: User can list appointments
The system SHALL return paginated list of appointments for the user's company.

#### Scenario: Successful list
- **WHEN** user sends GET /api/v1/appointments with optional date range
- **THEN** system returns 200 OK with paginated list sorted by data_hora_inicio

#### Scenario: Filter by date range
- **WHEN** user sends GET /api/v1/appointments?from=2026-07-01&to=2026-07-31
- **THEN** system returns only appointments within specified range

#### Scenario: Filter by client
- **WHEN** user sends GET /api/v1/appointments?clientId={id}
- **THEN** system returns only appointments for specified client

#### Scenario: Filter by status
- **WHEN** user sends GET /api/v1/appointments?status=scheduled
- **THEN** system returns only appointments with specified status

### Requirement: User can view appointment details
The system SHALL return full appointment data including client info.

#### Scenario: Successful view
- **WHEN** user sends GET /api/v1/appointments/{id} where id belongs to user's company
- **THEN** system returns 200 OK with full appointment data including client name

#### Scenario: Appointment not found
- **WHEN** user sends GET /api/v1/appointments/{id} with invalid id
- **THEN** system returns 404 Not Found

#### Scenario: Cross-tenant access
- **WHEN** user sends GET /api/v1/appointments/{id} where id belongs to another company
- **THEN** system returns 404 Not Found

### Requirement: User can update appointment
The system SHALL allow users to update appointment data.

#### Scenario: Successful update
- **WHEN** user sends PUT /api/v1/appointments/{id} with valid data
- **THEN** system returns 200 OK with updated appointment data

#### Scenario: Update past appointment
- **WHEN** user tries to update appointment that already started
- **THEN** system returns 409 Conflict with error "Não é possível alterar appointment passado"

### Requirement: User can cancel appointment
The system SHALL allow users to cancel scheduled appointments.

#### Scenario: Successful cancellation
- **WHEN** user sends POST /api/v1/appointments/{id}/cancel
- **THEN** system sets status to "cancelled" and returns 200 OK

#### Scenario: Cancel completed appointment
- **WHEN** user tries to cancel appointment with status "completed"
- **THEN** system returns 409 Conflict with error "Appointment já foi concluído"

### Requirement: User can complete appointment
The system SHALL allow users to mark appointments as completed.

#### Scenario: Successful completion
- **WHEN** user sends POST /api/v1/appointments/{id}/complete
- **THEN** system sets status to "completed" and returns 200 OK

#### Scenario: Complete future appointment
- **WHEN** user tries to complete appointment that hasn't started yet
- **THEN** system returns 409 Conflict with error "Appointment ainda não começou"

### Requirement: User can delete appointment
The system SHALL allow users to soft-delete appointments.

#### Scenario: Successful deletion
- **WHEN** user sends DELETE /api/v1/appointments/{id}
- **THEN** system sets deletedAt timestamp and returns 204 No Content
