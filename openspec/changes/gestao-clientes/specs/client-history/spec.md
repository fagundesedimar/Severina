## ADDED Requirements

### Requirement: Client has interaction history
The system SHALL maintain a timeline of all interactions with each client.

#### Scenario: View interaction history
- **WHEN** user sends GET /api/v1/clients/{id}/interactions
- **THEN** system returns paginated list of interactions sorted by date descending

#### Scenario: Empty history
- **WHEN** client has no interactions
- **THEN** system returns 200 OK with empty array

#### Scenario: Cross-tenant access
- **WHEN** user sends GET /api/v1/clients/{id}/interactions where id belongs to another company
- **THEN** system returns 404 Not Found

### Requirement: Interaction types are categorized
The system SHALL categorize interactions by type.

#### Scenario: Supported types
- **WHEN** interaction is created
- **THEN** system accepts type: "message", "call", "email", "note", "appointment"

#### Scenario: Invalid type
- **WHEN** user tries to create interaction with invalid type
- **THEN** system returns 400 Bad Request with valid types list

### Requirement: Interactions are linked to conversations
The system SHALL link interactions to WhatsApp conversations when applicable.

#### Scenario: Link to conversation
- **WHEN** interaction type is "message" and conversation_id is provided
- **THEN** system links interaction to conversation

#### Scenario: No conversation link
- **WHEN** interaction type is "note" or "call"
- **THEN** system creates interaction without conversation link

### Requirement: Interaction has metadata
The system SHALL store additional metadata for each interaction.

#### Scenario: Message metadata
- **WHEN** interaction type is "message"
- **THEN** system stores: direction (in/out), content_preview, read_status

#### Scenario: Call metadata
- **WHEN** interaction type is "call"
- **THEN** system stores: duration_seconds, call_status (answered/missed/busy)

#### Scenario: Appointment metadata
- **WHEN** interaction type is "appointment"
- **THEN** system stores: date, time, status (scheduled/completed/cancelled)

### Requirement: Interactions are immutable
The system SHALL not allow editing or deleting interactions.

#### Scenario: Edit attempt
- **WHEN** user tries to update interaction via PUT /api/v1/clients/{id}/interactions/{interactionId}
- **THEN** system returns 405 Method Not Allowed

#### Scenario: Delete attempt
- **WHEN** user tries to delete interaction via DELETE /api/v1/clients/{id}/interactions/{interactionId}
- **THEN** system returns 405 Method Not Allowed

### Requirement: Interaction history follows Stitch prototype
The history display SHALL follow the design in `stitch-prototypes/clientes.html`.

#### Scenario: Timeline view
- **WHEN** user views client detail
- **THEN** system shows interaction timeline with icons for each type (message, call, etc.)

#### Scenario: Interaction details
- **WHEN** user clicks on interaction in timeline
- **THEN** system expands to show full content and metadata

#### Scenario: Filter by type
- **WHEN** user applies type filter
- **THEN** system shows only interactions of selected type
