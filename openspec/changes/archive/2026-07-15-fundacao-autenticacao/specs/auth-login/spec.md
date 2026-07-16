## ADDED Requirements

### Requirement: User can login with email and password
The system SHALL authenticate users with email and password, returning JWT access token and refresh token.

#### Scenario: Successful login
- **WHEN** user sends POST /api/v1/auth/login with valid email and password
- **THEN** system returns 200 OK with accessToken (15min expiry) and sets refresh token in HTTP-only cookie (7 days)

#### Scenario: Invalid credentials
- **WHEN** user sends POST /api/v1/auth/login with invalid email or password
- **THEN** system returns 401 Unauthorized with error message "Credenciais inválidas"

#### Scenario: Empty fields
- **WHEN** user sends POST /api/v1/auth/login with empty email or password
- **THEN** system returns 400 Bad Request with validation errors

#### Scenario: Non-existent email
- **WHEN** user sends POST /api/v1/auth/login with email that doesn't exist
- **THEN** system returns 401 Unauthorized (same as invalid credentials for security)

### Requirement: User can refresh access token
The system SHALL issue new access tokens when refresh token is valid.

#### Scenario: Successful refresh
- **WHEN** user sends POST /api/v1/auth/refresh with valid refresh token in cookie
- **THEN** system returns 200 OK with new accessToken

#### Scenario: Expired refresh token
- **WHEN** user sends POST /api/v1/auth/refresh with expired refresh token
- **THEN** system returns 401 Unauthorized and clears refresh token cookie

#### Scenario: Invalid refresh token
- **WHEN** user sends POST /api/v1/auth/refresh with invalid refresh token
- **THEN** system returns 401 Unauthorized

### Requirement: User can logout
The system SHALL invalidate refresh tokens and clear session on logout.

#### Scenario: Successful logout
- **WHEN** user sends POST /api/v1/auth/logout with valid session
- **THEN** system returns 200 OK, clears refresh token cookie, and invalidates refresh token

### Requirement: Login UI follows Stitch prototype
The login page SHALL follow the design defined in `stitch-prototypes/login.html`.

#### Scenario: Login form renders correctly
- **WHEN** user navigates to /login
- **THEN** system displays form with email field, password field, "Entrar" button, and "Cadastre-se" link

#### Scenario: Password visibility toggle
- **WHEN** user clicks eye icon next to password field
- **THEN** password field toggles between masked and visible

#### Scenario: Loading state on submit
- **WHEN** user clicks "Entrar" button
- **THEN** button shows loading spinner and text "Autenticando..."

#### Scenario: Theme toggle works
- **WHEN** user clicks theme toggle button
- **THEN** page switches between light and dark mode with animation

### Requirement: JWT contains company_id claim
The system SHALL include company_id in JWT claims for multi-tenant isolation.

#### Scenario: JWT contains company_id
- **WHEN** user authenticates successfully
- **THEN** JWT payload includes company_id claim with user's company ID

#### Scenario: API validates company_id
- **WHEN** authenticated user sends request to any protected endpoint
- **THEN** backend extracts company_id from JWT and filters data by company_id

### Requirement: Password validation enforces security
The system SHALL enforce password complexity requirements.

#### Scenario: Password too short
- **WHEN** user tries to register with password less than 8 characters
- **THEN** system returns 400 Bad Request with error "Senha deve ter no mínimo 8 caracteres"

#### Scenario: Password missing complexity
- **WHEN** user tries to register with password without uppercase, number, or special character
- **THEN** system returns 400 Bad Request with error "Senha deve conter maiúscula, número e caractere especial"
