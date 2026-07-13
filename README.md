# Severina AI

Secretária virtual inteligente para pequenas empresas. Automatiza atendimento omnichannel, agendamentos, cobranças, follow-up e geração de insights com base em Inteligência Artificial.

---

## Sobre o Problema

Pequenas empresas e profissionais autônomos sofrem com a sobrecarga de tarefas administrativas e operacionais. Atendimento, agenda, cobrança e follow-up permanecem fragmentados entre WhatsApp, planilhas e cadernos. O resultado é perda de receita, atendimento inconsistente e baixa produtividade.

## Sobre a Solução

A Severina AI centraliza atendimento, agenda, finanças e CRM em uma única plataforma SaaS, com assistente virtual baseada em IA (RAG) que responde clientes automaticamente, agenda compromissos sem conflitos e envia cobranças com follow-up inteligente.

## Funcionalidades (MVP)

- **Atendimento Omnichannel** — WhatsApp e web integrados em um único fluxo
- **Agendamento Inteligente** — validação automática de conflitos e lembretes
- **Cobrança Automática** — geração, envio e follow-up de cobranças
- **CRM de Clientes** — timeline de interações e histórico completo
- **Dashboard de Insights** — métricas de atendimento, agenda e financeiro
- **Modo Claro/Escuro** — preferência persistida por usuário com toggle acessível

## Stack Tecnológica

| Camada | Tecnologia |
| :--- | :--- |
| Frontend | React, Next.js, TypeScript, Tailwind CSS, Shadcn |
| Backend | ASP.NET Core 8, C#, Entity Framework Core |
| Banco de Dados | PostgreSQL 15+, Redis, pgvector |
| Mensageria | RabbitMQ |
| IA / RAG | Embedding Service, pgvector, LLM Provider (multi-provider) |
| Infraestrutura | Docker, Kubernetes, Terraform, Azure/AWS |
| CI/CD | GitHub Actions ou Azure DevOps |
| Observabilidade | OpenTelemetry, Prometheus, Grafana, Jaeger |

## Pré-requisitos

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Node.js 18+](https://nodejs.org/)
- [Docker](https://www.docker.com/get-started)
- [PostgreSQL 15+](https://www.postgresql.org/download/) (ou via Docker)

## Início Rápido

### 1. Clone o repositório

```bash
git clone https://github.com/seu-usuario/severina-ai.git
cd severina-ai
```

### 2. Configure as variáveis de ambiente

```bash
cp .env.example .env
```

Preencha as variáveis de ambiente no arquivo `.env` conforme as instruções contidas nele.

### 3. Suba os serviços com Docker

```bash
docker-compose up -d
```

Isso inicia PostgreSQL, Redis e RabbitMQ.

### 4. Execute as migrations do banco

```bash
cd src/backend
dotnet ef database update --project BuildingBlocks/Infrastructure --startup-project API
```

### 5. Inicie o frontend

```bash
cd src/frontend
npm install
npm run dev
```

Acesse: `http://localhost:3000`

### 6. Inicie o backend

```bash
cd src/backend/API
dotnet run
```

A API estará disponível em: `http://localhost:3001/api/v1`

## Estrutura do Projeto

```text
src/
  frontend/
    app/                    # Next.js App Router (pages/layouts)
    components/             # Componentes UI reutilizáveis
    hooks/                  # Custom hooks
    services/               # Chamadas API e lógica de acesso a dados
    stores/                 # Zustand stores (estado global)
    styles/                 # Tailwind config, CSS custom properties
    types/                  # TypeScript type definitions
  backend/
    BuildingBlocks/
      Domain/               # Entidades, Value Objects, Domain Events
      Application/          # Use Cases, Command/Query Handlers
      Infrastructure/       # Implementações de repositórios, EF Core
      API/                  # Controllers, Middlewares, Filters
    SharedKernel/           # Utilitários compartilados
    Identity/               # Autenticação e autorização
    Companies/              # Bounded Context: gestão de empresas
    CRM/                    # Bounded Context: relacionamento com clientes
    Conversations/          # Bounded Context: atendimento omnichannel
    AI/                     # Bounded Context: serviços de IA / RAG
    Notifications/          # Bounded Context: notificações
    Billing/                # Bounded Context: cobranças e pagamentos
    Analytics/              # Bounded Context: métricas e relatórios
    Integrations/           # Bounded Context: integrações externas
    Gateway/                # API Gateway / BFF
infra/
  terraform/
  k8s/
docs/
  prd.md                    # Definição de Requisitos do Produto
  spec_req.md               # Especificação Técnica de Sistema
  spec_tech.md              # Arquitetura de Software
  spec_ui.md                # Especificação de UI
  design.md                 # Design System
  VisaoDeProduto.md         # Visão de Produto
  problem.md                # Declaração do Problema
```

## Arquitetura

O sistema segue uma arquitetura de **Microservices orientado a eventos** com **Clean Architecture**, **DDD** e **CQRS**.

```text
Frontend (Next.js) → API Gateway → Microserviços de Domínio → PostgreSQL
                                       ↓
                                  RabbitMQ → Workers Assíncronos (IA, Notificações, Analytics)
```

Para detalhes completos, consulte [docs/spec_tech.md](docs/spec_tech.md).

## Design System

O design system é construído sobre a filosofia de **clareza funcional com presença discreta**. Tokens de cor, tipografia, espaçamento e componentes estão documentados em [docs/design.md](docs/design.md).

Para especificação das interfaces gráficas e fluxos de navegação, consulte [docs/spec_ui.md](docs/spec_ui.md).

## Testes

```bash
# Backend — testes de unidade
cd src/backend
dotnet test

# Frontend — testes de unidade
cd src/frontend
npm test

# E2E (Playwright)
npm run test:e2e
```

Cobertura mínima: **80% linhas backend**, **70% linhas frontend**.

## Documentação

| Documento | Descrição |
| :--- | :--- |
| [Visão de Produto](docs/VisaoDeProduto.md) | Problema, público-alvo, objetivos e escopo |
| [PRD](docs/prd.md) | Requisitos funcionais e não funcionais |
| [Especificação Técnica](docs/spec_req.md) | Requisitos detalhados, endpoints e modelagem de dados |
| [Arquitetura](docs/spec_tech.md) | Arquitetura de software, stack e padrões |
| [UI](docs/spec_ui.md) | Interfaces gráficas e fluxos de navegação |
| [Design System](docs/design.md) | Tokens, componentes e diretrizes visuais |
| [Problema](docs/problem.md) | Declaração do problema |

## Contribuindo

Consulte as diretrizes de contribuição antes de abrir um Pull Request.

1. Faça fork do repositório
2. Crie uma branch de feature (`git checkout -b feature/nova-funcionalidade`)
3. Commit suas alterações (`git commit -m 'Adiciona nova funcionalidade'`)
4. Push para a branch (`git push origin feature/nova-funcionalidade`)
5. Abra um Pull Request

## Licença

Este projeto está sob licença. Consulte o arquivo [LICENSE](LICENSE) para mais detalhes.
