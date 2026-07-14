## ADDED Requirements

### Requirement: Dashboard shows attendance KPIs
The system SHALL display key attendance indicators on main dashboard.

#### Scenario: KPIs displayed
- **WHEN** user navigates to /dashboard
- **THEN** system shows: atendimentosHoje, atendimentosPendentes, taxaConversao, tempoMedioResposta

#### Scenario: Atendimentos hoje
- **WHEN** dashboard loads
- **THEN** system counts conversations created today for user's company

#### Scenario: Atendimentos pendentes
- **WHEN** dashboard loads
- **THEN** system counts conversations with status "pending" or "waiting"

#### Scenario: Taxa de conversão
- **WHEN** dashboard loads
- **THEN** system calculates (conversões / total atendimentos) * 100 for current month

#### Scenario: Tempo médio de resposta
- **WHEN** dashboard loads
- **THEN** system calculates average time between first message and first response

### Requirement: Dashboard shows client KPIs
The system SHALL display client-related indicators.

#### Scenario: Clientes ativos
- **WHEN** dashboard loads
- **THEN** system counts clients with status "active" in current month

#### Scenario: Novos clientes
- **WHEN** dashboard loads
- **THEN** system counts clients created in current month

#### Scenario: Clientes inativos
- **WHEN** dashboard loads
- **THEN** system counts clients with no interactions in last 30 days

### Requirement: Dashboard shows financial KPIs
The system SHALL display financial indicators (consolidated from Billing module).

#### Scenario: Faturamento do mês
- **WHEN** dashboard loads
- **THEN** system sums approved income transactions for current month

#### Scenario: Despesas do mês
- **WHEN** dashboard loads
- **THEN** system sums approved expense transactions for current month

#### Scenario: Saldo
- **WHEN** dashboard loads
- **THEN** system calculates faturamento - despesas for current month

#### Scenario: Cobranças pendentes
- **WHEN** dashboard loads
- **THEN** system counts invoices with status "pending" or "overdue"

### Requirement: Dashboard shows agenda KPIs
The system SHALL display schedule-related indicators.

#### Scenario: Compromissos hoje
- **WHEN** dashboard loads
- **THEN** system counts appointments scheduled for today

#### Scenario: Compromissos pendentes
- **WHEN** dashboard loads
- **THEN** system counts appointments with status "scheduled" for next 7 days

#### Scenario: Próximo compromisso
- **WHEN** dashboard loads
- **THEN** system shows next upcoming appointment with client name and time

### Requirement: Dashboard follows Stitch prototype
The dashboard UI SHALL follow the design in `stitch-prototypes/dashboard.html`.

#### Scenario: KPI cards layout
- **WHEN** dashboard renders
- **THEN** system shows KPI cards in responsive grid (1 col mobile, 2 cols tablet, 4 cols desktop)

#### Scenario: KPI card design
- **WHEN** KPI card renders
- **THEN** system shows icon, label, value, and trend indicator (up/down/neutral)

#### Scenario: Trend colors
- **WHEN** trend is positive
- **THEN** system shows green arrow up

#### Scenario: Trend negative
- **WHEN** trend is negative
- **THEN** system shows red arrow down

#### Scenario: JetBrains Mono for values
- **WHEN** KPI values display
- **THEN** system uses JetBrains Mono font
