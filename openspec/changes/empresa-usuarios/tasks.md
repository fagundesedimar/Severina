## 1. Domain Layer - Companies

- [ ] 1.1 Criar entidade Company (id, Nome, CnpjCpf, Email, Telefone, TipoPessoa, Status, Configuracoes, CreatedAt, UpdatedAt)
- [ ] 1.2 Criar Value Object CnpjCpf com validação de CPF/CNPJ
- [ ] 1.3 Criar Value Object Email com validação de formato
- [ ] 1.4 Criar Value Object Telefone com validação de formato
- [ ] 1.5 Criar enum TipoPessoa (Fisica, Juridica)
- [ ] 1.6 Criar enum StatusEmpresa (Ativa, Inativa, Suspensa)
- [ ] 1.7 Implementar comportamentos: Company.Activate(), Company.Deactivate(), Company.Suspend()
- [ ] 1.8 Criar Domain Events: CompanyCreatedEvent, CompanyDeactivatedEvent

## 2. Domain Layer - Users

- [ ] 2.1 Criar entidade User (id, CompanyId, Nome, Email, SenhaHash, Papel, Status, CreatedAt, UpdatedAt)
- [ ] 2.2 Criar enum PapelUsuario (Administrador, Operacional)
- [ ] 2.3 Criar enum StatusUsuario (Ativo, Inativo, Pendente)
- [ ] 2.4 Implementar comportamentos: User.Activate(), User.Deactivate(), User.ChangeRole()
- [ ] 2.5 Criar Domain Events: UserInvitedEvent, UserActivatedEvent, UserRoleChangedEvent

## 3. Infrastructure - EF Core

- [ ] 3.1 Criar CompanyConfiguration (IEntityTypeConfiguration) com Global Query Filter para CompanyId
- [ ] 3.2 Criar UserConfiguration com index em CompanyId + Email (único por empresa)
- [ ] 3.3 Criar migration inicial para Companies e Users
- [ ] 3.4 Criar ICompanyRepository com métodos CRUD
- [ ] 3.5 Criar IUserRepository com métodos CRUD e busca por empresa

## 4. Application - Companies

- [ ] 4.1 Criar DTOs: CompanyResponse, CreateCompanyRequest, UpdateCompanyRequest
- [ ] 4.2 Criar CreateCompanyCommand + Handler com validação (FluentValidation)
- [ ] 4.3 Criar GetCompanyQuery + Handler
- [ ] 4.4 Criar UpdateCompanyCommand + Handler
- [ ] 4.5 Criar DeactivateCompanyCommand + Handler
- [ ] 4.6 Configurar MediatR no DI container

## 5. Application - Users

- [ ] 5.1 Criar DTOs: UserResponse, InviteUserRequest, UpdateUserRoleRequest
- [ ] 5.2 Criar ListCompanyUsersQuery + Handler
- [ ] 5.3 Criar InviteUserCommand + Handler (gera código e salva em Redis)
- [ ] 5.4 Criar AcceptInviteCommand + Handler (valida código e cria usuário)
- [ ] 5.5 Criar DeactivateUserCommand + Handler
- [ ] 5.6 Criar UpdateUserRoleCommand + Handler
- [ ] 5.7 Criar UpdateUserProfileCommand + Handler

## 6. Application - Invite System

- [ ] 6.1 Criar IInviteCacheService interface para operações Redis
- [ ] 6.2 Criar InviteCacheService com SaveInvite, GetInvite, DeleteInvite, GetPendingInvites
- [ ] 6.3 Criar IEmailService interface
- [ ] 6.4 Criar MockEmailService para desenvolvimento
- [ ] 6.5 Integrar envio de email no InviteUserCommand

## 7. API Layer

- [ ] 7.1 Criar CompaniesController com endpoints CRUD
- [ ] 7.2 Criar UsersController com endpoints de gestão
- [ ] 7.3 Criar InvitesController com endpoints de convite
- [ ] 7.4 Criar middleware de extração de company_id do JWT
- [ ] 7.5 Configurar Authorization Policies (AdminOnly, AllAuthenticated)
- [ ] 7.6 Configurar Rate Limiting nos endpoints de convite

## 8. Frontend - Company Registration

- [ ] 8.1 Criar página /cadastro conforme protótipo Stitch (clientes.html)
- [ ] 8.2 Criar formulário PF/RJ com validação (React Hook Form + Zod)
- [ ] 8.3 Implementar toggle PF/PJ com mudança dinâmica de campos
- [ ] 8.4 Criar componente InputMask para CPF/CNPJ
- [ ] 8.5 Integrar com POST /api/v1/companies

## 9. Frontend - User Management

- [ ] 9.1 Criar página /configuracoes/usuarios
- [ ] 9.2 Criar lista de usuários com status e papel
- [ ] 9.3 Criar modal de convite de usuário
- [ ] 9.4 Criar modal de mudança de papel
- [ ] 9.5 Implementar desativação de usuário com confirmação
- [ ] 9.6 Integrar com endpoints de gestão de usuários

## 10. Frontend - Invite Flow

- [ ] 10.1 Criar página /convite/{code} para aceitar convite
- [ ] 10.2 Criar formulário de cadastro de usuário (nome, senha)
- [ ] 10.3 Integrar com POST /api/v1/invites/{code}/accept
- [ ] 10.4 Criar página de erro para convite expirado/inválido

## 11. Testes

- [ ] 11.1 Testes unitários de validação de CnpjCpf (CPF válido, inválido, CNPJ válido, inválido)
- [ ] 11.2 Testes unitários de Company (ativação, desativação, suspensão)
- [ ] 11.3 Testes unitários de User (ativação, desativação, mudança de papel)
- [ ] 11.4 Testes de integração de Company CRUD (EF Core + PostgreSQL)
- [ ] 11.5 Testes de integração de User CRUD
- [ ] 11.6 Testes de integração de Invite System (Redis + email mock)
- [ ] 11.7 Testes de integração de RBAC (papéis e permissões)
- [ ] 11.8 Testes E2E de cadastro de empresa (Playwright)
- [ ] 11.9 Testes E2E de convite de usuário (Playwright)
- [ ] 11.10 Testes de IDOR (across-tenant access deve retornar 404)

## 12. Lint e Qualidade

- [ ] 12.1 Rodar dotnet format no backend
- [ ] 12.2 Rodar ESLint no frontend (npm run lint)
- [ ] 12.3 Verificar cobertura mínima 80% backend, 70% frontend
- [ ] 12.4 Verificar acessibilidade (WCAG 2.1 AA) no formulário de cadastro
- [ ] 12.5 Verificar responsividade (mobile/tablet/desktop)
