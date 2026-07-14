# CHG-06: Dashboard, Analytics e Atendimento

## Status
`proposta` | `em_andamento` | `concluida`

## Resumo
Implementar o Dashboard principal (INT-02), Painel de Atendimento Omnichannel (INT-05) e endpoints de analytics/relatórios.

## Protótipo de Referência
- **Arquivo:** `stitch-prototypes/dashboard.html`
- **Tela:** Dashboard - Severina AI (Português)
- **Dispositivo:** Mobile (780x3176)
- **Screen ID:** `projects/2167347516013784741/screens/4de2ea5e16204529990b8e6671a365fb`

## Componentes do Protótipo (Dashboard)

### Estrutura HTML
```
dashboard-container
├── topbar (mobile)
│   ├── user-avatar
│   ├── title: "Severina AI"
│   └── actions (theme-toggle, notifications)
├── sidebar (desktop)
│   ├── logo + theme-toggle
│   ├── nav
│   │   ├── a: "Início" (home) [ativo]
│   │   ├── a: "Clientes" (group)
│   │   ├── a: "Agenda" (calendar_today)
│   │   ├── a: "Financeiro" (payments)
│   │   ├── div: "SISTEMA" (overline)
│   │   └── a: "Configurações" (settings)
│   └── user-profile-card
│       ├── avatar
│       ├── name: "Maria Severina"
│       └── role: "Administradora"
├── main-content
│   ├── welcome-section
│   │   ├── h1: "Visão Geral do Negócio"
│   │   └── p: "Sua assistente de IA processou 24 novas consultas hoje."
│   ├── stats-grid (3 cards bento)
│   │   ├── card: AGENDAMENTOS (42, +12%, progress bar 75%)
│   │   ├── card: RECEITA DIÁRIA ($1.280, +5%, progress bar 50%)
│   │   └── card: TAXA DE RESPOSTA (99,8%, Estável, progress bar 100%)
│   ├── two-column-layout
│   │   ├── section: "Conversas Recentes"
│   │   │   ├── conversation-card (John Doe, WhatsApp, IA PROCESSOU)
│   │   │   ├── conversation-card (Alice Smith, Webchat, AÇÃO NECESSÁRIA)
│   │   │   └── conversation-card (Mike Knight, WhatsApp)
│   │   └── section: "Próximos Compromissos"
│   │       └── table (CLIENTE, SERVIÇO, HORA, STATUS)
│   └── floating-ai-card
│       ├── icon: smart_toy (com ping animation)
│       ├── text: "Severina IA Ativa"
│       └── subtext: "Gerenciando 4 chats ativos"
└── bottomnav (mobile)
```

### Design Tokens Extraídos

#### Cores de Status
| Token | Valor | Uso |
|-------|-------|-----|
| `green-500` | #22c55e | Status online (ponto verde) |
| `green-100/700` | bg/text | Badge "Confirmado" |
| `blue-100/700` | bg/text | Badge "Pendente IA" |
| `surface-container-highest` | #dae2fd | Badge "Lista de Espera" |

#### Badges
```html
<span class="badge-success">Confirmado</span>
<span class="badge-info">Pendente IA</span>
<span class="badge-warning">Lista de Espera</span>
<span class="badge-primary">IA PROCESSOU</span>
<span class="badge-outline">AÇÃO NECESSÁRIA</span>
```

#### Cards de Métricas (Bento Style)
```html
<div class="stat-card">
  <div class="absolute-icon">calendar_month</div>
  <span class="overline">AGENDAMENTOS</span>
  <div class="value-row">
    <span class="headline-lg">42</span>
    <span class="trending-up">+12%</span>
  </div>
  <div class="progress-bar" style="width: 75%"></div>
  <span class="caption">75% da meta mensal atingida</span>
</div>
```

