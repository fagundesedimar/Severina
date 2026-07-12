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
    app/                    # Next.js App Router (pages/layouts)
    components/             # Componentes UI reutilizáveis (incluindo ThemeToggle)
    hooks/                  # Custom hooks (incluindo useTheme)
    services/               # Chamadas API e lógica de acesso a dados (incluindo preferences API)
    stores/                 # Zustand stores (estado global, incluindo useThemeStore)
    styles/                 # Tailwind config, CSS custom properties (temas light/dark), estilos globais
    types/                  # TypeScript type definitions
  backend/
    BuildingBlocks/
      Domain/               # Entidades, Value Objects, Aggregate Roots, Domain Events
      Application/          # Use Cases, Command/Query Handlers, Interfaces de serviço
      Infrastructure/       # Implementações de repositórios, EF Core, mensageria
      API/                  # Controllers, Middlewares, Filters, DTOs
    SharedKernel/           # Utilitários compartilhados, constants, extensions
    Identity/               # Autenticação, autorização, Identity Service
    Companies/              # Bounded Context: gestão de empresas
    CRM/                    # Bounded Context: relacionamento com clientes
    Conversations/          # Bounded Context: atendimento omnichannel
    AI/                     # Bounded Context: serviços de IA / RAG
    Notifications/          # Bounded Context: notificações (WhatsApp, push, email)
    Billing/                # Bounded Context: cobranças e pagamentos
    Analytics/              # Bounded Context: métricas e relatórios
    Integrations/           # Bounded Context: integrações externas (WhatsApp API, etc)
    Gateway/                # API Gateway / BFF (Backend for Frontend)
infra/
  terraform/
  k8s/
docs/
  problem.md
  prd.md
  specReq.md
  architecture.md
```

### Estrutura Interna de Cada Microserviço (Clean Architecture)

Cada Bounded Context segue a estrutura de camadas:

```text
Companies/                        # Exemplo de Bounded Context
  Domain/
    Entities/                     # Entidades de domínio (ex: Company.cs)
    ValueObjects/                 # Value Objects (ex: Cnpj.cs, Plano.cs)
    Events/                       # Domain Events (ex: CompanyCreatedEvent.cs)
    Exceptions/                   # Exceções de domínio (ex: InvalidCompanyException.cs)
    Interfaces/                   # Contratos de repositório (ex: ICompanyRepository.cs)
  Application/
    Commands/                     # Command Handlers (ex: CreateCompanyCommand.cs)
    Queries/                      # Query Handlers (ex: GetCompanyByIdQuery.cs)
    DTOs/                         # Data Transfer Objects (ex: CompanyDto.cs)
    Interfaces/                   # Contratos de serviço (ex: ICompanyService.cs)
    Validators/                   # Validações (ex: CreateCompanyValidator.cs)
    Mappings/                     # Mapeamentos (ex: CompanyProfile.cs - AutoMapper)
  Infrastructure/
    Persistence/                  # EF Core DbContext, Configurations, Migrations
    Repositories/                 # Implementações de repositórios
    Messaging/                    # Producers/Consumers de mensageria
    ExternalServices/             # Clients de APIs externas
  API/
    Controllers/                  # Controllers REST
    Middleware/                    # Middlewares (auth, logging, rate limiting)
    Filters/                      # Action Filters
    Extensions/                   # Extensões de DI e configuração
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
* Suporte a modo claro e escuro com toggle acessível
* Preferência de tema persistida por usuário (localStorage + API)
* Tokens de cor via CSS custom properties para troca dinâmica de tema

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

### Clean Architecture (Arquitetura Limpa)

O backend do Severina AI segue os princípios da Clean Architecture de Robert C. Martin, organizado em camadas concêntricas com a regra de dependência: **dependências apontam sempre para dentro**.

#### Camadas

| Camada | Responsabilidade | Dependências |
| :--- | :--- | :--- |
| **Domain** | Entidades, Value Objects, Aggregate Roots, Domain Events, interfaces de repositório. Contém a regra de negócio pura. | Nenhuma (camada mais interna) |
| **Application** | Use Cases, Command/Query Handlers, orquestração de fluxos, DTOs, validações. Coordena as operações. | Domain |
| **Infrastructure** | Implementações concretas: EF Core, repositórios, mensageria, clientes HTTP, banco vetorial. | Domain, Application |
| **API** | Controllers, middlewares, filters, extensões de DI. Ponto de entrada HTTP. | Application, Infrastructure |

#### Regra de Dependência

```text
API → Application → Domain
         ↓
    Infrastructure → Domain
```

* Domain **nunca** depende de camadas externas
* Application depende apenas de Domain (via interfaces)
* Infrastructure implementa interfaces definidas em Domain
* API depende de Application (via injeção de dependência)

