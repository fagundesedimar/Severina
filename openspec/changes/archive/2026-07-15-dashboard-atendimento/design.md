## Context

A Severina AI precisa de um dashboard principal que consolide métricas de todos os módulos (atendimento, clientes, financeiro, agenda). O WhatsApp Business não oferece métricas consolidadas, e pequenas empresas precisam de uma visão 360° do negócio. O Bounded Context Analytics será responsável pelas agregações e KPIs, com cache de alta performance.

## Goals / Non-Goals

**Goals:**
- Dashboard principal com KPIs de atendimento, produtividade e financeiro
- Gráficos de barras (atendimentos/dia), pizza (origens), line (tendência)
- Acesso rápido: novo atendimento, novo cliente, nova cobrança
- Cache Redis de alta performance (TTL 5min)
- Performance: < 200ms para 95% das requisições de leitura
- Responsividade: mobile ≤640px, tablet 641-1024px, desktop 1025-1440px

**Non-Goals:**
- Relatórios PDF exportáveis (futuro)
- Análise preditiva com IA
- Comparativo entre períodos
- Dashboard personalizável por usuário

## Decisions

### Decisão 1: Agregação via Queries Materializadas

**Escolha**: Criar queries de agregação que rodam sob demanda com cache Redis.

**Alternativas consideradas:**
- Materialized View no PostgreSQL — rejeitado por complexidade de refresh
- Tabela de resumo atualizada via trigger — rejeitado por overhead de escrita
- Event Sourcing com projeções — rejeitado por complexidade desnecessária

**Justificativa**: Queries agregadas com cache de 5 minutos são suficientes para MVP. Performance adequada para até 100k transações.

### Decisão 2: Dashboard como BFF (Backend for Frontend)

**Escolha**: Criar endpoint dedicado GET /api/v1/dashboard que agrega dados de múltiplos módulos.

**Alternativas consideradas:**
- Frontend fazer múltiplas chamadas — rejeitado por waterfall de requests
- GraphQL gateway — rejeitado por não estar na stack
- Micro-frontend com composição — rejeitado por complexidade

**Justificativa**: BFF reduz round-trips e permite cache centralizado. Frontend faz uma chamada única.

### Decisão 3: KPIs Calculados no Backend

**Escolha**: Backend calcula todos os KPIs e retorna DTO enxuto.

**Alternativas consideradas:**
- Frontend calcula KPIs — rejeitado por lógica de negócio no frontend
- WebSocket para atualização em tempo real — rejeitado por complexidade

**Justificativa**: KPIs no backend garantem consistência. Frontend apenas renderiza.

### Decisão 4: Cache com Invalidação Inteligente

**Escolha**: Invalidar cache do dashboard quando qualquer módulo criar/editar/deletar dados.

**Alternativas consideradas:**
- Cache sem invalidação (TTL fixo) — rejeitado por dados ficarem stale
- Invalidação via RabbitMQ — rejeitado por complexidade adicional

**Justificativa**: Invalidação síncrona no Command Handler é simples e eficaz. Cache de 5 min é aceitável.

### Decisão 5: Acesso Rápido via Actions

**Escolha**: Dashboard mostra botões de ação rápida (novo atendimento, novo cliente, nova cobrança).

**Alternativas consideradas:**
- Links no menu lateral — rejeitado por não ser proeminente
- Floating action button — rejeitado por conflitar com outros FABs

**Justificativa**: Actions no dashboard reduzem cliques para tarefas comuns.

## Risks / Trade-offs

- **[Risco] Queries de agregação podem ficar lentas** → Mitigação: Cache Redis + índices adequados + monitoramento
- **[Risco] Dashboard pode ficar stale** → Mitigação: TTL 5min + invalidação em mutations + timestamp de última atualização
- **[Risco] Muitos módulos para aggregar** → Mitigação: Lazy loading de seções + skeleton loading
- **[Trade-off] Sem tempo real** → Aceitável para MVP; WebSocket pode ser adicionado futuro
- **[Trade-off] BFF acopla frontend e backend** → Aceitável para MVP; pode ser substituído por GraphQL futuro
