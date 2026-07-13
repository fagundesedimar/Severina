# AGENTS.md — Severina AI

## Comportamento do Agente

1. **Pense antes de codificar.** Não assuma. Se houver múltiplas interpretações, apresente todas. Se uma abordagem mais simples existir, diga.
2. **Mude apenas o necessário.** Não "melhore" código adjacente. Não refatore o que não está quebrado. Cada linha alterada deve rastrear até o pedido do usuário.
3. **Simplicidade primeiro.** Código mínimo que resolve o problema. Sem abstrações especulativas. Se 200 linhas poderiam ser 50, reescreva.
4. **Critérios de conclusão.** "Feito" significa: lint passando, testes passando, build OK. Nunca declare pronto sem verificar.

---

## Stack Tecnológica

| Camada | Tecnologia |
| :--- | :--- |
| Frontend | React, Next.js (App Router), TypeScript, Tailwind CSS, Shadcn |
| Backend | C#, .NET 8, ASP.NET Core, Entity Framework Core |
| Banco | PostgreSQL 15+, Redis, pgvector |
| Mensageria | RabbitMQ (AMQP) |
| IA/RAG | Embedding Service, pgvector, LLM multi-provider |
| Infra | Docker, Kubernetes, Terraform |
| Auth | Clerk (JWT/OIDC) |
| Observabilidade | OpenTelemetry, Prometheus, Grafana |

---

## Estrutura do Monorepo

```text
src/
  frontend/        # Next.js — pages, components, hooks, stores, services
  backend/
    BuildingBlocks/ # Domain, Application, Infrastructure, API
    SharedKernel/   # Utilitários compartilados
    Identity/       # Autenticação e autorização
    Companies/      # Bounded Context: empresas
    CRM/            # Bounded Context: clientes
    Conversations/  # Bounded Context: atendimento omnichannel
    AI/             # Bounded Context: IA / RAG
    Notifications/  # Bounded Context: notificações
    Billing/        # Bounded Context: cobranças
    Analytics/      # Bounded Context: métricas
    Integrations/   # Bounded Context: integrações externas
    Gateway/        # API Gateway / BFF
infra/              # terraform, k8s
docs/               # prd, spec_req, spec_tech, spec_ui, design, etc.
```

---

## Comandos Principais

### Setup

```bash
cp .env.example .env          # configurar variáveis
docker-compose up -d           # subir PostgreSQL, Redis, RabbitMQ
dotnet ef database update      # migrations (backend)
cd src/frontend && npm install # dependências frontend
```

### Build

```bash
# Frontend
cd src/frontend && npm run build

# Backend
cd src/backend && dotnet build
```

### Run

```bash
# Frontend (porta 3000)
cd src/frontend && npm run dev

# Backend (porta 3001)
cd src/backend/API && dotnet run
```

### Banco de Dados

```bash
# Criar migration
dotnet ef migrations add <Nome> --project BuildingBlocks/Infrastructure --startup-project API

# Aplicar migrations
dotnet ef database update --project BuildingBlocks/Infrastructure --startup-project API

# Remover última migration
dotnet ef migrations remove --project BuildingBlocks/Infrastructure --startup-project API
```

### Testes

```bash
# Backend — unidade e integração
cd src/backend && dotnet test

# Frontend — unidade
cd src/frontend && npm test

# E2E (Playwright)
cd src/frontend && npm run test:e2e

# Lint
cd src/frontend && npm run lint
dotnet format --verify-no-changes --project src/backend
```

---

## Regras de Qualidade e Testes

- Cobertura mínima: **80% linhas backend**, **70% linhas frontend**
- Toda alteração de regra de negócio cobre: **Happy Path**, **Sad Path**, **Edge Cases**
- Testes de unidade: xUnit (backend), Jest (frontend)
- Testes de integração: xUnit + TestContainers
- Testes E2E: Playwright
- Lint: ESLint (frontend), dotnet format (backend)
- Pipeline CI/CD executa testes em cada PR com bloqueio de merge se cobertura abaixo do mínimo

---

## Governança e Autonomia no Terminal

### Sempre faça (autônomo)

- Rodar lint, testes e build para validar alterações
- Ler arquivos de documentação em `docs/`
- Criar/editar componentes, services, hooks, stores, types
- Criar migrations e entidades de domínio
- Executar comandos dotnet, npm, git (status, diff, log)

### Pergunte primeiro

- Alterar schema de banco de dados (migrations que apagam colunas/tabelas)
- Alterar variáveis de ambiente ou configuração de deploy
- Modificar autenticação, autorização ou regras de segurança
- Instalar novas dependências não previstas na stack
- Fazer push, commit, criar PR ou alterar branch

### Nunca faça

- Expor chaves de API, segredos ou `.env` em código ou commits
- Alterar `spec_tech.md`, `spec_req.md` ou `prd.md` sem pedido explícito
- Pular testes ou reduzir cobertura para "agilizar"
- Criar lógica de negócio no frontend (pertence ao backend)
- Usar `console.log()` em código de produção (usar logs estruturados)
- Acessar banco de dados diretamente fora do backend de domínio

---

## Context7 MCP — Consulta de Documentação

Use o Context7 MCP para consultar documentação atualizada de libs e frameworks antes de implementar:

```text
# Sempre antes de usar uma API/lib nova:
1. Buscar documentação via Context7 MCP
2. Verificar se o padrão já existe no codebase
3. Implementar seguindo o padrão encontrado
```

Exemplo: antes de usar uma API do Entity Framework ou Next.js, consulte a documentação via Context7 para garantir uso correto e atualizado.

---

## Documentação do Projeto

| Arquivo | Descrição |
| :--- | :--- |
| `docs/VisaoDeProduto.md` | Problema, personas, objetivos e escopo |
| `docs/prd.md` | Requisitos funcionais e não funcionais |
| `docs/spec_req.md` | Especificação técnica, endpoints e modelagem |
| `docs/spec_tech.md` | Arquitetura, stack, padrões e Clean Architecture |
| `docs/spec_ui.md` | Interfaces gráficas e fluxos de navegação |
| `docs/design.md` | Design system: tokens, componentes e responsividade |
| `docs/problem.md` | Declaração do problema |

---

## Aprendizado Contínuo

Ao final de cada mudança, execute esta verificação:

1. **Lint e testes passaram?** → `dotnet test` e `npm test` com saída zero
2. **Build OK?** → `dotnet build` e `npm run build` sem erros
3. **Padrão mantido?** → Código segue Clean Architecture, DDD, CQRS e SOLID
4. **Documentação atualizada?** → Se aplicável, atualizar docs relevantes
5. **Reflexão:** Houve código desnecessário? Alguma abordagem poderia ser mais simples? Há sugestão de melhoria para o time?

Se qualquer item falhar, corrija antes de declarar a tarefa concluída.