#### Diretrizes por Camada

**Domain:**
* Entidades representam conceitos de negócio com comportamento (não são anemic models)
* Value Objects são imutáveis e sem identidade própria (ex: `Cnpj`, `Dinheiro`, `Periodo`)
* Domain Events notificam mudanças de estado (ex: `CompanyCreated`, `InvoicePaid`)
* Exceções de domínio expressam violações de regra (ex: `SchedulingConflictException`)
* **Proibido:** referências a bibliotecas de infraestrutura (EF Core, HttpClient, etc)

**Application:**
* Command Handlers encapsulam operações de escrita (ex: `CreateCompanyHandler`)
* Query Handlers encapsulam operações de leitura (ex: `GetCompanyByIdHandler`)
* Use Cases orquestram múltiplos commands/queries quando necessário
* DTOs são a única forma de dados que sai da camada Application
* Validações de entrada usando FluentValidation ou Data Annotations
* **Proibido:** lógica de negócio direta, acesso a banco, chamadas HTTP diretas

**Infrastructure:**
* Repositories implementam interfaces de Domain usando EF Core
* Mapeamentos Entity ↔ Domain via AutoMapper ou configuração Fluent API
* Mensageria implementada via RabbitMQ producer/consumer
* Serviços externos encapsulados em clientes com tratamento de erro
* **Proibido:** lógica de negócio complexa (pertence ao Domain)

**API:**
* Controllers recebem DTOs e delegam para Command/Query Handlers
* Middlewares tratam cross-cutting concerns (auth, logging, rate limiting)
* DI container registra todas as dependências no startup
* **Proibido:** lógica de negócio, acesso direto ao banco, regras de validação

### Padrão DDD (Domain-Driven Design)

* **Bounded Contexts:** cada microserviço (Companies, CRM, Conversations, Billing, Analytics, Notifications, AI) é um Bounded Context independente com seu próprio modelo de domínio.
* **Entidades:** objetos com identidade única e ciclo de vida (ex: `Company`, `Client`, `Appointment`).
* **Value Objects:** objetos imutáveis definidos por seus atributos (ex: `Cnpj`, `Email`, `Dinheiro`, `Periodo`).
* **Aggregate Roots:** pontos de consistência transacional; acessos a entidades internas do agregado só ocorrem pela raiz (ex: `Client` é Aggregate Root para `Contact` e `Opportunity`).
* **Domain Events:** publicados após sucesso de uma operação para disparar efeitos colaterais assíncronos.
* **Repositórios:** interfaces definidas em Domain, implementadas em Infrastructure. Um repository por Aggregate Root.
* **Services de Domínio:** lógica que não pertence a nenhuma entidade específica (ex: `SchedulingService` para verificação de conflitos).

### Padrão CQRS (Command Query Responsibility Segregation)

* **Escopo no MVP:** CQRS simplificado com separação de Commands (escrita) e Queries (leitura) no nível de service layer, sem event store separado.
* **Commands:** representam intenções de mudança de estado (ex: `CreateAppointmentCommand`, `ConfirmPaymentCommand`). Validados por Command Handlers que executam regras de negócio.
* **Queries:** representam consultas otimizadas para leitura (ex: `GetClientAppointmentsQuery`, `GetDashboardMetricsQuery`). Podem usar projeções materializadas para performance.
* **Eventos de Domínio:** publicados após sucesso de um Command para disparar efeitos colaterais (notificações, analytics, sincronização).
* **Mediator Pattern:** uso do MediatR como mediator para desacoplar Controllers de Handlers, promovendo baixo acoplamento e alta coesão.
* **Evolução Futura:** migração para CQRS completo com Event Store e projeções assíncronas quando o volume de dados justificar.

### Design Patterns Adotados

A tabela a seguir lista os padrões de projeto utilizados no sistema e onde se aplicam:

