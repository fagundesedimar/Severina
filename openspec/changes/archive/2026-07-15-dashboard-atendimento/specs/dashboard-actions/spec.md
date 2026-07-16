## ADDED Requirements

### Requirement: Dashboard provides quick access actions
The system SHALL display action buttons for common tasks.

#### Scenario: Actions displayed
- **WHEN** dashboard loads
- **THEN** system shows action buttons: Novo Atendimento, Novo Cliente, Nova Cobrança

#### Scenario: Novo Atendimento action
- **WHEN** user clicks "Novo Atendimento"
- **THEN** system navigates to /atendimento/novo or opens new conversation modal

#### Scenario: Novo Cliente action
- **WHEN** user clicks "Novo Cliente"
- **THEN** system navigates to /clientes/novo or opens client form modal

#### Scenario: Nova Cobrança action
- **WHEN** user clicks "Nova Cobrança"
- **THEN** system navigates to /financeiro/cobrancas/nova or opens invoice form modal

### Requirement: Dashboard shows recent activity
The system SHALL display list of recent activities across modules.

#### Scenario: Activity feed
- **WHEN** dashboard loads
- **THEN** system shows last 10 activities (messages, clients created, payments) sorted by date

#### Scenario: Activity types
- **WHEN** activity is displayed
- **THEN** system shows icon, description, timestamp, and link to source

#### Scenario: Activity click
- **WHEN** user clicks on activity
- **THEN** system navigates to source module (client detail, conversation, transaction)

#### Scenario: Empty activity
- **WHEN** no activities exist
- **THEN** system shows "Nenhuma atividade recente" message

### Requirement: Dashboard shows pending tasks
The system SHALL display list of pending tasks requiring attention.

#### Scenario: Tasks displayed
- **WHEN** dashboard loads
- **THEN** system shows pending tasks: overdue invoices, unresponded messages, upcoming appointments

#### Scenario: Task priority
- **WHEN** tasks are displayed
- **THEN** system sorts by priority (overdue > pending > upcoming)

#### Scenario: Task click
- **WHEN** user clicks on task
- **THEN** system navigates to relevant module

#### Scenario: Task count badge
- **WHEN** dashboard loads
- **THEN** system shows badge with total pending tasks count

### Requirement: Dashboard actions follow Stitch prototype
The actions UI SHALL follow the design in `stitch-prototypes/dashboard.html`.

#### Scenario: Action buttons layout
- **WHEN** dashboard renders
- **THEN** system shows action buttons in horizontal row with icons

#### Scenario: Action button style
- **WHEN** action button renders
- **THEN** system shows primary blue background with white text

#### Scenario: Action button hover
- **WHEN** user hovers over action button
- **THEN** system shows darker blue background
