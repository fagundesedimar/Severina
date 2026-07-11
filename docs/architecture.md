# Arquitetura de Software

## Contexto Arquitetural

### Objetivo

Este documento define a arquitetura de software do produto **Severina AI**, estabelecendo diretrizes técnicas, restrições arquiteturais e requisitos não funcionais para implementação por equipes humanas e agentes de inteligência artificial.

### Escopo

A arquitetura contempla:

* Frontend
* Backend
* Banco de Dados
* Infraestrutura
* Segurança
* Observabilidade
* Integrações Externas

### Arquitetura de Referência

* Estilo arquitetural: Microservices orientado a eventos com Clean Architecture
* Comunicação: REST/HTTP para APIs públicas, AMQP para mensageria interna
* Infraestrutura: SaaS multi-tenant em nuvem pública
* Observabilidade: OpenTelemetry com logs estruturados, métricas e tracing distribuído
* Segurança: identidade baseada em JWT, MFA opcional e RBAC

### Stack Tecnológica

#### Frontend

* Linguagem: TypeScript
* Framework: React + Next.js
* Roteamento: Next.js App Router
* Estilização: Tailwind CSS + Shadcn
* Bibliotecas adicionais: React Query, Zustand, Heroicons

#### Backend

* Linguagem: C#
* Runtime: .NET 8
* Framework: ASP.NET Core
* ORM/ODM: Entity Framework Core

#### Banco de Dados

* SGBD: PostgreSQL
* Versão mínima: 15

#### Observabilidade

* Plataforma: OpenTelemetry com backend compatível (Prometheus, Grafana, Jaeger)

#### Identidade

* Provedor: Identity Service customizado com JWT / OIDC compatível

#### Desenvolvimento

* Ferramentas: Visual Studio Code, dotnet CLI, Node.js, Docker

#### DevOps

* CI/CD: GitHub Actions ou Azure DevOps
* Infraestrutura como código: Terraform

### Estrutura do Repositório

```text
src/
  frontend/
    components/
    pages/
    layouts/
    hooks/
    services/
    styles/
  backend/
    BuildingBlocks/
    Identity/
    Companies/
    CRM/
    Conversations/
    AI/
    Notifications/
    Billing/
    Analytics/
    Integrations/
    Gateway/
    SharedKernel/
infra/
  terraform/
  k8s/
docs/
  problem.md
  prd.md
  specReq.md
  architecture.md
```

---

## Adequação Funcional

### Fonte Única de Verdade

* A camada responsável pelas regras de negócio é o Backend, organizada em serviços de domínio do tipo Companies, CRM, Conversations, Billing e Analytics.
* A fonte única de verdade é o banco de dados PostgreSQL junto com os microserviços de domínio que encapsulam o modelo e validações.

### Política de Comunicação entre Camadas

Todas as operações de negócio devem ocorrer através de:

* API Gateway / Backend API

É proibido:

* acesso direto a camadas internas
* acesso direto ao banco de dados a partir do frontend
* acesso direto a serviços administrativos sem autenticação e autorização adequadas

### APIs e Versionamento

Base URL:

```text
https://api.severina.ai/v1
```

Estratégia de versionamento:

```text
/v1/{recurso}
```

### Endpoints Públicos

* /auth/login
* /auth/refresh
* /health

### Endpoints Protegidos

* /companies
* /appointments
* /invoices
* /analytics/reports

### Contrato de API

* APIs devem ser versionadas
* APIs devem possuir documentação OpenAPI
* Payloads devem utilizar formato JSON
* Paginação para coleções
* Filtros e ordenação quando aplicáveis
* Backend como fonte única de verdade
* Regras de negócio não devem existir no frontend

### Estratégia de Tenancy

#### MVP

* Multi-tenant lógico com `company_id` em cada registro e validação de escopo em backend

#### Evolução Futura

* Multi-tenant avançado com isolamento por schema ou banco e customização por cliente

---

## Eficiência de Desempenho

### Comunicação entre Componentes

