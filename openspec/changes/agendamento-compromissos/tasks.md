## 1. Domain Layer - Appointment

- [x] 1.1 Criar entidade Appointment (id, CompanyId, ClientId, Titulo, Descricao, DataHoraInicio, DataHoraFim, Tipo, Status, RecurrenceRule, CreatedAt, UpdatedAt, DeletedAt)
- [x] 1.2 Criar enum AppointmentType (Reuniao, FollowUp, Lembrete, Outro)
- [x] 1.3 Criar enum AppointmentStatus (Scheduled, Confirmed, Completed, Cancelled)
- [x] 1.4 Criar Value Object RecurrenceRule (Type, Interval, DaysOfWeek, EndType, EndDate, EndCount)
- [x] 1.5 Criar enum RecurrenceType (None, Daily, Weekly, Monthly, Custom)
- [x] 1.6 Implementar comportamentos: Appointment.Confirm(), Appointment.Cancel(), Appointment.Complete(), Appointment.Reschedule()
- [x] 1.7 Criar Domain Events: AppointmentCreatedEvent, AppointmentStatusChangedEvent, AppointmentCancelledEvent

## 2. Domain Layer - Recurrence

- [x] 2.1 Criar Value Object RecurrenceGenerator para gerar instâncias
- [x] 2.2 Implementar geração de instâncias daily (todos os dias)
- [x] 2.3 Implementar geração de instâncias weekly (dias específicos da semana)
- [x] 2.4 Implementar geração de instâncias monthly (dia específico do mês)
- [x] 2.5 Implementar geração de instâncias custom (intervalo + unidade)
- [x] 2.6 Implementar validação de conflitos para instâncias geradas

## 3. Infrastructure - EF Core

- [x] 3.1 Criar AppointmentConfiguration com Global Query Filter para CompanyId
- [x] 3.2 Criar index em DataHoraInicio + CompanyId para performance
- [x] 3.3 Criar index em ClientId para joins com cliente
- [x] 3.4 Criar migration para Appointments
- [x] 3.5 Criar IAppointmentRepository com CRUD + busca por data range
- [x] 3.6 Implementar paginação de resultados

## 4. Infrastructure - Redis Cache

- [x] 4.1 Criar IAppointmentCacheService interface
- [x] 4.2 Criar AppointmentCacheService com GetInstances, SetInstances, InvalidateSeries
- [x] 4.3 Implementar TTL de 5 minutos para instâncias cacheadas
- [x] 4.4 Implementar invalidação ao editar/cancelar instância

## 5. Application - Appointment CRUD

- [x] 5.1 Criar DTOs: AppointmentResponse, CreateAppointmentRequest, UpdateAppointmentRequest
- [x] 5.2 Criar CreateAppointmentCommand + Handler com validação (FluentValidation)
- [x] 5.3 Criar ListAppointmentsQuery + Handler com filtros (data range, cliente, status)
- [x] 5.4 Criar GetAppointmentByIdQuery + Handler
- [x] 5.5 Criar UpdateAppointmentCommand + Handler
- [x] 5.6 Criar CancelAppointmentCommand + Handler
- [x] 5.7 Criar CompleteAppointmentCommand + Handler
- [x] 5.8 Criar DeleteAppointmentCommand + Handler (soft delete)

## 6. Application - Recurrence

- [x] 6.1 Criar CreateRecurringAppointmentCommand + Handler
- [x] 6.2 Criar GetRecurrenceInstancesQuery + Handler (lazy generation)
- [x] 6.3 Criar EditSingleInstanceCommand + Handler (exceção de instância)
- [x] 6.4 Criar EditSeriesCommand + Handler (editar todas as instâncias)
- [x] 6.5 Criar CancelSingleInstanceCommand + Handler
- [x] 6.6 Criar CancelSeriesCommand + Handler

## 7. Application - Notifications

- [x] 7.1 Criar INotificationService interface para WebSocket
- [x] 7.2 Criar WebSocketNotificationService com SendToCompany, SendToUser
- [x] 7.3 Criar AppointmentReminderHandler (MediatR) que dispara 15min antes
- [x] 7.4 Criar AppointmentStatusChangedHandler que notifica mudança de status
- [x] 7.5 ImplementarBACKGROUND job para verificar appointments próximos (a cada minuto)

## 8. API Layer

- [x] 8.1 Criar AppointmentsController com endpoints CRUD
- [x] 8.2 Criar RecurrenceController com endpoints de recorrência
- [x] 8.3 Criar WebSocket endpoint para notificações
- [x] 8.4 Configurar middleware de autenticação WebSocket
- [x] 8.5 Configurar Rate Limiting nos endpoints de appointment

## 9. Frontend - Schedule View

- [x] 9.1 Criar página /agenda conforme protótipo Stitch (agenda.html)
- [x] 9.2 Criar componente WeekView com time slots de 08:00-20:00
- [x] 9.3 Criar componente MonthView com calendário
- [x] 9.4 Criar componente AppointmentCard com titulo, cliente, tipo, status
- [x] 9.5 Criar indicador de hora atual (linha vermelha)
- [x] 9.6 Criar botões de navegação (próxima/anterior semana, hoje)
- [x] 9.7 Criar date picker para navegação rápida

## 10. Frontend - Appointment Form

- [x] 10.1 Criar modal de criação/edição de appointment
- [x] 10.2 Criar formulário com campos: titulo, data/hora início/fim, cliente (select), tipo, descrição
- [x] 10.3 Criar seletor de recorrência (nenhum, diário, semanal, mensal, custom)
- [x] 10.4 Criar picker de dias da semana para recorrência semanal
- [x] 10.5 Criar picker de dia do mês para recorrência mensal
- [x] 10.6 Criar configuração de término (data ou contagem)
- [x] 10.7 Integrar com endpoints de appointment

## 11. Frontend - Notifications

- [x] 11.1 Criar WebSocket hook para receber notificações
- [x] 11.2 Criar componente Toast para notificações push
- [x] 11.3 Criar componente NotificationBell com badge
- [x] 11.4 Criar dropdown de notificações recentes
- [x] 11.5 Integrar com WebSocket endpoint

## 12. Testes

- [x] 12.1 Testes unitários de Appointment (confirm, cancel, complete, reschedule)
- [x] 12.2 Testes unitários de RecurrenceRule (geração de instâncias)
- [x] 12.3 Testes de integração de Appointment CRUD (EF Core + PostgreSQL)
- [x] 12.4 Testes de integração de recorrência (lazy generation + cache)
- [x] 12.5 Testes de integração de WebSocket (conexão + notificações)
- [x] 12.6 Testes de IDOR (across-tenant access deve retornar 404)
- [x] 12.7 Testes E2E de criação de appointment (Playwright)
- [x] 12.8 Testes E2E de appointment recorrente (Playwright)
- [x] 12.9 Testes E2E de notificação (Playwright)

## 13. Lint e Qualidade

- [x] 13.1 Rodar dotnet format no backend
- [x] 13.2 Rodar ESLint no frontend (npm run lint)
- [x] 13.3 Verificar cobertura mínima 80% backend, 70% frontend
- [x] 13.4 Verificar acessibilidade (WCAG 2.1 AA) na grade de agenda
- [x] 13.5 Verificar responsividade (mobile/tablet/desktop)
- [x] 13.6 Verificar performance da grade (< 200ms para 95%)
