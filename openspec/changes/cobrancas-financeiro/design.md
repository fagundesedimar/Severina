## Context

A Severina AI precisa de um módulo financeiro para controle de cobranças, receitas e despesas. O WhatsApp Business não oferece controle financeiro, e pequenas empresas de serviços precisam de uma visão consolidada do fluxo de caixa. O Bounded Context Billing será responsável pelo ciclo de vida financeiro, com dashboard e KPIs integrados.

## Goals / Non-Goals

**Goals:**
- CRUD completo de Transaction (receita/despesa) com categorização
- CRUD de Invoice/Charge (cobranças) com status e vencimento
- Dashboard financeiro com KPIs (saldo, receitas, despesas, previsão)
- Gráficos de barras (receitas/despesas por mês) e pizza (por categoria)
- Exportação CSV/XLSX de transações
- Performance: < 200ms para 95% das requisições de leitura

**Non-Goals:**
- Integração com Mercado Pago (futuro)
- Integração com bancos (Open Banking)
- Faturamento automático (NF-e)
- Controle de estoque

## Decisions

### Decisão 1: Entidade Transaction com Domínio Rico

**Escolha**: Transaction com comportamento encapsulado (Approve, Reject, categorize).

**Alternativas consideradas:**
- Model anêmico — rejeitado por não seguir DDD
- Event Sourcing — rejeitado por complexidade desnecessária para MVP

**Justificativa**: Clean Architecture exige que Domain contenha regras de negócio. Ex: não pode aprovar transação rejeitada.

### Decisão 2: Valores em decimal (não float)

**Escolha**: Armazenar valores financeiros como decimal(18,2) no PostgreSQL.

**Alternativas consideradas:**
- float/double — rejeitado por precisão
- integer (centavos) — rejeitado por complexidade de conversão
- BigInt — rejeitado por não suportar centavos

**Justificativa**: decimal(18,2) é padrão financeiro. .NET decimal mapeia diretamente para PostgreSQL decimal.

### Decisão 3: Dashboard com Cache Redis

**Escolha**: Cache de KPIs e gráficos em Redis com TTL de 5 minutos.

**Alternativas consideradas:**
- Query em tempo real — rejeitado por performance (agregações são lentas)
- Materialized View — rejeitado por complexidade de refresh
- Tabela de resumo — rejeitado por não ser real-time

**Justificativa**: Cache de 5 minutos é aceitável para dashboard financeiro. KPIs não precisam ser real-time.

### Decisão 4: Exportação via Background Job

**Escolha**: Exportação assíncrona com download posterior.

**Alternativas consideradas:**
- Síncrono com streaming — rejeitado por timeout com muitos registros
- Geração de arquivo único — rejeitado por consumo de memória

**Justificativa**: Background job permite exportar milhões de linhas sem timeout. Usuário recebe notificação quando arquivo está pronto.

### Decisão 5: Categorias como Value Objects

**Escolha**: Categorias armazenadas como enum + tabela de referência para nomes customizados.

**Alternativas consideradas:**
- JSONB livre — rejeitado por dificultar queries
- Tabela separada Category — rejeitado por complexidade desnecessária

**Justificativa**: Enum garante tipos válidos. Tabela de referência permite nomes customizados sem alterar schema.

## Risks / Trade-offs

- **[Risco] Dashboard pode ficar lento com muitas transações** → Mitigação: Cache Redis + paginação de gráficos
- **[Risco] Exportação de muitos dados pode consumir memória** → Mitigação: Streaming comStreamWriter + limite de 100k linhas
- **[Risco] Precisão decimal pode variar entre bancos** → Mitigação: Usar decimal(18,2) explicitamente no EF Core
- **[Trade-off] Sem integração com Mercado Pago** → Aceitável para MVP; webhook pode ser adicionado futuro
- **[Trade-off] Dashboard com cache de 5min** → Aceitável; dados financeiros não precisam ser real-time