* Protocolo: HTTPS para APIs públicas e AMQP para mensageria interna
* Formato: JSON para APIs, Protobuf para mensagens internas quando aplicável
* Requisitos de segurança: TLS 1.2+ em todas as comunicações, autenticação JWT para APIs

### Rate Limiting

* Usuário anônimo: 50 requisições por minuto
* Usuário autenticado: 200 requisições por minuto
* Por tenant (empresa): 1000 requisições por minuto
* Endpoints de IA: 30 requisições por minuto por empresa (controle de custo)
* Endpoints de autenticação: 10 tentativas por minuto por IP
* Estratégia de implementação: middleware no API Gateway com armazenamento em Redis
* Resposta HTTP `429 Too Many Requests` com header `Retry-After`

### Transações e Persistência

* Estratégia transacional: uso de transações de banco para operações críticas de cada serviço e padrões de saga para fluxos distribuídos

### Estratégias Futuras de Escalabilidade

* Escala horizontal de microserviços
* Réplicas de leitura no PostgreSQL
* Cache Redis para consultas frequentes e sessões

---

## Mensageria e Eventos

### Broker de Mensagens

* Tecnologia: RabbitMQ
* Protocolo: AMQP 0-9-1
* Formato de mensagem: JSON (padrão), Protobuf (para mensagens internas de alto throughput)

### Padrões de Comunicação Assíncrona

* **Event Publishing:** serviços publicam eventos após mudança de estado (ex: `InvoiceCreated`, `AppointmentConfirmed`)
* **Command Queue:** comandos assíncronos para processamento de longa duração (ex: envio de WhatsApp, geração de relatórios)
* **Fanout:** notificações que devem ser entregues a múltiplos consumidores (ex: dashboard em tempo real)

### Filas Definidas

| Fila | Consumidor | Uso |
| :--- | :--- | :--- |
| `notifications.whatsapp` | Worker de Notificações | Envio de mensagens WhatsApp (lembretes, follow-up, cobrança) |
| `notifications.push` | Worker de Notificações | Notificações push para o frontend |
| `billing.process` | Worker Financeiro | Processamento assíncrono de cobranças e reconciliação |
| `analytics.events` | Worker de Analytics | Coleta e agregação de eventos para dashboards |
| `ai.process` | Worker de IA | Processamento de consultas RAG e geração de respostas |

### Retry e Dead Letter Queue

* **Retry:** tentativa automática com backoff exponencial (3 tentativas, intervalo inicial de 5s)
* **DLQ:** mensagens que falharam após todas as tentativas são encaminhadas para fila morta para inspeção manual
* **TTL:** mensagens com tempo de vida superior a 24h são descartadas automaticamente

---

## Arquitetura de IA (RAG)

### Visão Geral

O serviço de IA do Severina AI utiliza um padrão **RAG (Retrieval-Augmented Generation)** para gerar respostas contextualizadas aos clientes, combinando recuperação semântica de documentos com modelos de linguagem.

### Componentes

* **Embedding Service:** converte textos em vetores numéricos para busca semântica
* **Vector Store:** banco vetorial baseado em pgvector, armazenado no mesmo PostgreSQL do sistema
* **LLM Provider:** provedor de modelo de linguagem para geração de respostas (multi-provider com fallback)
* **RAG Pipeline:** orquestra recuperação de contexto, montagem de prompt e geração de resposta

### Fluxo de Processamento

```text
Mensagem do cliente → Embedding da query → Busca vetorial (pgvector) → Montagem de contexto → Geração via LLM → Resposta
```

### Estratégia de Embedding

* Modelo padrão de embedding para buscas semânticas
* Vetores armazenados com indexação HNSW para performance em busca por similaridade
* Atualização de embeddings quando documentos de contexto são modificados

### Gerenciamento de Contexto por Empresa

* Cada empresa possui seu próprio namespace de embeddings (`company_id`)
* Documentos de contexto (FAQ, políticas, catálogo de serviços) são indexados por empresa
* Isolamento garantido: busca vetorial restrita ao `company_id` do tenant

### Fallback e Resiliência

* Se o LLM principal estiver indisponível, fallback automático para provedor secundário
* Se todos os provedores falharem, resposta padrão com indicação de contato manual
* Rate limiting por empresa para controlar custo de chamadas à IA

