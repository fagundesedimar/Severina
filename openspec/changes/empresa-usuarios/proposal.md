## Summary

Implementar o fluxo completo de cadastro de empresas (PF/PJ), onboarding guiado, e gerenciamento de usuários com sistema de convites e controle de acesso por papéis (RBAC).

## Problem Statement

Sem esta funcionalidade, não existe isolamento de dados entre empresas e não há como gerenciar quem tem acesso ao sistema. O cadastro de empresa é o primeiro passo para qualquer novo cliente da Severina.

## Solution Approach

### Backend
- CRUD de Company (nome, cnpj_cpf, plano, status, configurações)
- CRUD de User (nome, email, senha, papel, status)
- Sistema de convites por email (código de convite com expiração)
- RBAC: Administrador da Empresa (tudo), Usuário Operacional (limitado)
- Endpoint POST /api/v1/companies (cadastro)
- Endpoint POST /api/v1/invites (enviar convite)
- Endpoint POST /api/v1/invites/{code}/accept (aceitar convite)

### Frontend
- Formulário de cadastro PF/PJ conforme protótipo Stitch (clientes.html)
- Tela de onboarding guiado (step-by-step)
- Tela de gestão de usuários (listar, convidar, desativar)
- Controle de visibilidade baseado no papel

### Integrações
- Email service para envio de convites
- Redis para cache de códigos de convite

## Impact

- **Arquivos novos**: Company/User entities, controllers, services, migrations, pages
- **Arquivos modificados**: Auth service (adicionar company_id no JWT)
- **Dependencies**: Email service (novo), Redis (cache de convites)
- **Risco**: Multi-tenant isolation precisa ser testado rigorosamente (anti-IDOR)

## Stitch References
- `stitch-prototypes/clientes.html` - Formulário PF/PJ
- `stitch-prototypes/login.html` - Tema e design tokens
