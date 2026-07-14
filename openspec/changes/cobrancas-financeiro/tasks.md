## 1. Domain Layer - Transaction

- [ ] 1.1 Criar entidade Transaction (id, CompanyId, ClientId, Tipo, Valor, Data, Categoria, Descricao, Status, CreatedAt, UpdatedAt, DeletedAt)
- [ ] 1.2 Criar enum TransactionType (Receita, Despesa)
- [ ] 1.3 Criar enum TransactionStatus (Pending, Approved, Rejected)
- [ ] 1.4 Criar enum TransactionCategory (Servicos, Materiais, Frente, Impostos, Outros)
- [ ] 1.5 Criar Value Money (decimal 18,2) com validação de positivo
- [ ] 1.6 Implementar comportamentos: Transaction.Approve(), Transaction.Reject(), Transaction.Categorize()
- [ ] 1.7 Criar Domain Events: TransactionCreatedEvent, TransactionApprovedEvent, TransactionRejectedEvent

## 2. Domain Layer - Invoice

- [ ] 2.1 Criar entidade Invoice (id, CompanyId, ClientId, Valor, ValorPago, DataVencimento, DataPagamento, Descricao, Status, CreatedAt, UpdatedAt, DeletedAt)
- [ ] 2.2 Criar enum InvoiceStatus (Pending, Partial, Paid, Overdue, Cancelled)
- [ ] 2.3 Implementar comportamentos: Invoice.Pay(), Invoice.PartialPay(), Invoice.Cancel(), Invoice.MarkOverdue()
- [ ] 2.4 Criar InvoiceNumber Value Object (auto-incremento por empresa)
- [ ] 2.5 Criar Domain Events: InvoiceCreatedEvent, InvoicePaidEvent, InvoiceOverdueEvent

## 3. Infrastructure - EF Core

- [ ] 3.1 Criar TransactionConfiguration com Global Query Filter para CompanyId
- [ ] 3.2 Criar index em Data + CompanyId para queries por período
- [ ] 3.3 Criar index em Status + CompanyId para filtros
- [ ] 3.4 Criar InvoiceConfiguration com Global Query Filter para CompanyId
- [ ] 3.5 Criar index em DataVencimento + Status para detecção de overdue
- [ ] 3.6 Criar migration para Transactions e Invoices
- [ ] 3.7 Criar ITransactionRepository com CRUD + aggregates
- [ ] 3.8 Criar IInvoiceRepository com CRUD + status transitions

## 4. Infrastructure - Redis Cache

- [ ] 4.1 Criar IFinancialCacheService interface
- [ ] 4.2 Criar FinancialCacheService com GetDashboard, SetDashboard, InvalidateDashboard
- [ ] 4.3 Implementar TTL de 5 minutos para dashboard
- [ ] 4.4 Implementar invalidação ao criar/editar/deletar transação

## 5. Application - Transaction CRUD

- [ ] 5.1 Criar DTOs: TransactionResponse, CreateTransactionRequest, UpdateTransactionRequest
- [ ] 5.2 Criar CreateTransactionCommand + Handler com validação (FluentValidation)
- [ ] 5.3 Criar ListTransactionsQuery + Handler com filtros (data range, tipo, categoria, cliente)
- [ ] 5.4 Criar GetTransactionByIdQuery + Handler
- [ ] 5.5 Criar UpdateTransactionCommand + Handler
- [ ] 5.6 Criar DeleteTransactionCommand + Handler (soft delete)
- [ ] 5.7 Criar ApproveTransactionCommand + Handler
- [ ] 5.8 Criar RejectTransactionCommand + Handler

## 6. Application - Invoice CRUD

- [ ] 6.1 Criar DTOs: InvoiceResponse, CreateInvoiceRequest, UpdateInvoiceRequest
- [ ] 6.2 Criar CreateInvoiceCommand + Handler com validação
- [ ] 6.3 Criar ListInvoicesQuery + Handler com filtros
- [ ] 6.4 Criar GetInvoiceByIdQuery + Handler com payment history
- [ ] 6.5 Criar UpdateInvoiceCommand + Handler
- [ ] 6.6 Criar DeleteInvoiceCommand + Handler (soft delete)
- [ ] 6.7 Criar PayInvoiceCommand + Handler com validação de valor
- [ ] 6.8 Criar CancelInvoiceCommand + Handler
- [ ] 6.9 Criar BACKGROUND job para detectar overdue (diário)

## 7. Application - Dashboard