### Segurança

* Prompt injection detectado e sanitizado no pipeline
* Dados de contexto não expostos entre tenants
* Logs de interações com IA auditáveis para conformidade

---

## Compatibilidade

### Integração

* Padrão de integração: APIs REST, webhooks e mensageria RabbitMQ

### Formatos de Comunicação

* JSON
* Protobuf (para comunicação interna quando aplicável)

### Versionamento

* Estratégia de versionamento: SemVer para serviços e versionamento no caminho da API

### CORS

* Política CORS: permitir domínios autorizados do frontend e bloquear origens não confiáveis

### Portabilidade

* Regras de portabilidade: executar localmente via Docker e em produção via Kubernetes/Terraform

---

## Usabilidade

### Diretrizes Frontend

* Interfaces simples e intuitivas
* Navegação orientada a tarefas
* Componentes reutilizáveis e consistentes

### Experiência de Autenticação

* Estratégia de autenticação: login por e-mail e senha, refresh token e MFA opcional para contas sensíveis

### Consistência de Interfaces

Permissões para Server Actions, Controllers ou mecanismos equivalentes:

* acessar dados via backend autorizado
* validar input no backend
* reutilizar componentes UI aprovados

Restrições:

* não permitir lógica de negócio complexa no frontend
* não expor segredos ou chaves no cliente
* não usar componentes externos sem revisão de segurança

---

## Confiabilidade

### Tratamento de Erros

Padrão adotado:

* Resposta estruturada com type, title, status, detail e instance

Exemplo:

```json
{
  "type": "https://api.severina.ai/errors/validation",
  "title": "Validation Error",
  "status": 400,
  "detail": "O campo email é obrigatório.",
  "instance": "/companies/123"
}
```

### Auditoria

Operações auditadas:

* CREATE
* UPDATE
* DELETE

Campos obrigatórios:

* user_id
* timestamp
* action
* entity
* entity_id

### Migrations

* Política de migrations: todas as alterações de schema devem ser aplicadas via migrations versionadas em código

É proibido:

* alterações manuais no banco
* alterações sem migration

### Testes Automatizados

Ferramentas:

* Lint: ESLint e dotnet format
* Unidade: xUnit e Jest
* Integração: xUnit e Playwright API tests
* E2E: Playwright

### Cobertura Mínima

Backend:

* Linhas: 80%
* Branches: 70%

Frontend:

* Linhas: 70%
* Branches: 60%

### Critérios de Teste

Toda alteração de regra de negócio deve cobrir:

* Happy Path
* Sad Path
* Edge Cases

---

## Segurança

### Princípios Gerais

* Privacidade por design
* Menor privilégio
* Defesa em profundidade

### Gestão de Identidade

Responsabilidades do provedor:

* LOGIN
* LOGOUT
* RECUPERAÇÃO
* MFA
* SESSÃO

### Autenticação

#### Fluxo

Frontend:

* coletar credenciais e tokens de refresh
* redirecionar para tela de login quando necessário

Backend:

* validar credenciais e emitir JWT
* validar refresh tokens e renovar sessões

Identity Provider:

* armazenar usuários e credenciais
* gerenciar MFA e sessão

Fluxo:

```text
Usuário envia credenciais -> Backend valida -> Backend emite JWT -> Frontend usa JWT em requisições
```

### Autorização

* Modelo de autorização: RBAC com papéis e permissões por recurso
* Estratégia: RBAC com controle de acesso por empresa e função

### Papéis e Permissões

#### Administrador da Empresa

Pode:

* gerenciar usuários, clientes e configurações da empresa
* visualizar relatórios financeiros
* criar cobranças e configurar integrações

#### Usuário Operacional

Pode:

* gerenciar atendimentos, compromissos e cobranças
* consultar histórico de clientes

### Restrições do Frontend

É proibida a utilização de:

* SDKs de terceiros não auditados
* componentes que armazenem chaves secretas no cliente

### Proteção Contra Ameaças

#### Transporte

* TLS 1.2+ obrigatório

#### Headers