| Padrão | Tipo | Onde se aplica | Exemplo no domínio |
| :--- | :--- | :--- | :--- |
| **Repository** | Acesso a dados | Interfaces em Domain, implementação em Infrastructure | `ICompanyRepository` → `CompanyRepository` |
| **Unit of Work** | Transações | Gerenciamento de transações via EF Core `SaveChanges` | Garantir atomicidade ao criar cliente + compromisso |
| **CQRS** | Separação leitura/escrita | Command e Query Handlers | `CreateInvoiceCommand` vs `GetDashboardQuery` |
| **Mediator** | Desacoplamento | Comunicação entre camadas via MediatR | Controller → MediatR → Handler → Domain |
| **Strategy** | Comportamento variável | Provedores de IA, canais de notificação | `ILLMProvider` → `OpenAIProvider`, `AzureProvider` |
| **Observer** | Notificação em cadeia | Domain Events e event handlers | `InvoicePaidEvent` → notificar WhatsApp + atualizar analytics |
| **Saga/Orchestration** | Fluxos distribuídos | Processos que envolvem múltiplos serviços | Criação de compromisso → envio de confirmação → atualização de agenda |
| **Factory** | Criação de objetos | Criação de entidades com regras complexas | `Appointment.Create(client, slot, service)` |
| **Builder** | Construção gradual | Construção de objetos com muitos parâmetros | `ReportBuilder` para relatórios de analytics |
| **Specification** | Regras de negócio compostas | Consultas complexas com múltiplas condições | Filtrar clientes ativos com compromissos pendentes |
| **Circuit Breaker** | Resiliência | Chamadas a serviços externos (WhatsApp, LLM) | Bloquear chamadas por X tempo após N falhas consecutivas |
| **Retry com Backoff** | Resiliência | Retentativas de operações falhas | Retry exponencial em chamadas à API do WhatsApp |
| **Cache-Aside** | Performance | Cache de dados frequentemente consultados | Cache de dados da empresa em Redis com TTL |
| **Decorator** | Funcionalidades transversais | Validação, logging, auditoria sem modificar o handler | `[Authorize]`, `[ValidateModel]`, `[Audit]` |
| **Strategy (Frontend)** | Comportamento variável | Renderização condicional de componentes | `ChannelRenderer` para diferentes canais de atendimento |

### Princípios Clean Code

O código-fonte deve seguir os princípios **SOLID** e boas práticas de Clean Code:

#### SOLID

| Princípio | Aplicação |
| :--- | :--- |
| **S**ingle Responsibility | Cada classe/método tem uma única responsabilidade. Handlers处理 um único comando/query. |
| **O**pen/Closed | Entidades abertas para extensão, fechadas para modificação. Novas regras via Domain Events, não alteração de código existente. |
| **L**iskov Substitution | Subtipos devem ser substituíveis por seus tipos base. Implementações de repositório seguem o contrato da interface. |
| **I**nterface Segregation | Interfaces pequenas e específicas. `IReadRepository<T>` e `IWriteRepository<T>` em vez de `IRepository<T>` monolítica. |
| **D**ependency Inversion | Módulos de alto nível não dependem de módulos de baixo nível. Ambos dependem de abstrações (interfaces em Domain). |

#### Boas Práticas

* **DRY** (Don't Repeat Yourself): extrair lógica comum para métodos, extensões ou base classes.
* **KISS** (Keep It Simple, Stupid): preferir soluções simples e diretas. Evitar over-engineering no MVP.
* **YAGNI** (You Aren't Gonna Need It): não implementar funcionalidades "para o futuro" sem demanda atual.
* **Boy Scout Rule:** deixar o código melhor do que encontrou. Refatorar code smells durante desenvolvimento.
* **Naming expressivo:** nomes de classes, métodos e variáveis devem revelar intenção (ex: `ScheduleAppointment` em vez de `ProcessData`).
* **Métodos curtos:** máximo de 20-30 linhas por método. Extrair métodos auxiliares quando necessário.
* **Parâmetros limitados:** máximo de 3-4 parâmetros por método. Usar objetos (DTOs, Value Objects) quando houver mais.
* **Ausência de código morto:** remover código comentado, variáveis não utilizadas e branches inalcançáveis.
* **Imutabilidade:** preferir objetos imutáveis quando possível (Value Objects, Records em C#).
* **Expressividade:** código deve ler como prosa. Usar nomes descritivos em vez de comentários.

#### Frontend (React/Next.js)

* Componentes React com responsabilidade única (Smart vs Dumb components)
* Custom Hooks para lógica reativa compartilhada
* Server Components do Next.js para renderização no servidor quando possível
* Client Components apenas quando interatividade é necessária
* Zustand para estado global (incluindo store de tema: `useThemeStore`); React Query para estado de servidor
* Props drilling limitado a 2-3 níveis; usar Context ou stores quando necessário
* Componentes presentacionais desacoplados de lógica de negócio
* Tema (claro/escuro/sistema) gerenciado via Zustand store com persistência em localStorage e sincronização com API de preferências do usuário

### Restrições Arquiteturais

* Não misturar regras de negócio entre serviços
* Não acessar banco diretamente fora do backend de domínio
* Não criar dependências cíclicas entre microserviços
* Não colocar lógica de negócio em Controllers ou Components
* Não expor entidades de domínio diretamente como resposta de API (usar DTOs)
* Não usar `public` setters em entidades de domínio (usar métodos de comportamento)

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
* Verificar padrões de design existentes e reutilizar quando aplicável

Ao finalizar:

* Executar lint
* Executar testes
* Atualizar documentação
* Informar artefatos modificados
* Confirmar que o código segue SOLID, Clean Architecture e Clean Code

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
