## 1. Application - Dashboard Service

- [ ] 1.1 Criar IDashboardService interface com métodos: GetKpis, GetCharts, GetActivity, GetPendingTasks
- [ ] 1.2 Criar DashboardService implementation com injeção de dependência de todos os módulos
- [ ] 1.3 Criar DashboardResponse DTO consolidado (KPIs, charts, activity, tasks)
- [ ] 1.4 Criar KpisDto (atendimentosHoje, pendentes, taxaConversao, tempoMedioResposta, clientesAtivos, novosClientes, faturamento, despesas, saldo, compromissosHoje)
- [ ] 1.5 Criar ChartsDto (barData, pieData, lineData)
- [ ] 1.6 Criar ActivityDto (id, type, description, timestamp, sourceUrl)
- [ ] 1.7 Criar PendingTaskDto (id, type, title, priority, dueDate, sourceUrl)

## 2. Application - KPI Aggregation

- [ ] 2.1 Criar GetDashboardQuery + Handler
- [ ] 2.2 Implementar agregação de atendimentos (hoje, pendentes, taxa conversão, tempo resposta)
- [ ] 2.3 Implementar agregação de clientes (ativos, novos, inativos)
- [ ] 2.4 Implementar agregação financeiro (faturamento, despesas, saldo, pendentes)
- [ ] 2.5 Implementar agregação de agenda (compromissos hoje, pendentes, próximo)
- [ ] 2.6 Implementar cálculo de tendências (comparação com mês anterior)

## 3. Application - Charts Aggregation

- [ ] 3.1 Implementar aggregate de bar chart (atendimentos/dia últimos 30 dias)
- [ ] 3.2 Implementar aggregate de pie chart (atendimentos por origem)
- [ ] 3.3 Implementar aggregate de line chart (tendência últimos 30 dias)
- [ ] 3.4 Implementar comparison line (mês anterior vs atual)

## 4. Application - Activity Feed

- [ ] 4.1 Implementar agregação de atividades recentes (últimas 10)
- [ ] 4.2 Implementar atividades de atendimento (mensagens recebidas/enviadas)
- [ ] 4.3 Implementar atividades de clientes (criados, atualizados)
- [ ] 4.4 Implementar atividades financeiras (pagamentos, cobranças)
- [ ] 4.5 Implementar atividades de agenda (compromissos criados, concluídos)

## 5. Application - Pending Tasks

- [ ] 5.1 Implementar agregação de tarefas pendentes
- [ ] 5.2 Implementar detecção de cobranças atrasadas
- [ ] 5.3 Implementar detecção de mensagens sem resposta
- [ ] 5.4 Implementar detecção de compromissos próximos
- [ ] 5.5 Implementar priorização de tarefas (overdue > pending > upcoming)

## 6. Infrastructure - Redis Cache

- [ ] 6.1 Criar IDashboardCacheService interface
- [ ] 6.2 Criar DashboardCacheService com Get, Set, Invalidate
- [ ] 6.3 Implementar chave de cache "dashboard:{companyId}"
- [ ] 6.4 Implementar TTL de 5 minutos
- [ ] 6.5 Implementar invalidação em mutations de todos os módulos
- [ ] 6.6 Implementar cache warming job (background na inicialização)

## 7. Application - Partial Failure Handling

- [ ] 7.1 Implementar Circuit Breaker para cada módulo
- [ ] 7.2 Implementar timeout de 2 segundos por módulo
- [ ] 7.3 Implementar fallback com dados parciais
- [ ] 7.4 Implementar logging de erros por módulo

## 8. API Layer

- [ ] 8.1 Criar DashboardController com endpoint GET /api/v1/dashboard
- [ ] 8.2 Configurar autenticação JWT no endpoint
- [ ] 8.3 Configurar rate limiting (max 60 req/min por empresa)
- [ ] 8.4 Configurar headers de cache (Cache-Control, ETag)

## 9. Frontend - Dashboard Page

- [ ] 9.1 Criar página /dashboard conforme protótipo Stitch (dashboard.html)
- [ ] 9.2 Criar layout responsivo (grid 1/2/4 cols)
- [ ] 9.3 Criar skeleton loading para todas as seções
- [ ] 9.4 Criar error boundary para falhas parciais
- [ ] 9.5 Integrar com GET /api/v1/dashboard via React Query

## 10. Frontend - KPI Cards

- [ ] 10.1 Criar componente KpiCard (icon, label, value, trend)
- [ ] 10.2 Criar componente KpiGrid (grid responsivo)
- [ ] 10.3 Implementar trend indicators (seta verde/vermelha)
- [ ] 10.4 Implementar formatação de valores (moeda, porcentagem, tempo)
- [ ] 10.5 Implementar fonte JetBrains Mono para valores

## 11. Frontend - Charts

- [ ] 11.1 Criar componente BarChart (atendimentos/dia)
- [ ] 11.2 Criar componente PieChart (atendimentos/origem)
- [ ] 11.3 Criar componente LineChart (tendência)
- [ ] 11.4 Implementar tooltips interativos
- [ ] 11.5 Implementar responsividade (scroll horizontal em mobile)
- [ ] 11.6 Integrar com recharts ou nivo

## 12. Frontend - Actions & Activity

- [ ] 12.1 Criar componente ActionButtons (Novo Atendimento, Novo Cliente, Nova Cobrança)
- [ ] 12.2 Criar componente ActivityFeed (lista de atividades)
- [ ] 12.3 Criar componente PendingTasks (lista de tarefas)
- [ ] 12.4 Implementar navegação nos botões de ação
- [ ] 12.5 Implementar navegação em atividades e tarefas
- [ ] 12.6 Implementar badge de contagem de tarefas pendentes

## 13. Testes

- [ ] 13.1 Testes unitários de DashboardService (agregações)
- [ ] 13.2 Testes unitários de cálculo de KPIs
- [ ] 13.3 Testes de integração de GET /api/v1/dashboard
- [ ] 13.4 Testes de integração de cache Redis
- [ ] 13.5 Testes de integração de invalidação de cache
- [ ] 13.6 Testes de integração de falha parcial (module unavailable)
- [ ] 13.7 Testes de IDOR (across-tenant access deve retornar dados vazios)
- [ ] 13.8 Testes E2E de dashboard completo (Playwright)
- [ ] 13.9 Testes E2E de cache (Playwright)

## 14. Lint e Qualidade

- [ ] 14.1 Rodar dotnet format no backend
- [ ] 14.2 Rodar ESLint no frontend (npm run lint)
- [ ] 14.3 Verificar cobertura mínima 80% backend, 70% frontend
- [ ] 14.4 Verificar acessibilidade (WCAG 2.1 AA) no dashboard
- [ ] 14.5 Verificar responsividade (mobile/tablet/desktop)
- [ ] 14.6 Verificar performance (< 200ms warm, < 500ms cold)