* Content-Security-Policy
* X-Content-Type-Options
* X-Frame-Options

#### Injeção

* Política anti-injection: validação e sanitização de entradas no backend

#### Controle de Acesso

* Política IDOR: verificar `company_id` e permissões em todas as requisições

### Segurança de Dados

* Regras de acesso a dados baseadas em contexto de empresa e papel do usuário

### Segurança de APIs

* Requisitos de autenticação: JWT obrigatório em endpoints protegidos
* Requisitos de autorização: validação de permissões por recurso

---

## Manutenibilidade

### Organização de Código

* Backend organizado por domínio e serviços
* Frontend organizado por componentes, páginas e hooks

### Convenções de Desenvolvimento

* Nomeação consistente em camelCase/PascalCase
* Código documentado e revisado via PR
* Adoção de DDD, Clean Architecture e CQRS no backend

### Padrão CQRS (Command Query Responsibility Segregation)

* **Escopo no MVP:** CQRS simplificado com separação de_Commands_ (escrita) e_Queries_ (leitura) no nível de service layer, sem event store separado.
* **Commands:** representam intenções de mudança de estado (ex: `CreateAppointmentCommand`, `ConfirmPaymentCommand`). Validados por Command Handlers que executam regras de negócio.
* **Queries:** representam consultas otimizadas para leitura (ex: `GetClientAppointmentsQuery`, `GetDashboardMetricsQuery`). Podem usar projeções materializadas para performance.
* **Eventos de Domínio:** publicados após sucesso de um Command para disparar efeitos colaterais (notificações, analytics, sincronização).
* **Evolução Futura:** migração para CQRS completo com Event Store e projeções assíncronas quando o volume de dados justificar.

### Padrão DDD (Domain-Driven Design)

* **Bounded Contexts:** cada microserviço (Companies, CRM, Conversations, Billing, Analytics, Notifications) é um Bounded Context independente.
* **Entidades e Value Objects:** modelados conforme o domínio de cada contexto.
* **Aggregate Roots:** pontos de consistência transacional (ex: `Client` é Aggregate Root para seus compromissos e contatos).
* **Repositórios:** abstraem acesso a dados, implementados por camada de infraestrutura.

### Variáveis de Ambiente

* Estratégia de configuração: variáveis de ambiente para segredos, URLs e flags de recurso

É proibido:

* commit de segredos no repositório
* uso de variáveis hardcoded

### Diretrizes para Agentes de IA

Antes de qualquer alteração:

* Ler documentação arquitetural
* Avaliar impacto técnico
* Avaliar impacto de segurança
* Avaliar impacto de observabilidade
* Avaliar impacto em testes

Ao finalizar:

* Executar lint
* Executar testes
* Atualizar documentação
* Informar artefatos modificados

### Restrições Arquiteturais

* Não misturar regras de negócio entre serviços
* Não acessar banco diretamente fora do backend de domínio
* Não criar dependências cíclicas entre microserviços

---

## Portabilidade

### Containers

* Padrão de container: Docker com imagens multistage e leves

### Banco de Dados

Ambiente local:

* usar PostgreSQL em container Docker

Ambiente de produção:

* usar PostgreSQL gerenciado ou provisionado com réplicas de leitura

### Independência de Fornecedor

* Regras de portabilidade: evitar serviços proprietários não portáveis e preferir padrões abertos

### Infraestrutura

* Estratégia IaC: Terraform para provisionamento de nuvem

### Diagrama de Implantação (MVP)

