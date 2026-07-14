## 1. Domain Layer - Appointment

- [ ] 1.1 Criar entidade Appointment (id, CompanyId, ClientId, Titulo, Descricao, DataHoraInicio, DataHoraFim, Tipo, Status, RecurrenceRule, CreatedAt, UpdatedAt, DeletedAt)
- [ ] 1.2 Criar enum AppointmentType (Reuniao, FollowUp, Lembrete, Outro)
- [ ] 1.3 Criar enum AppointmentStatus (Scheduled, Confirmed, Completed, Cancelled)
- [ ] 1.4 Criar Value Object RecurrenceRule (Type, Interval, DaysOfWeek, EndType, EndDate, EndCount)
- [ ] 1.5 Criar enum RecurrenceType (None, Daily, Weekly, Monthly, Custom)
- [ ] 1.6 Implementar comportamentos: Appointment.Confirm(), Appointment.Cancel(), Appointment.Complete(), Appointment.Reschedule()
- [ ] 1.7 Criar Domain Events: AppointmentCreatedEvent, AppointmentStatusChangedEvent, AppointmentCancelledEvent

## 2. Domain Layer - Recurrence

- [ ] 2.1 Criar Value Object RecurrenceGenerator para gerar instâncias
- [ ] 2.2 Implementar geração de instâncias daily (todos os dias)
- [ ] 2.3 Implementar geração de instâncias weekly (dias específicos da semana)
- [ ] 2.4 Implementar geração de instâncias monthly (dia específico do mês)
- [ ] 2.5 Implementar geração de instâncias custom (intervalo + unidade)
- [ ] 2.6 Implementar validação de conflitos para instâncias geradas

## 3. Infrastructure - EF Core

- [ ] 3.1 Criar AppointmentConfiguration com Global Query Filter para CompanyId
- [ ] 3.2 Criar index em DataHoraInicio + CompanyId para performance
- [ ] 3.3 Criar index em ClientId para joins com cliente
- [ ] 3.4 Criar migration para Appointments
- [ ] 3.5 Criar IAppointmentRepository com CRUD + busca por data range
- [ ] 3.6 Implementar paginação de resultados

## 4. Infrastructure - Redis Cache

- [ ] 4.1 Criar IAppointmentCacheService interface
- [ ] 4.2 Criar AppointmentCacheService com GetInstances, SetInstances, InvalidateSeries
- [ ] 4.3 Implementar TTL de 5 minutos para instâncias cacheadas
- [ ] 4.4 Implementar invalidação ao editar/cancelar instância

## 5. Application - Appointment CRUD

- [ ] 5.1 Criar DTOs: AppointmentResponse, CreateAppointmentRequest, UpdateAppointmentRequest
- [ ] 5.2 Criar CreateAppointmentCommand + Handler com validação (FluentValidation)
- [ ] 5.3 Criar ListAppointmentsQuery + Handler com filtros (data range, cliente, status)
- [ ] 5.4 Criar GetAppointmentByIdQuery + Handler
- [ ] 5.5 Criar UpdateAppointmentCommand + Handler
- [ ] 5.6 Criar CancelAppointmentCommand + Handler
- [ ] 5.7 Criar CompleteAppointmentCommand + Handler
- [ ] 5.8 Criar DeleteAppointmentCommand + Handler (soft delete)

## 6. Application - Recurrence

- [ ] 6.1 Criar CreateRecurringAppointmentCommand + Handler
- [ ] 6.2 Criar GetRecurrenceInstancesQuery + Handler (lazy generation)
- [ ] 6.3 Criar EditSingleInstanceCommand + Handler (exceção de instância)
- [ ] 6.4 Criar EditSeriesCommand + Handler (editar todas as instâncias)
- [ ] 6.5 Criar CancelSingleInstanceCommand + Handler
- [ ] 6.6 Criar CancelSeriesCommand + Handler

## 7. Application - Notifications

- [ ] 7.1 Criar INotificationService interface para WebSocket
- [ ] 7.2 Criar WebSocketNotificationService com SendToCompany, SendToUser
- [ ] 7.3 Criar AppointmentReminderHandler (MediatR) que dispara 15min antes
- [ ] 7.4 Criar AppointmentStatusChangedHandler que notifica mudança de status
- [ ] 7.5 ImplementarBACKGROUND job para verificar appointments próximos (a cada minuto)

## 8. API Layer

- [ ] 8.1 Criar AppointmentsController com endpoints CRUD
- [ ] 8.2 Criar RecurrenceController com endpoints de recorrência
- [ ] 8.3 Criar WebSocket endpoint para notificações
- [ ] 8.4 Configurar middleware de autenticação WebSocket
- [ ] 8.5 Configurar Rate Limiting nos endpoints de appointment

## 9. Frontend - Schedule View

- [ ] 9.1 Criar página /agenda conforme protótipo Stitch (agenda.html)
- [ ] 9.2 Criar componente WeekView com time slots de 08:00-20:00
- [ ] 9.3 Criar componente MonthView com calendário
- [ ] 9.4 Criar componente AppointmentCard com titulo, cliente, tipo, status
- [ ] 9.5 Criar indicador de hora atual (linha vermelha)
- [ ] 9.6 Criar botões de navegação (próxima/anterior semana, hoje)
- [ ] 9.7 Criar date picker para navegação rápida

## 10. Frontend - Appointment Form

- [ ] 10.1 Criar modal de criação/edição de appointment
- [ ] 10.2 Criar formulário com campos: titulo, data/hora início/fim, cliente (select), tipo, descrição
- [ ] 10.3 Criar seletor de recorrência (nenhum, diário, semanal, mensal, custom)
- [ ] 10.4 Criar picker de dias da semana para recorrência semanal
- [ ] 10.5 Criar picker de dia do mês para recorrência mensal
- [ ] 10.6 Criar configuração de término (data ou contagem)
- [ ] 10.7 Integrar com endpoints de appointment

## 11. Frontend - Notifications

- [ ] 11.1 Criar WebSocket hook para receber notificações
- [ ] 11.2 Criar componente Toast para notificações push
- [ ] 11.3 Criar componente NotificationBell com badge
- [ ] 11.4 Criar dropdown de notificações recentes
- [ ] 11.5 Integrar com WebSocket endpoint

## 12. Testes

- [ ] 12.1 Testes unitários de Appointment (confirm, cancel, complete, reschedule)
- [ ] 12.2 Testes unitários de RecurrenceRule (geração de instâncias)
- [ ] 12.3 Testes de integração de Appointment CRUD (EF Core + PostgreSQL)
- [ ] 12.4 Testes de integração de recorrência (lazy generation + cache)
- [ ] 12.5 Testes de integração de WebSocket (conexão + notificações)
- [ ] 12.6 Testes de IDOR (across-tenant access deve retornar 404)
- [ ] 12.7 Testes E2E de criação de appointment (Playwright)
- [ ] 12.8 Testes E2E de appointment recorrente (Playwright)
- [ ] 12.9 Testes E2E de notificação (Playwright)

## 13. Lint e Qualidade

- [ ] 13.1 Rodar dotnet format no backend
- [ ] 13.2 Rodar ESLint no frontend (npm run lint)
- [ ] 13.3 Verificar cobertura mínima 80% backend, 70% frontend
- [ ] 13.4 Verificar acessibilidade (WCAG 2.1 AA) na grade de agenda
- [ ] 13.5 Verificar responsividade (mobile/tablet/desktop)
- [ ] 13.6 Verificar performance da grade (< 200ms para 95%)
