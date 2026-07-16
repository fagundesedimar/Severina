# Invoice Management

## Purpose

Create, update, pay, cancel, and manage invoices/charges.

## Requirements

### Requirement: User can create invoice/charge
The system SHALL allow authenticated users to create invoices/charges for clients.

#### Scenario: Successful creation
- **WHEN** user sends POST /api/v1/invoices with valid data (cliente_id, valor, data_vencimento, descricao)
- **THEN** system returns 201 Created with invoice data including id and status "pending"

#### Scenario: Invoice with linked client
- **WHEN** user creates invoice with cliente_id
- **THEN** system links invoice to client profile

#### Scenario: Invoice without client
- **WHEN** user creates invoice without cliente_id
- **THEN** system allows creation (generic charge)

#### Scenario: Valor zero
- **WHEN** user sends POST /api/v1/invoices with valor=0
- **THEN** system returns 400 Bad Request with error "Valor deve ser maior que zero"

#### Scenario: Past due date
- **WHEN** user sends POST /api/v1/invoices with data_vencimento in the past
- **THEN** system allows creation (cobrança retroativa)

### Requirement: User can list invoices
The system SHALL return paginated list of invoices for the user's company.

#### Scenario: Successful list
- **WHEN** user sends GET /api/v1/invoices with optional filters
- **THEN** system returns 200 OK with paginated list sorted by data_vencimento

#### Scenario: Filter by status
- **WHEN** user sends GET /api/v1/invoices?status=pending
- **THEN** system returns only invoices with specified status

#### Scenario: Filter by client
- **WHEN** user sends GET /api/v1/invoices?clientId={id}
- **THEN** system returns only invoices linked to specified client

#### Scenario: Filter by date range
- **WHEN** user sends GET /api/v1/invoices?from=2026-07-01&to=2026-07-31
- **THEN** system returns only invoices within specified range

### Requirement: User can view invoice details
The system SHALL return full invoice data including payment history.

#### Scenario: Successful view
- **WHEN** user sends GET /api/v1/invoices/{id} where id belongs to user's company
- **THEN** system returns 200 OK with full invoice data and payment history

#### Scenario: Invoice not found
- **WHEN** user sends GET /api/v1/invoices/{id} with invalid id
- **THEN** system returns 404 Not Found

### Requirement: User can update invoice
The system SHALL allow users to update invoice data.

#### Scenario: Successful update
- **WHEN** user sends PUT /api/v1/invoices/{id} with valid data
- **THEN** system returns 200 OK with updated invoice data

#### Scenario: Update paid invoice
- **WHEN** user tries to update invoice with status "paid"
- **THEN** system returns 409 Conflict with error "Cobrança paga não pode ser alterada"

### Requirement: User can mark invoice as paid
The system SHALL allow users to mark invoices as paid.

#### Scenario: Successful payment
- **WHEN** user sends POST /api/v1/invoices/{id}/pay with valor_pago and data_pagamento
- **THEN** system sets status to "paid" and returns 200 OK

#### Scenario: Partial payment
- **WHEN** user sends POST /api/v1/invoices/{id}/pay with valor_pago < valor
- **THEN** system sets status to "partial" and records payment

#### Scenario: Overpayment
- **WHEN** user sends POST /api/v1/invoices/{id}/pay with valor_pago > valor
- **THEN** system returns 400 Bad Request with error "Valor pago excede valor da cobrança"

### Requirement: User can cancel invoice
The system SHALL allow users to cancel pending invoices.

#### Scenario: Successful cancellation
- **WHEN** user sends POST /api/v1/invoices/{id}/cancel
- **THEN** system sets status to "cancelled" and returns 200 OK

#### Scenario: Cancel paid invoice
- **WHEN** user tries to cancel invoice with status "paid"
- **THEN** system returns 409 Conflict with error "Cobrança paga não pode ser cancelada"

### Requirement: User can delete invoice
The system SHALL allow users to soft-delete invoices.

#### Scenario: Successful deletion
- **WHEN** user sends DELETE /api/v1/invoices/{id}
- **THEN** system sets deletedAt timestamp and returns 204 No Content

### Requirement: Invoice has status lifecycle
The system SHALL manage invoice status transitions.

#### Scenario: Status transitions
- **WHEN** invoice is created
- **THEN** status is "pending"

#### Scenario: Payment status
- **WHEN** invoice is marked as paid
- **THEN** status changes to "paid" and paidAt is set

#### Scenario: Overdue detection
- **WHEN** invoice data_vencimento is past and status is "pending"
- **THEN** system marks invoice as "overdue" automatically
