# Roadmap de Implementação - Severina AI

## Visão Geral

Este roadmap define a ordem de implementação das mudanças incrementais para o MVP do Severina AI. Cada change é uma unidade entregável que pode ser revisada e testada independentemente.

**Duração estimada total:** 12-14 semanas (3 meses)

**Fonte de verdade para UI:** Protótipos do Stitch (`stitch-prototypes/`)

---

## Protótipos do Stitch (Referência)

| Protótipo | Arquivo | Screen ID | Dispositivo |
|-----------|---------|-----------|-------------|
| Login | `stitch-prototypes/login.html` | `3c86564ba69c4c5da5933231ff03a2f6` | Mobile (780x1768) |
| Dashboard | `stitch-prototypes/dashboard.html` | `4de2ea5e16204529990b8e6671a365fb` | Mobile (780x3176) |
| Clientes | `stitch-prototypes/clientes.html` | `92e0a7df51e54e1d99f3a065833dbd54` | Mobile (780x2470) |
| Agenda | `stitch-prototypes/agenda.html` | `fe120f54a4584ad7b407e067ef64ba97` | Mobile (780x2346) |
| Financeiro | `stitch-prototypes/financeiro.html` | `1a23804950da41dca1ccd660f0c19bea` | Mobile (780x4158) |

---

## Design Tokens Comuns (Extraídos do Stitch)

```javascript
// Tailwind Config
theme.extend.colors = {
  "primary": "#004ac6",
  "primary-container": "#2563eb",
  "surface": "#faf8ff",
  "on-surface": "#131b2e",
  "outline-variant": "#c3c6d7",
  "success": "#16a34a",
  "warning": "#d97706",
  "error": "#ba1a1a",
  "info": "#0284c7"
}

theme.extend.borderRadius = {
  "pill": "9999px"  // Botões primary
}

theme.extend.fontFamily = {
  "mono": ["JetBrains Mono"]  // Valores financeiros
}
```

---

## Fase 1: Fundação (Semanas 1-3)

### CHG-01: Fundação do Projeto e Autenticação
- **Status:** `proposta`
- **Dependências:** Nenhuma
- **Protótipo:** `login.html`
- **Entregáveis:**
  - Projeto Next.js com TypeScript, Tailwind, ESLint
  - Projeto ASP.NET Core 8 com Clean Architecture
  - PostgreSQL com EF Core e migrations
  - Autenticação JWT (login, refresh, logout)
  - Tela de Login conforme protótipo Stitch
  - Toggle de tema com micro-interações

---

## Fase 2: Gestão Base (Semanas 4-5)

### CHG-02: Gestão de Empresa e Usuários
- **Status:** `proposta`
- **Dependências:** CHG-01
- **Protótipo:** `dashboard.html` (sidebar "Configurações")
- **Entregáveis:**
  - Endpoints de empresa (GET/PUT)
  - Endpoints de preferências (GET/PUT)
  - Tela INT-07 (Configurações)
  - Controle de acesso por papel

---

## Fase 3: CRM (Semanas 6-7)

### CHG-03: Gestão de Clientes
- **Status:** `proposta`
- **Dependências:** CHG-01, CHG-02
- **Protótipo:** `clientes.html`
- **Entregáveis:**
  - CRUD de clientes
  - Busca com debounce
  - Filtros e paginação
  - Tela conforme protótipo (tabela desktop / cards mobile)
  - Timeline de interações
  - Exportação CSV

---

## Fase 4: Agendamento (Semanas 8-10)

### CHG-04: Agendamento e Compromissos
- **Status:** `proposta`
- **Dependências:** CHG-01, CHG-02, CHG-03
- **Protótipo:** `agenda.html`
- **Entregáveis:**
  - CRUD de compromissos
  - Validação de conflito
  - Modal conforme protótipo
  - DateTime Picker
  - Lembretes automáticos

---

## Fase 5: Financeiro (Semanas 11-12)

### CHG-05: Cobranças e Financeiro
- **Status:** `proposta`
- **Dependências:** CHG-01, CHG-02, CHG-03, CHG-04
- **Protótipo:** `financeiro.html`
- **Entregáveis:**
  - CRUD de cobranças
  - Cards de indicadores (Bento style)
  - Tabela conforme protótipo
  - Lembretes via WhatsApp
  - Relatório PDF

---

## Fase 6: Dashboard e Atendimento (Semanas 13-14)

### CHG-06: Dashboard, Analytics e Atendimento
- **Status:** `proposta`
- **Dependências:** CHG-01, CHG-02, CHG-03, CHG-04, CHG-05
- **Protótipo:** `dashboard.html` (completo)
- **Entregáveis:**
  - Dashboard INT-02 completo conforme protótipo
  - Sidebar responsiva
  - Cards de métricas Bento style
  - Conversas Recentes com badges
  - Floating AI card com ping animation
  - BottomNavBar mobile
  - Atendimento INT-05

---

## Mapa de Dependências

```
CHG-01 (Fundação) ← login.html
  ├── CHG-02 (Empresa/Usuários) ← dashboard.html (sidebar)
  │     ├── CHG-03 (Clientes) ← clientes.html
  │     │     ├── CHG-04 (Agendamento) ← agenda.html
  │     │     │     └── CHG-05 (Financeiro) ← financeiro.html
  │     │     │           └── CHG-06 (Dashboard) ← dashboard.html (completo)
```

---

## Critérios de Aprovação por Fase

Cada fase deve atender:

1. **Qualidade:**
   - Cobertura de testes: 80% backend, 70% frontend
   - Todos os testes unitários, integração e E2E passando
   - Linting sem erros (ESLint + dotnet format)

2. **Fidelidade ao Protótipo:**
   - UI exatamente conforme protótipo Stitch
   - Design tokens extraídos do protótipo
   - Micro-interações implementadas
   - Responsividade (mobile/tablet/desktop)

3. **Funcionalidade:**
   - Todos os critérios de conclusão do proposal.md atendidos
   - Happy path, sad path e edge cases cobertos

4. **Revisão:**
   - Code review aprovado
   - Documentação atualizada

---

## Marcos

| Marco | Fase | Entrega | Protótipo | Prazo |
|-------|------|---------|-----------|-------|
| M1 | 1 | Autenticação funcional | login.html | Semana 3 |
| M2 | 2 | Gestão de empresa/usuários | dashboard.html (sidebar) | Semana 5 |
| M3 | 3 | CRM funcional | clientes.html | Semana 7 |
| M4 | 4 | Agendamento funcional | agenda.html | Semana 10 |
| M5 | 5 | Financeiro funcional | financeiro.html | Semana 12 |
| M6 | 6 | MVP completo | dashboard.html (completo) | Semana 14 |

---

## Notas

- **Prioridade:** Cada fase deve ser concluída antes da próxima iniciar
- **Protótipos:** Sempre referenciar o HTML do Stitch antes de implementar
- **Testes:** Testes são implementados junto com o código, não no final
- **Revisão:** Code review obrigatório antes de merge para main
- **Deploy:** Cada fase pode ser deployada em staging para validação
