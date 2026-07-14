## 1. Setup do Projeto Backend

- [x] 1.1 Criar solução ASP.NET Core 8 com estrutura Clean Architecture (Domain, Application, Infrastructure, API)
- [x] 1.2 Configurar Entity Framework Core com PostgreSQL
- [x] 1.3 Criar entidade Company no Domain (id, nome, cnpj_cpf, plano, status)
- [x] 1.4 Criar entidade User no Domain (id, company_id, nome, email, senha_hash, papel, status)
- [x] 1.5 Criar migrations iniciais (Company, User)
- [x] 1.6 Configurar JWT Bearer no API

## 2. Setup do Projeto Frontend

- [x] 2.1 Criar projeto Next.js com TypeScript, Tailwind CSS, ESLint
- [x] 2.2 Configurar App Router com estrutura de pastas (app, components, hooks, stores, services)
- [x] 2.3 Instalar dependências (zustand, react-query, @heroicons/react)
- [x] 2.4 Configurar design system com tokens do Stitch (colors, typography, spacing)
- [x] 2.5 Criar CSS custom properties para light/dark themes

## 3. Backend - Autenticação

- [x] 3.1 Criar endpoint POST /api/v1/auth/login
- [x] 3.2 Implementar validação de credenciais (email + senha_hash via BCrypt)
- [x] 3.3 Gerar JWT access token (15min) com claims (sub, email, company_id, papel)
- [x] 3.4 Gerar refresh token (7 dias) e salvar em HTTP-only cookie
- [x] 3.5 Criar endpoint POST /api/v1/auth/refresh
- [x] 3.6 Criar endpoint POST /api/v1/auth/logout (invalida refresh token)
- [x] 3.7 Implementar middleware de autenticação JWT

## 4. Backend - Multi-Tenant

- [ ] 4.1 Configurar Global Query Filter para company_id no EF Core
- [ ] 4.2 Criar middleware para extrair company_id do JWT
- [ ] 4.3 Implementar verificação de company_id em todos os endpoints
- [ ] 4.4 Criar testes de integração para isolamento de tenants

## 5. Frontend - Tema

- [x] 5.1 Criar useThemeStore (Zustand) com persistência localStorage
- [x] 5.2 Implementar toggle de tema (light/dark/system) com micro-interações
- [x] 5.3 Configurar CSS custom properties para light/dark themes
- [x] 5.4 Implementar tema padrão "system" via prefers-color-scheme
- [ ] 5.5 Sincronizar preferência de tema com API (PUT /api/v1/users/preferences)

## 6. Frontend - Login

- [x] 6.1 Criar página /login conforme protótipo Stitch (login.html)
- [x] 6.2 Implementar formulário de email/senha com validação
- [x] 6.3 Implementar toggle de visibilidade da senha
- [x] 6.4 Implementar botão "Entrar" com loading spinner
- [x] 6.5 Implementar link "Cadastre-se" (futuro)
- [x] 6.6 Implementar chamada POST /api/v1/auth/login
- [x] 6.7 Armazenar JWT em memory/zustand após login
- [x] 6.8 Redirecionar para /dashboard após sucesso

## 7. Frontend - Autenticação

- [x] 7.1 Criar hook useAuth para gerenciar estado de autenticação
- [x] 7.2 Implementar interceptador HTTP para injetar JWT nas requisições
- [x] 7.3 Implementar refresh automático de token quando expira
- [x] 7.4 Criar rota protegida (redirect para /login se não autenticado)
- [x] 7.5 Implementar logout e limpeza de estado

## 8. Testes

- [ ] 8.1 Testes unitários de validação de email/senha (backend)
- [ ] 8.2 Testes unitários de geração/validação de JWT (backend)
- [ ] 8.3 Testes de integração de login/refresh/logout (backend)
- [ ] 8.4 Testes de integração de multi-tenant (backend)
- [ ] 8.5 Testes unitários de useThemeStore (frontend)
- [ ] 8.6 Testes unitários de useAuth (frontend)
- [ ] 8.7 Testes E2E de fluxo login → dashboard (Playwright)

## 9. Lint e Qualidade

- [ ] 9.1 Rodar ESLint no frontend (npm run lint)
- [ ] 9.2 Rodar dotnet format no backend
- [ ] 9.3 Verificar cobertura mínima 80% backend, 70% frontend
- [ ] 9.4 Verificar acessibilidade (WCAG 2.1 AA) no componente de login
- [ ] 9.5 Verificar responsividade (mobile/tablet/desktop)
