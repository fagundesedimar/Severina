## 1. Domain Layer - Companies

- [x] 1.1 Criar entidade Company (id, Nome, CnpjCpf, Email, Telefone, TipoPessoa, Status, Configuracoes, CreatedAt, UpdatedAt)
- [x] 1.2 Criar Value Object CnpjCpf com validação de CPF/CNPJ
- [x] 1.3 Criar Value Object Email com validação de formato
- [x] 1.4 Criar Value Object Telefone com validação de formato
- [x] 1.5 Criar enum TipoPessoa (Fisica, Juridica)
- [x] 1.6 Criar enum StatusEmpresa (Ativa, Inativa, Suspensa)
- [x] 1.7 Implementar comportamentos: Company.Activate(), Company.Deactivate(), Company.Suspend()
- [x] 1.8 Criar Domain Events: CompanyCreatedEvent, CompanyDeactivatedEvent

## 2. Domain Layer - Users

- [x] 2.1 Criar entidade User (id, CompanyId, Nome, Email, SenhaHash, Papel, Status, CreatedAt, UpdatedAt)
- [x] 2.2 Criar enum PapelUsuario (Administrador, Operacional)
- [x] 2.3 Criar enum StatusUsuario (Ativo, Inativo, Pendente)
- [x] 2.4 Implementar comportamentos: User.Activate(), User.Deactivate(), User.ChangeRole()
- [x] 2.5 Criar Domain Events: UserInvitedEvent, UserActivatedEvent, UserRoleChangedEvent

## 3. Infrastructure - EF Core

- [x] 3.1 Criar CompanyConfiguration (IEntityTypeConfiguration) com Global Query Filter para CompanyId
- [x] 3.2 Criar UserConfiguration com index em CompanyId + Email (único por empresa)
- [x] 3.3 Criar migration inicial para Companies e Users
- [x] 3.4 Criar ICompanyRepository com métodos CRUD
- [x] 3.5 Criar IUserRepository com métodos CRUD e busca por empresa

## 4. Application - Companies

- [x] 4.1 Criar DTOs: CompanyResponse, CreateCompanyRequest, UpdateCompanyRequest
- [x] 4.2 Criar CreateCompanyCommand + Handler com validação (FluentValidation)
- [x] 4.3 Criar GetCompanyQuery + Handler
- [x] 4.4 Criar UpdateCompanyCommand + Handler
- [x] 4.5 Criar DeactivateCompanyCommand + Handler
- [x] 4.6 Configurar MediatR no DI container

## 5. Application - Users

- [x] 5.1 Criar DTOs: UserResponse, InviteUserRequest, UpdateUserRoleRequest
- [x] 5.2 Criar ListCompanyUsersQuery + Handler
- [x] 5.3 Criar InviteUserCommand + Handler (gera código e salva em Redis)
- [x] 5.4 Criar AcceptInviteCommand + Handler (valida código e cria usuário)
- [x] 5.5 Criar DeactivateUserCommand + Handler
- [x] 5.6 Criar UpdateUserRoleCommand + Handler
- [x] 5.7 Criar UpdateUserProfileCommand + Handler

## 6. Application - Invite System

- [x] 6.1 Criar IInviteCacheService interface para operações Redis
- [x] 6.2 Criar InviteCacheService com SaveInvite, GetInvite, DeleteInvite, GetPendingInvites
- [x] 6.3 Criar IEmailService interface
- [x] 6.4 Criar MockEmailService para desenvolvimento
- [x] 6.5 Integrar envio de email no InviteUserCommand

## 7. API Layer

- [x] 7.1 Criar CompaniesController com endpoints CRUD
- [x] 7.2 Criar UsersController com endpoints de gestão
- [x] 7.3 Criar InvitesController com endpoints de convite
- [x] 7.4 Criar middleware de extração de company_id do JWT
- [x] 7.5 Configurar Authorization Policies (AdminOnly, AllAuthenticated)
- [x] 7.6 Configurar Rate Limiting nos endpoints de convite

## 8. Frontend - Company Registration

- [x] 8.1 Criar página /cadastro conforme protótipo Stitch (clientes.html)
- [x] 8.2 Criar formulário PF/RJ com validação (React Hook Form + Zod)
- [x] 8.3 Implementar toggle PF/PJ com mudança dinâmica de campos
- [x] 8.4 Criar componente InputMask para CPF/CNPJ
- [x] 8.5 Integrar com POST /api/v1/companies

## 9. Frontend - User Management

- [x] 9.1 Criar página /configuracoes/usuarios
- [x] 9.2 Criar lista de usuários com status e papel
- [x] 9.3 Criar modal de convite de usuário
- [x] 9.4 Criar modal de mudança de papel
- [x] 9.5 Implementar desativação de usuário com confirmação
- [x] 9.6 Integrar com endpoints de gestão de usuários

## 10. Frontend - Invite Flow

- [x] 10.1 Criar página /convite/{code} para aceitar convite
- [x] 10.2 Criar formulário de cadastro de usuário (nome, senha)
- [x] 10.3 Integrar com POST /api/v1/invites/{code}/accept
- [x] 10.4 Criar página de erro para convite expirado/inválido

## 11. Testes

- [x] 11.1 Testes unitários de validação de CnpjCpf (CPF válido, inválido, CNPJ válido, inválido)
- [x] 11.2 Testes unitários de Company (ativação, desativação, suspensão)
- [x] 11.3 Testes unitários de User (ativação, desativação, mudança de papel)
- [x] 11.4 Testes de integração de Company CRUD (EF Core + PostgreSQL)
- [x] 11.5 Testes de integração de User CRUD
- [x] 11.6 Testes de integração de Invite System (Redis + email mock)
- [x] 11.7 Testes de integração de RBAC (papéis e permissões)
- [x] 11.8 Testes E2E de cadastro de empresa (Playwright)
- [x] 11.9 Testes E2E de convite de usuário (Playwright)
- [x] 11.10 Testes de IDOR (across-tenant access deve retornar 404)

## 12. Lint e Qualidade

- [x] 12.1 Rodar dotnet format no backend
- [x] 12.2 Rodar ESLint no frontend (npm run lint)
- [x] 12.3 Verificar cobertura mínima 80% backend, 70% frontend
- [x] 12.4 Verificar acessibilidade (WCAG 2.1 AA) no formulário de cadastro
- [x] 12.5 Verificar responsividade (mobile/tablet/desktop)
