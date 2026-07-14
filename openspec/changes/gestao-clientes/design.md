## Context

O Bounded Context CRM da Severina AI precisa de um sistema completo de gestão de clientes. O WhatsApp Business não mantém histórico de clientes, então precisamos de uma fonte única de verdade com busca avançada, importação em lote e visão 360° do cliente. Multi-tenant isolation via company_id é obrigatório.

## Goals / Non-Goals

**Goals:**
- CRUD completo de Client (nome, email, telefone, empresa, tags, notas)
- Busca full-text por nome, email, empresa com paginação
- Importação CSV/XLSX com validação (até 1000 registros)
- Visualização de detalhe do cliente com histórico de interações
- Interface conforme protótipo Stitch (clientes.html)
- Performance: < 200ms para 95% das requisições de leitura

**Non-Goals:**
- Integração direta com WhatsApp Business API (futuro)
- Sincronização automática de contatos
- Exportação de dados (futuro)
- Análise de sentimento de interações

## Decisions

### Decisão 1: Entidade Client com Domínio Rico

**Escolha**: Client com comportamento encapsulado (AddTag, RemoveTag, AddNote, ChangeContactInfo).

**Alternativas consideradas:**
- Model anêmico com service externo — rejeitado por não seguir DDD
- Documento MongoDB — rejeitado por stack ser PostgreSQL

**Justificativa**: Clean Architecture exige que Domain contenha regras de negócio. Tags e notas são Value Objects imutáveis.

### Decisão 2: Busca Full-Text com tsvector no PostgreSQL

**Escolha**: Usar tsvector + GIN index para busca full-text nativa no PostgreSQL.

**Alternativas consideradas:**
- ILIKE simples — rejeitado por performance com muitos registros
- Elasticsearch — rejeitado por complexidade operacional para MVP
- pg_trgm com LIKE — rejeitado por não suportar ranking de relevância

**Justificativa**: PostgreSQL nativo é suficiente para volumes iniciais. tsvector com GIN index oferece performance adequada (< 50ms para 10k registros).

### Decisão 3: Importação com Background Job

**Escolha**: Upload de arquivo → validação síncrona (100 primeiros) → processamento assíncrono via MediatR (Rest of the Job).

**Alternativas consideradas:**
- Processamento síncrono completo — rejeitado por timeout com 1000 registros
- RabbitMQ background job — rejeitado por ser overkill para MVP
- Hangfire — rejeitado por dependência adicional

**Justificativa**: MediatR com notification handler é suficiente para MVP. Futuro pode migrar para Hangfire se necessário.

### Decisão 4: Detalhe do Cliente com Lazy Loading de Histórico

**Escolha**: Endpoint GET /api/v1/clients/{id} retorna dados básicos. Histórico carregado sob demanda via GET /api/v1/clients/{id}/interactions.

**Alternativas consideradas:**
- Eager loading de tudo — rejeitado por performance (histórico pode ser grande)
- GraphQL — rejeitado por não estar na stack

**Justificativa**: Lazy loading evita over-fetching. Histórico de interações pode crescer indefinidamente.

### Decisão 5: Tags como Value Objects

**Escolha**: Tags armazenadas como array de strings no JSONB column do PostgreSQL.

**Alternativas consideradas:**
- Tabela separada ClientTag — rejeitado por complexidade desnecessária
- Array de strings simples — rejeitado por não permitir metadata (cor, descrição)

**Justificativa**: JSONB oferece flexibilidade de schema com query capability. Tags são leves e não precisam de referência cruzada.

## Risks / Trade-offs

- **[Risco] Busca full-text com muitos dados pode degradar** → Mitigação: GIN index + monitoramento de query performance
- **[Risco] Importação de 1000 registros pode consumir muita memória** → Mitigação: Stream com CsvHelper + limite de 1000 linhas
- **[Risco] Histórico de interações pode crescer indefinidamente** → Mitigação: Paginação + TTL futuro para interações antigas
- **[Trade-off] JSONB para tags** → Aceitável para MVP; migrar para tabela normalizada se precisar de queries complexas
- **[Trade-off] Lazy loading de histórico** → Requer chamada adicional; aceitável para UX
