# Dashboard Charts

## Purpose

Chart visualizations for attendance, sources, and trends on the dashboard.

## Requirements

### Requirement: Dashboard shows attendance bar chart
The system SHALL display bar chart of daily attendance.

#### Scenario: Daily bars
- **WHEN** dashboard loads
- **THEN** system shows bar chart with last 30 days, bars for atendimentos (blue)

#### Scenario: Day selection
- **WHEN** user clicks on day bar
- **THEN** system filters dashboard for selected day

#### Scenario: Empty days
- **WHEN** day has no attendance
- **THEN** system shows zero-height bar

#### Scenario: Hover tooltip
- **WHEN** user hovers over bar
- **THEN** system shows tooltip with date and count

### Requirement: Dashboard shows source pie chart
The system SHALL display pie chart of attendance by source.

#### Scenario: Source distribution
- **WHEN** dashboard loads
- **THEN** system shows pie chart with sources (WhatsApp, Instagram, Web, Telefone) and percentages

#### Scenario: Source click
- **WHEN** user clicks on source slice
- **THEN** system filters dashboard by selected source

#### Scenario: Unknown source
- **WHEN** attendance has unknown source
- **THEN** system groups as "Outros"

### Requirement: Dashboard shows trend line chart
The system SHALL display line chart of attendance trend.

#### Scenario: 30-day trend
- **WHEN** dashboard loads
- **THEN** system shows line chart with daily attendance count for last 30 days

#### Scenario: Comparison line
- **WHEN** user enables comparison
- **THEN** system shows previous 30 days as dashed line

#### Scenario: Trend annotation
- **WHEN** significant peak/drop exists
- **THEN** system shows annotation with date and count

### Requirement: Dashboard charts follow Stitch prototype
The chart UI SHALL follow the design in `stitch-prototypes/dashboard.html`.

#### Scenario: Chart colors
- **WHEN** charts render
- **THEN** system uses blue for attendance, green for clients, red for expenses

#### Scenario: Chart responsive
- **WHEN** viewing on mobile
- **THEN** system shows charts full-width with scrollable container

#### Scenario: Chart loading
- **WHEN** charts are loading
- **THEN** system shows skeleton placeholder
