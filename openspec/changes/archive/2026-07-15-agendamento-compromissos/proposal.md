## Summary

Implementar sistema de agendamento de compromissos com suporte a repetição, notificações push, e visualização em grade semanal/mensal integrada ao calendar.

## Problem Statement

Não há sistema de follow-up para compromissos agendados. O WhatsApp Business API não mantém histórico de tarefas pendentes, causando perda de follow-ups importantes.

## Solution Approach

### Backend
- CRUD de Appointment (título, data_hora, cliente_id, tipo, status)
- Suporte a repetição: none, daily, weekly, monthly, custom
- Endpoints: GET/POST/PUT/DELETE /api/v1/appointments

### Frontend
- Grade semanal/mensal conforme protótipo Stitch (agenda.html)
- Modal de criação/edição de compromisso
- Visualização de ocupação (slots disponíveis vs ocupados)
- Filtros por tipo, status, cliente

### Integrações
- Push notification via WebSocket (novo serviço)
- Google Calendar API (futuro)

## Impact

- **Arquivos novos**: Appointment entity, controller, service, migration, pages
- **Dependencies**: WebSocket server (novo serviço)
- **Risco**: Recorrência pode gerar muitos registros - usar lazy generation

## Stitch References
- `stitch-prototypes/agenda.html` - Grade semanal, visualização de ocupação
