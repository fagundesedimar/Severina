## 1. Domain Layer - Transaction

- [x] 1.1 Criar entidade Transaction (id, CompanyId, ClientId, Tipo, Valor, Data, Categoria, Descricao, Status, CreatedAt, UpdatedAt, DeletedAt)
- [x] 1.2 Criar enum TransactionType (Receita, Despesa)
- [x] 1.3 Criar enum TransactionStatus (Pending, Approved, Rejected)
- [x] 1.4 Criar enum TransactionCategory (Servicos, Materiais, Frente, Impostos, Outros)
- [x] 1.5 Criar Value Money (decimal 18,2) com validação de positivo
- [x] 1.6 Implementar comportamentos: Transaction.Approve(), Transaction.Reject(), Transaction.Categorize()
- [x] 1.7 Criar Domain Events: TransactionCreatedEvent, TransactionApprovedEvent, TransactionRejectedEvent

## 2. Domain Layer - Invoice

- [x] 2.1 Criar entidade Invoice (id, CompanyId, ClientId, Valor, ValorPago, DataVencimento, DataPagamento, Descricao, Status, CreatedAt, UpdatedAt, DeletedAt)
- [x] 2.2 Criar enum InvoiceStatus (Pending, Partial, Paid, Overdue, Cancelled)
- [x] 2.3 Implementar comportamentos: Invoice.Pay(), Invoice.PartialPay(), Invoice.Cancel(), Invoice.MarkOverdue()
- [x] 2.4 Criar InvoiceNumber Value Object (auto-incremento por empresa)
- [x] 2.5 Criar Domain Events: InvoiceCreatedEvent, InvoicePaidEvent, InvoiceOverdueEvent

## 3. Infrastructure - EF Core

- [x] 3.1 Criar TransactionConfiguration com Global Query Filter para CompanyId
- [x] 3.2 Criar index em Data + CompanyId para queries por período
- [x] 3.3 Criar index em Status + CompanyId para filtros
- [x] 3.4 Criar InvoiceConfiguration com Global Query Filter para CompanyId
- [x] 3.5 Criar index em DataVencimento + Status para detecção de overdue
- [x] 3.6 Criar migration para Transactions e Invoices (EnsureCreated used)
- [x] 3.7 Criar ITransactionRepository com CRUD + aggregates
- [x] 3.8 Criar IInvoiceRepository com CRUD + status transitions

## 4. Infrastructure - Redis Cache

- [x] 4.1 Criar IFinancialCacheService interface
- [x] 4.2 Criar FinancialCacheService com GetDashboard, SetDashboard, InvalidateDashboard
- [x] 4.3 Implementar TTL de 5 minutos para dashboard
- [x] 4.4 Implementar invalidação ao criar/editar/deletar transação

## 5. Application - Transaction CRUD

- [x] 5.1 Criar DTOs: TransactionResponse, CreateTransactionRequest, UpdateTransactionRequest
- [x] 5.2 Criar CreateTransactionCommand + Handler com validação (FluentValidation)
- [x] 5.3 Criar ListTransactionsQuery + Handler com filtros (data range, tipo, categoria, cliente)
- [x] 5.4 Criar GetTransactionByIdQuery + Handler
- [x] 5.5 Criar UpdateTransactionCommand + Handler
- [x] 5.6 Criar DeleteTransactionCommand + Handler (soft delete)
- [x] 5.7 Criar ApproveTransactionCommand + Handler
- [x] 5.8 Criar RejectTransactionCommand + Handler

## 6. Application - Invoice CRUD

- [x] 6.1 Criar DTOs: InvoiceResponse, CreateInvoiceRequest, UpdateInvoiceRequest
- [x] 6.2 Criar CreateInvoiceCommand + Handler com validação
- [x] 6.3 Criar ListInvoicesQuery + Handler com filtros
- [x] 6.4 Criar GetInvoiceByIdQuery + Handler com payment history
- [x] 6.5 Criar UpdateInvoiceCommand + Handler
- [x] 6.6 Criar DeleteInvoiceCommand + Handler (soft delete)
- [x] 6.7 Criar PayInvoiceCommand + Handler com validação de valor
- [x] 6.8 Criar CancelInvoiceCommand + Handler
- [x] 6.9 Criar BACKGROUND job para detectar overdue (diário)

## 7. Application - Dashboard

