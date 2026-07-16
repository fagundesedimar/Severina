## Context

A Severina AI precisa de um sistema de agendamento de compromissos para gerenciar follow-ups e reuniões. O WhatsApp Business não mantém histórico de tarefas pendentes, causando perda de follow-ups importantes. O Bounded Context Conversations será responsável pelo agendamento, com suporte a repetição e notificações.

## Goals / Non-Goals

**Goals:**
- CRUD completo de Appointment (título, data_hora, cliente_id, tipo, status)
- Suporte a repetição: none, daily, weekly, monthly, custom
- Notificações push via WebSocket
- Visualização em grade semanal/mensal conforme protótipo Stitch (agenda.html)
- Performance: < 200ms para 95% das requisições de leitura

**Non-Goals:**
- Integração com Google Calendar (futuro)
- Sincronização com Outlook Calendar
- Lembretes via SMS ou email (usar WebSocket)
- Recorrência complexa (ex: "todo terceiro dia útil")

## Decisions

### Decisão 1: Entidade Appointment com Domínio Rico

**Escolha**: Appointment com comportamento encapsulado (Confirm, Cancel, Reschedule, Complete).

**Alternativas consideradas:**
- Model anêmico — rejeitado por não seguir DDD
- Event Sourcing — rejeitado por complexidade desnecessária para MVP

**Justificativa**: Clean Architecture exige que Domain contenha regras de negócio. Ex: não pode confirmar appointment cancelado.

### Decisão 2: Reporrência via Lazy Generation

**Escolha**: Armazenar padrão de repetição (RecurrenceRule) e gerar instâncias sob demanda.

**Alternativas consideradas:**
- Materializar todas as instâncias na criação — rejeitado por consumo de espaço
- Gerar apenas a próxima ocorrência — rejeitado por dificultar visualização mensal
- Usar biblioteca.repeat.js — rejeitado por dependência adicional

**Justificativa**: Lazy generation com cache em Redis permite visualizar grade semanal/mesual sem overhead. Cada instância é gerada via algoritmo RRULE simplificado.

### Decisão 3: Notificações via WebSocket

**Escolha**: WebSocket server dedicado para push notifications em tempo real.

**Alternativas consideradas:**
- Server-Sent Events (SSE) — rejeitado por não suportar bidirecional
- Polling HTTP — rejeitado por overhead de rede
- Firebase Cloud Messaging — rejeitado por dependência externa

**Justificativa**: WebSocket é padrão para push notifications. Futuro pode integrar com FCM para mobile.

### Decisão 4: Grade com Virtualização

**Escolha**: Virtualizar grade semanal/mensal para performance com muitos appointments.

**Alternativas consideradas:**
- Renderizar todos os appointments — rejeitado por performance
- Paginação por semana — rejeitado por UX ruim

**Justificativa**: Virtualização (react-window) permite renderizar apenas appointments visíveis. Performace adequate para 100+ appointments por dia.

### Decisão 5: Conflitos de Horário

**Escolha**: Validar conflitos de horário na criação/edição de appointments.

**Alternativas consideradas:**
- Permitir sobreposição — rejeitado por UX ruim
- Bloquear automaticamente — rejeitado por poder ser intencional

**Justificativa**: Sistema deve alertar sobre conflitos mas permitir ao usuário decidir. Mostrar appointments sobrepostos em vermelho na grade.

## Risks / Trade-offs

- **[Risco] WebSocket pode não escalar horizontalmente** → Mitigação: Redis backplane para broadcast entre instâncias
- **[Risco] Lazy generation pode causar inconsistências** → Mitigação: Cache com TTL curto (5min) e invalidação na edição
- **[Risco] Conflitos de horário podem ser confusos** → Mitigação: UI clara com sobreposição visual em vermelho
- **[Trade-off] Sem integração com Google Calendar** → Aceitável para MVP; migrar se clientes solicitarem
- **[Trade-off] WebSocket sem fallback** → Aceitável para web; mobile pode precisar de FCM futuro
