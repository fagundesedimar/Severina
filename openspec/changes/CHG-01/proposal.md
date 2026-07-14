# CHG-01: Fundação do Projeto e Autenticação

## Status
`proposta` | `em_andamento` | `concluida`

## Resumo
Configurar a estrutura base do projeto full-stack (Next.js + ASP.NET Core), incluindo autenticação JWT, multi-tenant lógico com `company_id`, e a tela de login/cadastro (INT-01).

## Protótipo de Referência
- **Arquivo:** `stitch-prototypes/login.html`
- **Tela:** Login - Severina AI (Português)
- **Dispositivo:** Mobile (780x1768)
- **Screen ID:** `projects/2167347516013784741/screens/3c86564ba69c4c5da5933231ff03a2f6`

## Componentes do Protótipo (Login)

### Estrutura HTML
```
login-container
├── header (logo + theme toggle)
│   ├── theme-toggle (light_mode/dark_mode icon)
│   └── Logo (clinical_notes icon + "Severina AI")
├── welcome-message
│   ├── h2: "Bem-vindo de volta"
│   └── p: "Acesse para gerenciar sua secretária virtual"
├── form#login-form
│   ├── email-field (label + input + placeholder)
│   ├── password-field (label + input + toggle visibility)
│   │   └── "Esqueceu a senha?" link
│   ├── button[type=submit]: "Entrar" (pill, primary)
│   ├── divider: "OU"
│   └── google-login-button (SVG + "Continuar com Google")
└── footer: "Não tem uma conta? Cadastre-se"
```

### Design Tokens Extraídos
| Token | Valor | Uso |
|-------|-------|-----|
| `primary` | #004ac6 | Cor primária (botões, links) |
| `primary-container` | #2563eb | Fundo botão login |
| `surface` | #faf8ff | Fundo da página |
| `on-surface` | #131b2e | Texto principal |
| `outline-variant` | #c3c6d7 | Bordas de inputs |
| `surface-container` | #eaedff | Fundo inputs dark |

### Micro-interações
- **Theme Toggle:** Animação de rotação (rotate 180deg + scale 0.8→1)
- **Password Toggle:** ícone visibility/visibility_off com scale feedback
- **Submit Button:** Loading spinner (animate-spin) + "Autenticando..."
- **Button Active:** `transform: scale(0.95)`

### Tailwind Config
```javascript
darkMode: "class"
theme.extend.colors.primary = "#004ac6"
theme.extend.colors.primary-container = "#2563eb"
theme.extend.borderRadius.full = "9999px" // pill buttons
```

## Escopo Funcional
- Inicialização do projeto Next.js com TypeScript, Tailwind CSS, ESLint
- Inicialização do projeto ASP.NET Core 8 com Clean Architecture
- Configuração do PostgreSQL com Entity Framework Core e migrations
- Implementação de autenticação JWT (login, refresh token, logout)
- Implementação de autorização multi-tenant com claims `company_id`
- Tela de Login (INT-01) conforme protótipo Stitch
- Toggle de tema (claro/escuro/sistema) com persistência localStorage + API

## RF Relacionados
- RF-001: Cadastro de Empresa
- RF-008: Preferências de Tema (Dark Mode)

## RNF Relacionados
- RNF-001: Desempenho (< 200ms leitura)
- RNF-002: Segurança (JWT, criptografia)
- RNF-007: Testabilidade (80% backend, 70% frontend)
- RNF-009: Modo claro/escuro

## Dependências
- Nenhuma (change inicial)

## Riscos
- Configuração incorreta de multi-tenant pode vazar dados entre empresas
- JWT sem refresh token pode causar experiência ruim com sessões curtas

## Tamanho Estimado
- **Complexidade**: Média
- **Esforço**: 2-3 semanas
- **Risco**: Médio

## Critérios de Conclusão
- [ ] Projeto Next.js compila sem erros
- [ ] Projeto ASP.NET Core compila sem erros
- [ ] EF Core migrations criadas para entidades base (Empresa, Usuario)
- [ ] Endpoint POST /api/v1/auth/login retorna JWT válido
- [ ] Endpoint POST /api/v1/auth/refresh renova token
- [ ] Tela INT-01 renderiza conforme protótipo Stitch (login.html)
- [ ] Toggle de tema funciona com micro-interações do protótipo
- [ ] Testes unitários de autenticação passam
- [ ] Testes de integração de login passam
- [ ] Teste E2E de fluxo login → dashboard passa

## Testes Obrigatórios

### Unitários (xUnit backend, Jest frontend)
- Validação de formato de email
- Validação de força de senha
- Geração e validação de JWT
- Claims de multi-tenant
- Hooks de autenticação (useAuth)

### Integração (xUnit + TestContainers)
- POST /api/v1/auth/login com credenciais válidas/inválidas
- POST /api/v1/auth/refresh com token válido/expirado
- Migração de banco aplicada com sucesso

### E2E (Playwright)
- Fluxo completo: cadastro → login → redirecionamento para dashboard
- Toggle de tema persiste entre reloads