- [x] 7.1 Criar GetDashboardQuery + Handler
- [x] 7.2 Criar DashboardResponse DTO (KPIs, charts, recent)
- [x] 7.3 Implementar cálculo de KPIs (saldo, receitas, despesas, pendentes, atrasadas)
- [x] 7.4 Implementar aggregate de dados para gráfico mensal (últimos 12 meses)
- [x] 7.5 Implementar aggregate de dados para gráfico de categorias
- [x] 7.6 Integrar com cache Redis

## 8. Application - Export

- [x] 8.1 Criar ExportTransactionsCommand + Handler
- [x] 8.2 Criar ExportInvoicesCommand + Handler
- [x] 8.3 Criar IExportService interface
- [x] 8.4 Criar CsvExportService com CsvHelper
- [x] 8.5 Criar XlsxExportService com ClosedXML
- [x] 8.6 Criar ExportJob entity para rastrear progresso
- [x] 8.7 Criar ExportJobRepository

## 9. API Layer

- [x] 9.1 Criar TransactionsController com endpoints CRUD
- [x] 9.2 Criar InvoicesController com endpoints CRUD
- [x] 9.3 Criar DashboardController com endpoint de KPIs
- [x] 9.4 Criar ExportController com endpoints de exportação
- [x] 9.5 Configurar validação de valores financeiros (decimal)
- [x] 9.6 Configurar Rate Limiting nos endpoints de exportação

## 10. Frontend - Dashboard

- [x] 10.1 Criar página /financeiro conforme protótipo Stitch (financeiro.html)
- [x] 10.2 Criar componente KPI cards (saldo, receitas, despesas, pendentes)
- [x] 10.3 Criar componente bar chart (receitas vs despesas por mês)
- [x] 10.4 Criar componente pie chart (despesas por categoria)
- [x] 10.5 Criar componente lista de transações recentes
- [x] 10.6 Integrar com GET /api/v1/dashboard

## 11. Frontend - Transactions

- [x] 11.1 Criar página /financeiro/transacoes
- [x] 11.2 Criar lista de transações com paginação e filtros
- [x] 11.3 Criar formulário de criação/edição de transação
- [x] 11.4 Criar seleção de tipo (receita/despesa) com cores
- [x] 11.5 Criar seleção de categoria com ícones
- [x] 11.6 Criar ações de aprovar/rejeitar com confirmação
- [x] 11.7 Integrar com endpoints de transação

## 12. Frontend - Invoices

- [x] 12.1 Criar página /financeiro/cobrancas
- [x] 12.2 Criar lista de cobranças com status e cores
- [x] 12.3 Criar formulário de criação/edição de cobrança
- [x] 12.4 Criar modal de registro de pagamento
- [x] 12.5 Criar ações de cancelar com confirmação
- [x] 12.6 Integrar com endpoints de invoice

## 13. Frontend - Export

- [x] 13.1 Criar modal de exportação com seleção de formato
- [x] 13.2 Criar filtros de exportação (data range, tipo, categoria)
- [x] 13.3 Criar indicador de progresso de exportação
- [x] 13.4 Criar link de download com expiry
- [x] 13.5 Integrar com endpoints de exportação

## 14. Testes

- [x] 14.1 Testes unitários de Transaction (approve, reject, categorize)
- [x] 14.2 Testes unitários de Invoice (pay, partialPay, cancel, markOverdue)
- [x] 14.3 Testes de integração de Transaction CRUD (EF Core + PostgreSQL)
- [x] 14.4 Testes de integração de Invoice CRUD
- [x] 14.5 Testes de integração de Dashboard (KPIs + cache)
- [x] 14.6 Testes de integração de Export (CSV + XLSX)
- [x] 14.7 Testes de IDOR (across-tenant access deve retornar 404)
- [x] 14.8 Testes E2E de criação de transação (Playwright)
- [x] 14.9 Testes E2E de dashboard (Playwright)
- [x] 14.10 Testes E2E de exportação (Playwright)

## 15. Lint e Qualidade

- [x] 15.1 Rodar dotnet format no backend
- [x] 15.2 Rodar ESLint no frontend (npm run lint)
- [x] 15.3 Verificar cobertura mínima 80% backend, 70% frontend
- [x] 15.4 Verificar acessibilidade (WCAG 2.1 AA) no dashboard financeiro
- [x] 15.5 Verificar responsividade (mobile/tablet/desktop)
- [x] 15.6 Verificar performance do dashboard (< 200ms para 95%)
