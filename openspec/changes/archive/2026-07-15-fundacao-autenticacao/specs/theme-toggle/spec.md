## ADDED Requirements

### Requirement: User can toggle theme between light, dark, and system
The system SHALL allow users to switch between light, dark, and system themes.

#### Scenario: Toggle to dark mode
- **WHEN** user clicks theme toggle button
- **THEN** page switches to dark mode with smooth transition (0.2s ease)

#### Scenario: Toggle to light mode
- **WHEN** user clicks theme toggle button while in dark mode
- **THEN** page switches to light mode with smooth transition

#### Scenario: System theme follows OS preference
- **WHEN** user selects "system" theme
- **THEN** page follows OS prefers-color-scheme setting

### Requirement: Theme preference persists across sessions
The system SHALL save theme preference in localStorage and sync with API.

#### Scenario: Preference saved to localStorage
- **WHEN** user changes theme
- **THEN** preference is saved to localStorage key "severina-theme"

#### Scenario: Preference synced to API
- **WHEN** user changes theme while authenticated
- **THEN** preference is sent to PUT /api/v1/users/preferences

#### Scenario: Preference loaded on login
- **WHEN** user logs in on new device
- **THEN** theme preference is loaded from API and applied

### Requirement: Theme toggle follows accessibility standards
The theme toggle SHALL meet WCAG 2.1 AA requirements.

#### Scenario: Toggle has proper ARIA attributes
- **WHEN** theme toggle renders
- **THEN** element has aria-label="Alternar tema", role="switch", and aria-checked attribute

#### Scenario: Keyboard accessible
- **WHEN** user presses Enter or Space on focused theme toggle
- **THEN** theme toggles and state updates

#### Scenario: Focus visible
- **WHEN** theme toggle is focused via keyboard
- **THEN** 2px solid focus ring is visible (color: primary-focus)

### Requirement: Theme transition is smooth
The system SHALL animate theme transitions without page reload.

#### Scenario: Color transition
- **WHEN** theme changes
- **THEN** all themed elements transition with "background-color 0.2s ease, color 0.2s ease"

#### Scenario: No page reload
- **WHEN** theme changes
- **THEN** page does not reload and state is preserved

### Requirement: Default theme is system
The system SHALL default to system theme on first visit.

#### Scenario: First visit
- **WHEN** user visits for first time without localStorage
- **THEN** theme defaults to system (follows prefers-color-scheme)

#### Scenario: Toggle icon reflects current theme
- **WHEN** theme is light
- **THEN** toggle shows moon icon (to switch to dark)

#### Scenario: Toggle icon in dark mode
- **WHEN** theme is dark
- **THEN** toggle shows sun icon (to switch to light)

### Requirement: Theme toggle UI follows Stitch prototype
The toggle SHALL follow the design in `stitch-prototypes/login.html`.

#### Scenario: Toggle position
- **WHEN** login page renders
- **THEN** theme toggle is positioned top-right corner

#### Scenario: Toggle animation
- **WHEN** user clicks toggle
- **THEN** icon rotates 180deg and scales 0.8→1 (200ms)