```text
┌─────────────────────────────────────────────────────┐
│                    Cloud (Azure/AWS)                  │
│                                                       │
│  ┌──────────────┐   ┌──────────────────────────┐     │
│  │  CDN / WAF   │   │    API Gateway / LB      │     │
│  └──────┬───────┘   └──────────┬───────────────┘     │
│         │                      │                      │
│         ▼                      ▼                      │
│  ┌──────────────┐   ┌──────────────────────────┐     │
│  │   Frontend   │   │   Backend (ASP.NET Core) │     │
│  │  (Next.js)   │   │   ┌────────┐ ┌────────┐ │     │
│  │  Container   │   │   │Companies│ │  CRM   │ │     │
│  └──────────────┘   │   └────────┘ └────────┘ │     │
│                      │   ┌────────┐ ┌────────┐ │     │
│                      │   │Billing │ │  AI    │ │     │
│                      │   └────────┘ └────────┘ │     │
│                      │   ┌────────┐ ┌────────┐ │     │
│                      │   │Conserv.│ │Analytics│ │     │
│                      │   └────────┘ └────────┘ │     │
│                      └──────────┬───────────────┘     │
│                                 │                      │
│              ┌──────────────────┼──────────┐          │
│              ▼                  ▼          ▼          │
│  ┌──────────────┐  ┌──────────────┐  ┌──────────┐   │
│  │  PostgreSQL   │  │   RabbitMQ   │  │  Redis   │   │
│  │  (Principal)  │  │  (Mensageria)│  │ (Cache)  │   │
│  └──────────────┘  └──────────────┘  └──────────┘   │
│                                                       │
│  ┌──────────────────────────────────────────────┐    │
│  │        Workers / Background Services          │    │
│  │  (Notificações, Analytics, IA, Billing)       │    │
│  └──────────────────────────────────────────────┘    │
└─────────────────────────────────────────────────────┘
```

### Kubernetes (Evolução)

* Orquestração com Kubernetes para auto-scaling horizontal de microserviços
* Namespaces isolados por ambiente (dev, staging, production)
* Helm Charts para deploy padronizado
* Ingress Controller para roteamento de tráfego externo

---

## Observabilidade

### OpenTelemetry

Instrumentar:

* requisições HTTP
* eventos de mensageria
* comandos e queries

Propagar:

* trace_id
* span_id
* request_id

### Tracing Distribuído

* Estratégia de correlação: usar trace_id em todos os serviços e logs

### Logs Estruturados

É proibido:

```text
console.log()
```

Campos mínimos:

* timestamp
* level
* service
* trace_id
* message

### Métricas

* Estratégia de métricas: coletar latência, erros, throughput e saturação

---

## Backup e Recuperação

### Política de Backup

* **Banco de dados (PostgreSQL):** backup diário completo com retenção de 30 dias; backup incrementale a cada 6 horas
* **Banco vetorial (pgvector):** incluso no backup do PostgreSQL
* **Configurações e segredos:** versionados em repositório seguro com backup em vault dedicado
* **Logs e auditoria:** retenção mínima de 90 dias em armazenamento frio

### RPO e RTO

| Critério | MVP | Produção Madura |
| :--- | :--- | :--- |
| **RPO** (Recovery Point Objective) | 24 horas | 6 horas |
| **RTO** (Recovery Time Objective) | 4 horas | 1 hora |

### Estratégia de Recuperação

* **Restore Point:** restauração a partir de snapshot de backup diário
* **Ponto a ponto:** restauração granular a partir de WAL (Write-Ahead Log) do PostgreSQL
* **Testes de restore:** executados mensalmente em ambiente de staging para validar integridade dos backups

### Disaster Recovery

* **MVP:** backup em região secundária da mesma nuvem, restauração manual
* **Evolução:** replicação ativa-passiva cross-region com failover automático

---

## Evolução Planejada

### Infraestrutura

* Evolução para orquestração Kubernetes e múltiplas zonas
* Adição de cache distribuído e filas gerenciadas
* Suporte a múltiplas regiões

### Armazenamento

* Evolução do PostgreSQL com suporte a pgvector para IA

### Pagamentos

* Evolução para integração com gateways de pagamento e planos de assinatura

### Comunicação

* Evolução para suporte a SMS, e-mail e outros canais além de WhatsApp

### Plataformas

* Evolução para apps móveis e dashboards de white label

### Funcionalidades

* Agente executivo multi-domínio
* Automatização avançada de workflows

---

## Limites de Implementação do MVP

É proibido implementar:

* controles contábeis avançados para grandes empresas
* integração nativa com ERPs corporativos
* suporte multilíngue completo no MVP

Esses elementos pertencem exclusivamente a versões futuras do produto.