#### Conversas Recentes
```html
<div class="conversation-card">
  <div class="avatar-with-status">
    <div class="avatar">JD</div>
    <div class="status-dot online"></div>
  </div>
  <div class="content">
    <div class="header">
      <span class="name">John Doe</span>
      <span class="time">2m atrás</span>
    </div>
    <p class="message">"Gostaria de remarcar meu horário..."</p>
    <div class="badges">
      <span class="badge-channel">WhatsApp</span>
      <span class="badge-status">IA PROCESSOU</span>
    </div>
  </div>
</div>
```

#### Floating AI Card
```html
<div class="fixed bottom-24 right-6">
  <div class="ai-card">
    <div class="icon-wrapper">
      <span class="material-symbols-outlined">smart_toy</span>
      <div class="ping-animation"></div>
    </div>
    <div class="text">
      <p class="label">Severina IA Ativa</p>
      <p class="subtext">Gerenciando 4 chats ativos</p>
    </div>
  </div>
</div>
```

### Micro-interações
- **Card Hover:** `group-hover:scale-110` no ícone de fundo
- **Conversation Card:** `hover:bg-surface-container-low` + cursor pointer
- **Chevron Right:** `opacity-0 group-hover:opacity-100`
- **AI Card:** `active:scale-95` + ping animation no ponto verde
- **BottomNav Item:** `active:scale-95` + transition

### BottomNavBar (Mobile)
```html
<nav class="fixed bottom-0">
  <a class="active">Início</a>
  <a>Clientes</a>
  <a>Agenda</a>
  <a>Financeiro</a>
</nav>
```

## Escopo Funcional
- Endpoint GET /api/v1/analytics/reports (métricas por período)
- Dashboard INT-02 conforme protótipo completo
- Sidebar fixa (240px desktop, 64px colapsada, drawer mobile)
- Topbar com avatar, toggle tema, notificações
- Cards de indicadores (Bento style) com progress bars
- Conversas Recentes com badges de canal e status
- Próximos Compromissos em tabela
- Floating AI card com ping animation
- BottomNavBar mobile com 4 itens
- Atendimento INT-05 com layout 2 colunas
- Timeline de mensagens (bolhas)
- Campo de entrada com sugestões de IA

## RF Relacionados
- RF-003: Integração WhatsApp
- RF-006: Dashboard de Insights

## Dependências
- CHG-01 (Autenticação)
- CHG-02 (Gestão de empresa/usuários)
- CHG-03 (Gestão de clientes)
- CHG-04 (Agendamentos)
- CHG-05 (Cobranças)

## Riscos
- Integração WhatsApp pode não estar pronta para MVP
- Performance da timeline pode degradar com muitas mensagens

## Tamanho Estimado
- **Complexidade**: Alta
- **Esforço**: 3 semanas
- **Risco**: Médio-Alto

## Critérios de Conclusão
- [ ] Dashboard INT-02 renderiza conforme protótipo (dashboard.html)
- [ ] Sidebar funciona (expandida, colapsada, drawer mobile)
- [ ] Cards de indicadores (Bento style) com progress bars
- [ ] Conversas Recentes exibem badges de canal/status
- [ ] Próximos Compromissos em tabela
- [ ] Floating AI card com ping animation
- [ ] BottomNavBar mobile funciona
- [ ] Atendimento INT-05 renderiza layout 2 colunas
- [ ] Timeline de mensagens funciona
- [ ] Responsividade funciona (mobile/tablet/desktop)
- [ ] Testes unitários de métricas passam
- [ ] Testes de integração de endpoints passam
- [ ] Testes E2E de fluxos principais passam

## Testes Obrigatórios

### Unitários
- Cálculo de métricas do dashboard
- Formatação de timestamps relativos
- Lógica de paginação de conversas

### Integração
- GET /api/v1/analytics/reports com filtros
- Listagem de conversas
- Envio de mensagem

### E2E
- Fluxo: Login → Dashboard → Ver indicadores
- Fluxo: Dashboard → Atendimento → Responder mensagem
- Responsividade: teste em diferentes viewports
