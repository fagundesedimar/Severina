## ADDED Requirements

### Requirement: User can create transaction
The system SHALL allow authenticated users to create new transactions (income/expense).

#### Scenario: Successful creation
- **WHEN** user sends POST /api/v1/transactions with valid data (tipo, valor, data, categoria, descricao, cliente_id optional)
- **THEN** system returns 201 Created with transaction data including id and status "pending"

#### Scenario: Income transaction
- **WHEN** user sends POST /api/v1/transactions with tipo="receita"
- **THEN** system creates income transaction with positive valor

#### Scenario: Expense transaction
- **WHEN** user sends POST /api/v1/transactions with tipo="despesa"
- **THEN** system creates expense transaction with positive valor (absolute value)

#### Scenario: Negative valor
- **WHEN** user sends POST /api/v1/transactions with negative valor
- **THEN** system returns 400 Bad Request with error "Valor deve ser positivo"

#### Scenario: Transaction date in future
- **WHEN** user sends POST /api/v1/transactions with data in the future
- **THEN** system allows creation (agendamento de receitas/despesas)

#### Scenario: Missing required fields
- **WHEN** user sends POST /api/v1/transactions without tipo, valor, or data
- **THEN** system returns 400 Bad Request with validation errors

### Requirement: User can list transactions
The system SHALL return paginated list of transactions for the user's company.

#### Scenario: Successful list
- **WHEN** user sends GET /api/v1/transactions with optional filters
- **THEN** system returns 200 OK with paginated list sorted by data descending

#### Scenario: Filter by type
- **WHEN** user sends GET /api/v1/transactions?tipo=receita
- **THEN** system returns only income transactions

#### Scenario: Filter by date range
- **WHEN** user sends GET /api/v1/transactions?from=2026-07-01&to=2026-07-31
- **THEN** system returns only transactions within specified range

#### Scenario: Filter by category
- **WHEN** user sends GET /api/v1/transactions?categoria=servicos
- **THEN** system returns only transactions with specified category

#### Scenario: Filter by client
- **WHEN** user sends GET /api/v1/transactions?clientId={id}
- **THEN** system returns only transactions linked to specified client

#### Scenario: Cross-tenant isolation
- **WHEN** user sends GET /api/v1/transactions
- **THEN** system returns only transactions belonging to user's company_id

### Requirement: User can view transaction details
The system SHALL return full transaction data.

#### Scenario: Successful view
- **WHEN** user sends GET /api/v1/transactions/{id} where id belongs to user's company
- **THEN** system returns 200 OK with full transaction data

#### Scenario: Transaction not found
- **WHEN** user sends GET /api/v1/transactions/{id} with invalid id
- **THEN** system returns 404 Not Found

#### Scenario: Cross-tenant access
- **WHEN** user sends GET /api/v1/transactions/{id} where id belongs to another company
- **THEN** system returns 404 Not Found

### Requirement: User can update transaction
The system SHALL allow users to update transaction data.

#### Scenario: Successful update
- **WHEN** user sends PUT /api/v1/transactions/{id} with valid data
- **THEN** system returns 200 OK with updated transaction data

#### Scenario: Update approved transaction
- **WHEN** user tries to update transaction with status "approved"
- **THEN** system returns 409 Conflict with error "Transação aprovada não pode ser alterada"

### Requirement: User can delete transaction
The system SHALL allow users to soft-delete transactions.

#### Scenario: Successful deletion
- **WHEN** user sends DELETE /api/v1/transactions/{id}
- **THEN** system sets deletedAt timestamp and returns 204 No Content

#### Scenario: Delete approved transaction
- **WHEN** user tries to delete transaction with status "approved"
- **THEN** system returns 409 Conflict with error "Transação aprovada não pode ser removida"

### Requirement: User can approve transaction
The system SHALL allow users to approve pending transactions.

#### Scenario: Successful approval
- **WHEN** user sends POST /api/v1/transactions/{id}/approve
- **THEN** system sets status to "approved" and returns 200 OK

#### Scenario: Approve already approved
- **WHEN** user tries to approve transaction with status "approved"
- **THEN** system returns 409 Conflict with error "Transação já foi aprovada"

### Requirement: User can reject transaction
The system SHALL allow users to reject pending transactions.

#### Scenario: Successful rejection
- **WHEN** user sends POST /api/v1/transactions/{id}/reject with motivo
- **THEN** system sets status to "rejected" and returns 200 OK

#### Scenario: Rejection without motivo
- **WHEN** user sends POST /api/v1/transactions/{id}/reject without motivo
- **THEN** system returns 400 Bad Request with error "Motivo é obrigatório para rejeição"
