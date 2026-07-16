## ADDED Requirements

### Requirement: User can import clients from CSV/XLSX
The system SHALL allow users to upload CSV or XLSX files with client data.

#### Scenario: Successful upload
- **WHEN** user sends POST /api/v1/clients/import with valid CSV/XLSX file (≤1000 rows)
- **THEN** system returns 202 Accepted with import job id

#### Scenario: File too large
- **WHEN** user sends POST /api/v1/clients/import with file containing >1000 rows
- **THEN** system returns 400 Bad Request with error "Arquivo excede limite de 1000 registros"

#### Scenario: Invalid file format
- **WHEN** user sends POST /api/v1/clients/import with unsupported file format (e.g., .txt)
- **THEN** system returns 400 Bad Request with error "Formato não suportado. Use CSV ou XLSX"

#### Scenario: Empty file
- **WHEN** user sends POST /api/v1/clients/import with empty file
- **THEN** system returns 400 Bad Request with error "Arquivo vazio"

### Requirement: Import validates data before processing
The system SHALL validate all rows before importing.

#### Scenario: Validation preview
- **WHEN** user uploads file
- **THEN** system returns validation preview with total rows, valid rows, and error rows

#### Scenario: Row validation errors
- **WHEN** row has invalid email or missing required fields
- **THEN** system marks row as error with specific error message

#### Scenario: Duplicate detection
- **WHEN** row has email that already exists in company
- **THEN** system marks row as warning (will be skipped unless updateExisting=true)

### Requirement: Import processes in background
The system SHALL process large imports asynchronously.

#### Scenario: Background processing
- **WHEN** import job is created
- **THEN** system processes rows in background and updates progress

#### Scenario: Progress status
- **WHEN** user sends GET /api/v1/clients/import/{jobId}
- **THEN** system returns current status (processing, completed, failed) with progress percentage

#### Scenario: Import completion notification
- **WHEN** import job completes
- **THEN** system sends notification with summary (imported, skipped, errors)

### Requirement: Import supports conflict resolution
The system SHALL handle duplicate emails during import.

#### Scenario: Skip duplicates (default)
- **WHEN** import encounters duplicate email and updateExisting=false
- **THEN** system skips duplicate row and adds to skipped count

#### Scenario: Update existing
- **WHEN** import encounters duplicate email and updateExisting=true
- **THEN** system updates existing client with new data

### Requirement: Import follows Stitch prototype
The import UI SHALL follow the design in `stitch-prototypes/clientes.html`.

#### Scenario: Upload component
- **WHEN** user navigates to import page
- **THEN** system displays drag-and-drop area with "Arraste seu arquivo aqui" text

#### Scenario: Preview table
- **WHEN** file is uploaded
- **THEN** system shows preview table with first 10 rows and validation status

#### Scenario: Progress indicator
- **WHEN** import is processing
- **THEN** system shows progress bar with percentage and estimated time
