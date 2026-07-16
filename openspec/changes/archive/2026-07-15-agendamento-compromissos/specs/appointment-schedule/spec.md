## ADDED Requirements

### Requirement: Weekly schedule view
The system SHALL display appointments in a weekly grid view.

#### Scenario: Default view
- **WHEN** user navigates to /agenda
- **THEN** system shows current week with time slots from 08:00 to 20:00

#### Scenario: Time slots
- **WHEN** week view loads
- **THEN** system shows hourly time slots with appointments positioned by start time

#### Scenario: Appointment cards
- **WHEN** appointment exists in time slot
- **THEN** system shows card with titulo, clienteNome, tipo, and status color

#### Scenario: Current time indicator
- **WHEN** viewing current week
- **THEN** system shows red line at current time position

### Requirement: Monthly schedule view
The system SHALL display appointments in a monthly calendar view.

#### Scenario: Month view
- **WHEN** user switches to month view
- **THEN** system shows calendar grid with day cells containing appointment indicators

#### Scenario: Day cell content
- **WHEN** day has appointments
- **THEN** system shows first 3 appointments with "+N more" indicator

#### Scenario: Day click
- **WHEN** user clicks on day cell
- **THEN** system switches to week view showing that day

### Requirement: Schedule navigation
The system SHALL allow navigation between time periods.

#### Scenario: Next week
- **WHEN** user clicks "Próxima semana" button
- **THEN** system loads appointments for next week

#### Scenario: Previous week
- **WHEN** user clicks "Semana anterior" button
- **THEN** system loads appointments for previous week

#### Scenario: Today button
- **WHEN** user clicks "Hoje" button
- **THEN** system returns to current week/day

#### Scenario: Date picker
- **WHEN** user clicks on date in header
- **THEN** system shows date picker for quick navigation

### Requirement: Schedule shows occupancy
The system SHALL display occupancy information for each time slot.

#### Scenario: Occupied slot
- **WHEN** time slot has appointment
- **THEN** slot shows appointment card with client name

#### Scenario: Available slot
- **WHEN** time slot has no appointments
- **THEN** slot shows "+" button for quick appointment creation

#### Scenario: Conflict indicator
- **WHEN** multiple appointments overlap
- **THEN** system shows conflict indicator (red border) on affected slots

### Requirement: Quick appointment creation
The system SHALL allow creating appointments directly from schedule.

#### Scenario: Click available slot
- **WHEN** user clicks "+" on empty time slot
- **THEN** system opens appointment form pre-filled with selected time

#### Scenario: Drag to create
- **WHEN** user drags on empty time range
- **THEN** system creates appointment spanning dragged duration

### Requirement: Schedule follows Stitch prototype
The schedule UI SHALL follow the design in `stitch-prototypes/agenda.html`.

#### Scenario: Week view layout
- **WHEN** viewing week
- **THEN** system shows left time column, day columns with appointments

#### Scenario: Appointment colors
- **WHEN** appointment is displayed
- **THEN** system colors by tipo: reunião (blue), follow-up (green), lembrete (yellow)

#### Scenario: Responsive layout
- **WHEN** viewing on mobile
- **THEN** system shows condensed day view with scrollable time slots
