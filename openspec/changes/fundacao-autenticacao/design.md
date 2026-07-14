## Context

O Severina AI é uma secretária virtual para pequenas empresas. A autenticação é o primeiro módulo a ser implementado, servindo como base para todos os outros bounded contexts. O protótipo do Stitch (`login.html`) define a UI padrão para login.

**Estado atual:** Projetos não inicializados
**Restrições:** Multi-tenant com company_id, JWT com refresh token, Clean Architecture

## Goals / Non-Goals

**Goals:**
- Implementar autenticação JWT com access token (15min) e refresh token (7 dias)
- Suportar multi-tenant lógico com company_id em todas as entidades
- Criar tela de login conforme protótipo Stitch com micro-interações
- Configurar Clean Architecture no backend (Domain → Application → Infrastructure → API)
- Configurar Next.js com App Router, TypeScript e Tailwind CSS

**Non-Goals:**
- MFA (futuro)
- Login via Google (futuro)
- OAuth/OIDC completo (futuro)
- Rate limiting avançado (futuro)

## Decisions

### 1. JWT com Refresh Token

**Decisão:** Usar access token (15min) + refresh token (7 dias) armazenado em HTTP-only cookie.

**Alternativas consideradas:**
- Session-based: Requer Redis em cada requisição, mais complexo
- JWT sem refresh: Experiência ruim com sessões curtas
- OAuth/OIDC: Complexo demais para MVP

**Justificativa:** JWT é stateless, escalável e suporta multi-tenant via claims. Refresh token em HTTP-only cookie previne XSS.

### 2. Multi-Tenant com company_id

**Decisão:** Coluna `company_id` em todas as tabelas com filtro automático via Global Query Filter no EF Core.

**Alternativas consideradas:**
- Schema por tenant: Complexo para migrações
- Database por tenant: Caro para MVP
- Row-level security do PostgreSQL: Não suportado em todas as versões

**Justificativa:** Simples, eficiente e suficiente para 1000 empresas no MVP. Global Query Filter garante isolamento automático.

### 3. Clean Architecture

**Decisão:** Separar em camadas Domain, Application, Infrastructure, API com dependência unidirecional.

**Estrutura:**
```
src/backend/
  BuildingBlocks/
    Domain/          # Entidades, Value Objects, Domain Events
    Application/     # Commands, Queries, Handlers, DTOs
    Infrastructure/  # EF Core, Repositories, External Services
    API/             # Controllers, Middlewares
  Identity/          # Bounded Context: Autenticação
```

**Justificativa:** Separação de responsabilidades, testabilidade, independência de framework.

### 4. Frontend com Next.js App Router

**Decisão:** Usar App Router com Server Components para páginas estáticas e Client Components para interatividade.

**Alternativas consideradas:**
- Pages Router: Mais maduro, mas menos performático
- Remix: Menor ecossistema

**Justificativa:** App Router é o padrão oficial, suporta Server Components e melhor performance.

### 5. Tema com CSS Custom Properties

**Decisão:** Usar CSS custom properties no `:root` (light) e `[data-theme="dark"]` (dark) com toggle via `useThemeStore` (Zustand).

**Alternativas consideradas:**
- next-themes: Dependência externa desnecessária
- Tailwind darkMode class: Não suporta "system" nativamente

**Justificativa:** Controle total, persistência em localStorage + API, suporte a "system" via `prefers-color-scheme`.

## Risks / Trade-offs

| Risco | Mitigação |
|-------|-----------|
| JWT leak via XSS | HTTP-only cookie para refresh token, SameSite=Strict |
| Multi-tenant leak | Global Query Filter + testes de integração |
| Performance com muitos tenants | Índice em company_id, cache de sessão |
| Migrações complexas | EF Core migrations com backup automático |

## Fluxo de Autenticação

```
1. Usuário insere email/senha
2. Frontend chama POST /api/v1/auth/login
3. Backend valida credenciais, gera JWT + refresh token
4. Refresh token salvo em HTTP-only cookie
5. JWT retornado no body (armazenado em memory/zustand)
6. Usuário redirecionado para dashboard
7. Em cada requisição, JWT enviado no header Authorization
8. Quando JWT expira, frontend chama POST /api/v1/auth/refresh
9. Novo JWT retornado, fluxo continua
```

## Protótipo de Referência

- **Arquivo:** `stitch-prototypes/login.html`
- **Screen ID:** `3c86564ba69c4c5da5933231ff03a2f6`
- **Tokens extraídos:** primary=#004ac6, primary-container=#2563eb, surface=#faf8ff
