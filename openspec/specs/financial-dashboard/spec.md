# Financial Dashboard

## Purpose

Financial indicators, charts, and recent transactions for the financial module.

## Requirements

### Requirement: Dashboard shows financial KPIs
The system SHALL display key financial indicators on dashboard.

#### Scenario: KPIs displayed
- **WHEN** user navigates to /financeiro
- **THEN** system shows: saldo, receitasMes, despesasMes, previsaoProximoMes, contasPendentes, contasAtrasadas

#### Scenario: Saldo calculation
- **WHEN** dashboard loads
- **THEN** system calculates saldo = totalReceitas - totalDespesas (approved transactions)

#### Scenario: Contas pendentes
- **WHEN** dashboard loads
- **THEN** system counts invoices with status "pending"

#### Scenario: Contas atrasadas
- **WHEN** dashboard loads
- **THEN** system counts invoices with status "overdue"

### Requirement: Dashboard shows monthly chart
The system SHALL display bar chart of income vs expenses by month.

#### Scenario: Monthly bars
- **WHEN** dashboard loads
- **THEN** system shows bar chart with last 12 months, bars for receitas (green) and despesas (red)

#### Scenario: Month selection
- **WHEN** user clicks on month bar
- **THEN** system filters transactions for selected month

#### Scenario: Empty months
- **WHEN** month has no transactions
- **THEN** system shows zero-height bar

### Requirement: Dashboard shows category pie chart
The system SHALL display pie chart of expenses by category.

#### Scenario: Category distribution
- **WHEN** dashboard loads
- **THEN** system shows pie chart with expense categories and percentages

#### Scenario: Category click
- **WHEN** user clicks on category slice
- **THEN** system filters transactions by selected category

#### Scenario: Top categories
- **WHEN** more than 6 categories exist
- **THEN** system groups smaller categories as "Outros"

### Requirement: Dashboard shows recent transactions
The system SHALL display list of recent transactions.

#### Scenario: Recent list
- **WHEN** dashboard loads
- **THEN** system shows last 10 transactions with tipo, valor, data, categoria

#### Scenario: Transaction click
- **WHEN** user clicks on transaction
- **THEN** system navigates to transaction detail

#### Scenario: See all link
- **WHEN** user clicks "Ver todas"
- **THEN** system navigates to /financeiro/transacoes

### Requirement: Dashboard follows Stitch prototype
The dashboard UI SHALL follow the design in `stitch-prototypes/financeiro.html`.

#### Scenario: KPI cards layout
- **WHEN** dashboard renders
- **THEN** system shows KPI cards in responsive grid (2 cols mobile, 4 cols desktop)

#### Scenario: Chart colors
- **WHEN** charts render
- **THEN** system uses green for income, red for expenses, blue for accent

#### Scenario: JetBrains Mono for values
- **WHEN** KPI values display
- **THEN** system uses JetBrains Mono font for financial values

### Requirement: Dashboard uses cache
The system SHALL cache dashboard data in Redis.

#### Scenario: Cache hit
- **WHEN** user requests dashboard within 5 minutes of last request
- **THEN** system returns cached data

#### Scenario: Cache miss
- **WHEN** user requests dashboard after 5 minutes
- **THEN** system recalculates and caches new data

#### Scenario: Cache invalidation
- **WHEN** transaction is created/updated/deleted
- **THEN** system invalidates dashboard cache for that company

### Requirement: Dashboard is performant
The system SHALL load dashboard in under 200ms.

#### Scenario: Performance requirement
- **WHEN** user navigates to /financeiro
- **THEN** dashboard loads within 200ms (95th percentile)
