## ADDED Requirements

### Requirement: System sends push notifications via WebSocket
The system SHALL send real-time notifications to connected clients.

#### Scenario: Appointment reminder
- **WHEN** appointment is 15 minutes away
- **THEN** system sends WebSocket notification with appointment details

#### Scenario: Appointment status change
- **WHEN** appointment status changes (confirmed, cancelled, completed)
- **THEN** system sends WebSocket notification to relevant users

#### Scenario: New appointment created
- **WHEN** new appointment is created
- **THEN** system sends WebSocket notification to company admins

### Requirement: Notifications are scoped to company
The system SHALL only send notifications to users within the same company.

#### Scenario: Company isolation
- **WHEN** notification is triggered
- **THEN** system sends only to WebSocket connections belonging to same company_id

#### Scenario: User preference
- **WHEN** user has disabled notifications
- **THEN** system does not send notifications to that user

### Requirement: Notification has structured data
The system SHALL include relevant data in notifications.

#### Scenario: Appointment reminder data
- **WHEN** sending appointment reminder
- **THEN** notification includes: appointmentId, titulo, data_hora_inicio, clienteNome

#### Scenario: Status change data
- **WHEN** sending status change notification
- **THEN** notification includes: appointmentId, oldStatus, newStatus, changedBy

### Requirement: WebSocket connection management
The system SHALL manage WebSocket connections lifecycle.

#### Scenario: Connection established
- **WHEN** client connects to WebSocket
- **THEN** system authenticates via JWT and registers connection

#### Scenario: Connection lost
- **WHEN** WebSocket connection is lost
- **THEN** system attempts reconnection with exponential backoff

#### Scenario: Invalid token
- **WHEN** client connects with invalid/expired JWT
- **THEN** system closes connection with reason "unauthorized"

### Requirement: Notifications follow Stitch prototype
The notification UI SHALL follow the design in `stitch-prototypes/agenda.html`.

#### Scenario: Toast notification
- **WHEN** notification is received
- **THEN** system shows toast with appointment title and time

#### Scenario: Notification bell
- **WHEN** user has unread notifications
- **THEN** system shows badge on notification bell icon

#### Scenario: Notification list
- **WHEN** user clicks notification bell
- **THEN** system shows dropdown with recent notifications