- [ ] 7.1 Criar GetDashboardQuery + Handler
- [ ] 7.2 Criar DashboardResponse DTO (KPIs, charts, recent)
- [ ] 7.3 Implementar cálculo de KPIs (saldo, receitas, despesas, pendentes, atrasadas)
- [ ] 7.4 Implementar aggregate de dados para gráfico mensal (últimos 12 meses)
- [ ] 7.5 Implementar aggregate de dados para gráfico de categorias
- [ ] 7.6 Integrar com cache Redis

## 8. Application - Export

- [ ] 8.1 Criar ExportTransactionsCommand + Handler
- [ ] 8.2 Criar ExportInvoicesCommand + Handler
- [ ] 8.3 Criar IExportService interface
- [ ] 8.4 Criar CsvExportService com CsvHelper
- [ ] 8.5 Criar XlsxExportService com ClosedXML
- [ ] 8.6 Criar ExportJob entity para rastrear progresso
- [ ] 8.7 Criar ExportJobRepository

## 9. API Layer

- [ ] 9.1 Criar TransactionsController com endpoints CRUD
- [ ] 9.2 Criar InvoicesController com endpoints CRUD
- [ ] 9.3 Criar DashboardController com endpoint de KPIs
- [ ] 9.4 Criar ExportController com endpoints de exportação
- [ ] 9.5 Configurar validação de valores financeiros (decimal)
- [ ] 9.6 Configurar Rate Limiting nos endpoints de exportação

## 10. Frontend - Dashboard

- [ ] 10.1 Criar página /financeiro conforme protótipo Stitch (financeiro.html)
- [ ] 10.2 Criar componente KPI cards (saldo, receitas, despesas, pendentes)
- [ ] 10.3 Criar componente bar chart (receitas vs despesas por mês)
- [ ] 10.4 Criar componente pie chart (despesas por categoria)
- [ ] 10.5 Criar componente lista de transações recentes
- [ ] 10.6 Integrar com GET /api/v1/dashboard

## 11. Frontend - Transactions

- [ ] 11.1 Criar página /financeiro/transacoes
- [ ] 11.2 Criar lista de transações com paginação e filtros
- [ ] 11.3 Criar formulário de criação/edição de transação
- [ ] 11.4 Criar seleção de tipo (receita/despesa) com cores
- [ ] 11.5 Criar seleção de categoria com ícones
- [ ] 11.6 Criar ações de aprovar/rejeitar com confirmação
- [ ] 11.7 Integrar com endpoints de transação

## 12. Frontend - Invoices

- [ ] 12.1 Criar página /financeiro/cobrancas
- [ ] 12.2 Criar lista de cobranças com status e cores
- [ ] 12.3 Criar formulário de criação/edição de cobrança
- [ ] 12.4 Criar modal de registro de pagamento
- [ ] 12.5 Criar ações de cancelar com confirmação
- [ ] 12.6 Integrar com endpoints de invoice

## 13. Frontend - Export

- [ ] 13.1 Criar modal de exportação com seleção de formato
- [ ] 13.2 Criar filtros de exportação (data range, tipo, categoria)
- [ ] 13.3 Criar indicador de progresso de exportação
- [ ] 13.4 Criar link de download com expiry
- [ ] 13.5 Integrar com endpoints de exportação

## 14. Testes

- [ ] 14.1 Testes unitários de Transaction (approve, reject, categorize)
- [ ] 14.2 Testes unitários de Invoice (pay, partialPay, cancel, markOverdue)
- [ ] 14.3 Testes de integração de Transaction CRUD (EF Core + PostgreSQL)
- [ ] 14.4 Testes de integração de Invoice CRUD
- [ ] 14.5 Testes de integração de Dashboard (KPIs + cache)
- [ ] 14.6 Testes de integração de Export (CSV + XLSX)
- [ ] 14.7 Testes de IDOR (across-tenant access deve retornar 404)
- [ ] 14.8 Testes E2E de criação de transação (Playwright)
- [ ] 14.9 Testes E2E de dashboard (Playwright)
- [ ] 14.10 Testes E2E de exportação (Playwright)

## 15. Lint e Qualidade

- [ ] 15.1 Rodar dotnet format no backend
- [ ] 15.2 Rodar ESLint no frontend (npm run lint)
- [ ] 15.3 Verificar cobertura mínima 80% backend, 70% frontend
- [ ] 15.4 Verificar acessibilidade (WCAG 2.1 AA) no dashboard financeiro
- [ ] 15.5 Verificar responsividade (mobile/tablet/desktop)
- [ ] 15.6 Verificar performance do dashboard (< 200ms para 95%)
