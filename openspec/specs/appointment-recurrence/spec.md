# Appointment Recurrence

## Purpose

Suporte a compromissos recorrentes com regras configuráveis (diário, semanal, mensal, custom), geração lazy, e edição de instâncias individuais.

## Requirements

### Requirement: Appointment supports recurrence rules
The system SHALL support recurring appointments with configurable patterns.

#### Scenario: Daily recurrence
- **WHEN** user creates appointment with recurrence "daily"
- **THEN** system generates appointments for each day within recurrence range

#### Scenario: Weekly recurrence
- **WHEN** user creates appointment with recurrence "weekly" and daysOfWeek=[1,3,5]
- **THEN** system generates appointments for Monday, Wednesday, Friday within range

#### Scenario: Monthly recurrence
- **WHEN** user creates appointment with recurrence "monthly" on day 15
- **THEN** system generates appointments on 15th of each month within range

#### Scenario: Custom recurrence
- **WHEN** user creates appointment with recurrence "custom" and interval=2, unit="weeks"
- **THEN** system generates appointments every 2 weeks within range

### Requirement: Recurrence has end condition
The system SHALL enforce end condition for recurring appointments.

#### Scenario: End by date
- **WHEN** user sets recurrenceEndType="date" and recurrenceEndDate="2026-12-31"
- **THEN** system generates appointments until specified date

#### Scenario: End after count
- **WHEN** user sets recurrenceEndType="count" and recurrenceCount=10
- **THEN** system generates exactly 10 occurrences

#### Scenario: No end condition
- **WHEN** user tries to create recurrence without end condition
- **THEN** system returns 400 Bad Request with error "Recorrência deve ter condição de término"

### Requirement: Recurring instances are lazy generated
The system SHALL generate recurring instances on-demand, not upfront.

#### Scenario: Query triggers generation
- **WHEN** user requests appointments for week containing recurrence
- **THEN** system generates instances for that week if not cached

#### Scenario: Cached instances
- **WHEN** user requests same week again
- **THEN** system returns cached instances from Redis (TTL 5min)

### Requirement: User can edit single instance
The system SHALL allow editing individual instances of recurring appointments.

#### Scenario: Edit single instance
- **WHEN** user sends PUT /api/v1/appointments/{instanceId} with exception flag
- **THEN** system updates only that instance, preserving recurrence rule

#### Scenario: Edit all future instances
- **WHEN** user sends PUT /api/v1/appointments/{instanceId}/series with data
- **THEN** system updates recurrence rule and regenerates future instances

### Requirement: User can cancel single instance
The system SHALL allow canceling individual instances without affecting the series.

#### Scenario: Cancel single instance
- **WHEN** user sends POST /api/v1/appointments/{instanceId}/cancel
- **THEN** system cancels only that instance, series continues

#### Scenario: Cancel all future
- **WHEN** user sends POST /api/v1/appointments/{instanceId}/cancel-series
- **THEN** system cancels current and all future instances

### Requirement: Recurrence follows Stitch prototype
The recurrence UI SHALL follow the design in `stitch-prototypes/agenda.html`.

#### Scenario: Recurrence selector
- **WHEN** user creates appointment
- **THEN** system shows recurrence options: Nenhum, Diário, Semanal, Mensal, Custom

#### Scenario: Weekly day picker
- **WHEN** user selects "Semanal"
- **THEN** system shows checkboxes for each day of week

#### Scenario: Monthly day picker
- **WHEN** user selects "Mensal"
- **THEN** system shows day-of-month selector (1-31)
