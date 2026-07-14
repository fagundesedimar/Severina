## ADDED Requirements

### Requirement: User can export transactions to CSV/XLSX
The system SHALL allow users to export transaction data.

#### Scenario: Successful export request
- **WHEN** user sends POST /api/v1/transactions/export with optional filters
- **THEN** system returns 202 Accepted with export job id

#### Scenario: Export with filters
- **WHEN** user requests export with filters (date range, type, category)
- **THEN** system exports only filtered transactions

#### Scenario: Export all
- **WHEN** user requests export without filters
- **THEN** system exports all transactions for company

#### Scenario: Export format CSV
- **WHEN** user specifies format="csv"
- **THEN** system generates CSV file with columns: data, tipo, valor, categoria, descricao, status

#### Scenario: Export format XLSX
- **WHEN** user specifies format="xlsx"
- **THEN** system generates XLSX file with formatted headers and cell types

### Requirement: Export processes in background
The system SHALL process exports asynchronously.

#### Scenario: Background processing
- **WHEN** export job is created
- **THEN** system processes in background and updates status

#### Scenario: Progress status
- **WHEN** user sends GET /api/v1/transactions/export/{jobId}
- **THEN** system returns current status (processing, completed, failed) with progress percentage

#### Scenario: Export completion
- **WHEN** export job completes
- **THEN** system provides download link with 24-hour expiry

#### Scenario: Export failure
- **WHEN** export job fails
- **THEN** system returns error message and allows retry

### Requirement: Export respects row limit
The system SHALL limit export to 100,000 rows.

#### Scenario: Within limit
- **WHEN** export request has ≤100,000 matching rows
- **THEN** system processes full export

#### Scenario: Exceeds limit
- **WHEN** export request has >100,000 matching rows
- **THEN** system returns 400 Bad Request with error "Exportação limitada a 100.000 registros"

### Requirement: Export follows Stitch prototype
The export UI SHALL follow the design in `stitch-prototypes/financeiro.html`.

#### Scenario: Export button
- **WHEN** user views transaction list
- **THEN** system shows "Exportar" button in toolbar

#### Scenario: Export options
- **WHEN** user clicks export button
- **THEN** system shows modal with format selection (CSV/XLSX) and filter options

#### Scenario: Download notification
- **WHEN** export is ready
- **THEN** system shows toast with download link

### Requirement: User can export invoices
The system SHALL allow users to export invoice data.

#### Scenario: Invoice export
- **WHEN** user sends POST /api/v1/invoices/export
- **THEN** system returns 202 Accepted with export job id

#### Scenario: Invoice export columns
- **WHEN** invoice export completes
- **THEN** file contains: numero, cliente, valor, data_vencimento, data_pagamento, status
