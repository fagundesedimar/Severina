## Context

A Severina AI precisa de um sistema de cadastro de empresas e gerenciamento de usuários para viabilizar o isolamento multi-tenant. Atualmente não existe estrutura para suportar múltiplas empresas no mesmo sistema. O Bounded Context Companies será responsável pelo ciclo de vida completo de empresas e seus usuários.

## Goals / Non-Goals

**Goals:**
- Criar entidades Company e User com domínio rico (Value Objects, Domain Events)
- Implementar RBAC com dois papéis: Administrador da Empresa e Usuário Operacional
- Sistema de convites por email com código expirável
- Multi-tenant isolamento via company_id em todas as queries
- Formulário de cadastro PF/PJ conforme protótipo Stitch
- Onboarding guiado para novas empresas

**Non-Goals:**
- MFA (será implementado em mudança separada)
- Integração com Google Workspace ou Microsoft 365
- Provisionamento automático de WhatsApp Business
- Planos de pagamento (billing separado)

## Decisions

### Decisão 1: Entidades com Domínio Rico (não anêmico)

**Escolha**: Company e User com comportamento encapsulado nos modelos de domínio.

**Alternativas consideradas:**
- Models anêmicos (propriedades públicas + services externos) — rejeitado por não seguir DDD
- Records imutáveis — rejeitado porque Company precisa de mutação de estado (status, config)

**Justificativa**: Clean Architecture exige que Domain contenha lógica de negócio. Métodos como `Company.Activate()`, `User.Invite()` encapsulam regras.

### Decisão 2: CQRS com MediatR

**Escolha**: Commands e Queries separados via MediatR para operações de Company/User.

**Alternativas consideradas:**
- Controllers diretamente chamando Services — rejeitado por acoplar lógica de negócio na API
- Repository pattern puro — rejeitado por não suportar validação centralizada

**Justificativa**: CQRS simplificado (sem Event Sourcing) é suficiente para MVP. MediatR facilita testes unitários e separação de concerns.

### Decisão 3: Sistema de Convites com Redis

**Escolha**: Códigos de convite armazenados em Redis com TTL de 7 dias.

**Alternativas consideradas:**
- Tabela no PostgreSQL — rejeitado por overhead de cleanup
- JWT como código de convite — rejeitado por não ser introspectível

**Justificativa**: Redis é natural para dados temporários com TTL. Convites expiram automaticamente sem cleanup manual.

### Decisão 4: RBAC com Claims no JWT

**Escolha**: Papéis (role) e company_id como claims no JWT. Validação no backend via Policy-based authorization.

**Alternativas consideradas:**
- Claims por endpoint (attribute-based) — rejeitado por complexidade de manutenção
- Session-based auth — rejeitado por não ser stateless

**Justificativa**: JWT com claims é o padrão da stack. Policies centralizam regras de acesso.

### Decisão 5: Onboarding Wizard no Frontend

**Escolha**: Wizard de 3 passos (Dados da Empresa → Perfil → Convites) usando React Hook Form + Zustand.

**Alternativas consideradas:**
- Formulário único longo — rejeitado por UX ruim
- Múltiplas páginas separadas — rejeitado por perder contexto

**Justificativa**: Wizard mantém o usuário orientado. Zustand preserva estado entre passos.

## Risks / Trade-offs

- **[Risco] Multi-tenant isolation bypass** → Mitigação: Global Query Filter no EF Core + testes automatizados de IDOR
- **[Risco] Convites não expiram se Redis cair** → Mitigação: TTL no Redis + validação de data no backend
- **[Risco] RBAC pode ser muito restritivo** → Mitigação: Criar papel customizado futuro se necessário
- **[Trade-off] CQRS sem Event Sourcing** → Aceitável para MVP; migrar se precisar de audit trail completo
- **[Trade-off] Email service acoplado** → Usar interface IEmailService para desacoplamento futuro
