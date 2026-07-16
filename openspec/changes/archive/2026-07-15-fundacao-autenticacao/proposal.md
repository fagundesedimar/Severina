## Why

O Severina AI precisa de uma base sólida de autenticação e autorização para suportar o multi-tenant lógico com `company_id`. A tela de login (INT-01) é o ponto de entrada principal dos usuários e deve seguir o protótipo do Stitch (`login.html`) com micro-interações de tema e validação de credenciais.

## What Changes

- Inicialização do projeto Next.js com TypeScript, Tailwind CSS, ESLint e App Router
- Inicialização do projeto ASP.NET Core 8 com Clean Architecture (Domain, Application, Infrastructure, API)
- Configuração do PostgreSQL com Entity Framework Core e migrations iniciais
- Implementação de autenticação JWT com access token e refresh token
- Implementação de autorização multi-tenant com claims `company_id`
- Tela de Login (INT-01) conforme protótipo Stitch com:
  - Formulário de email/senha com validação
  - Toggle de visibilidade da senha
  - Botão "Entrar" com loading spinner
  - Login via Google (futuro)
  - Toggle de tema (claro/escuro) com micro-interações
- Endpoint POST /api/v1/auth/login
- Endpoint POST /api/v1/auth/refresh
- Endpoint POST /api/v1/auth/logout

## Capabilities

### New Capabilities

- `auth-login`: Autenticação de usuário com JWT, login/logout, refresh token
- `theme-toggle`: Toggle de tema claro/escuro/sistema com persistência localStorage + API
- `multi-tenant`: Isolamento de dados por company_id em todas as entidades

### Modified Capabilities

(Nenhuma — change inicial)

## Impact

- **Código afetado**: Novos projetos Next.js e ASP.NET Core
- **APIs**: POST /api/v1/auth/login, POST /api/v1/auth/refresh, POST /api/v1/auth/logout
- **Dependências**: Next.js, ASP.NET Core, EF Core, JWT Bearer, Tailwind CSS
- **Banco**: Tabelas iniciais (empresa, usuario) com migrations
- **Segurança**: JWT com refresh token, RBAC com papéis (Admin, Operacional)
- **Multi-tenant**: company_id em todas as requisições
- **Observabilidade**: Logs de autenticação via OpenTelemetry
- **Protótipo**: `stitch-prototypes/login.html` (Screen ID: 3c86564ba69c4c5da5933231ff03a2f6)
