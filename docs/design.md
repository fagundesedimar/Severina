# Severina AI - Design System

## Overview

O design system do Severina AI é construído sobre uma filosofia de **clareza funcional com presence discreta**. Assim como a Apple enquadra seus produtos com UI quase invisível, o Severina AI emprega uma abordagem onde a **interface recua para que a informação e a ação do usuário fiquem em primeiro plano**.

Cada tela é uma composição de cards limpos, espaçamento generoso e hierarquia tipográfica clara. Nada compete com o conteúdo funcional — o usuário sempre sabe onde está, o que pode fazer e qual é o próximo passo.

O sistema é projetado para **empreendedores com baixa familiaridade tecnológica**, portanto a densidade visual é moderada: cada seção ocupa espaço suficiente para respirar, ações primárias são óbvias, e a linguagem visual comunica **profissionalismo, confiabilidade e simplicidade**.

**Características-chave:**
- Funcional-first: cada pixel serve a uma ação ou informação. Sem decoração.
- Cards limpos com bordas sutis e sem sombras desnecessárias — elevation vem da superfície, não de efeitos.
- Único accent color: azul severina (`{colors.primary}`) carrega todos os elementos interativos.
- Duas gramáticas de botão: pill primary para CTAs e rect compacto para ações secundárias.
- Inter 400/500/600 como fam tipográfica (substituto open-source de SF Pro).
- Ritmo previsível: nav → content cards → footer. O usuário nunca se perde.
- Superfícies alternadas: light canvas ↔ subtle off-white para criar ritmo sem bordas.

---

## Colors

### Brand & Accent

| Token | Hex | Uso |
|---|---|---|
| `{colors.primary}` | #2563eb | A cor interativa universal. Links, botões primary, focus ring, badges ativos. Todo "clica aqui" é azul. |
| `{colors.primary-hover}` | #1d4ed8 | Estado hover de todos os elementos primary. |
| `{colors.primary-focus}` | #3b82f6 | Anel de foco por teclado (`outline: 2px solid`). |
| `{colors.primary-light}` | #dbeafe | Fundo de badges ativos, highlights de seleção, estados de sucesso sutis. |
| `{colors.primary-on-dark}` | #60a5fa | Links e CTAs em superfícies escuras (sidebar collapsed, modais escuros). |

### Surface

| Token | Hex | Uso |
|---|---|---|
| `{colors.canvas}` | #ffffff | Canvas dominante. Cards, formulários, conteúdo principal. |
| `{colors.canvas-subtle}` | #f8fafc | Fundo de sidebar, headers de seção, áreas de suporte visual. |
| `{colors.canvas-muted}` | #f1f5f9 | Fundo de estados vazios, skeletons, áreas de carregamento. |
| `{colors.surface-card}` | #ffffff | Fundo de cards de conteúdo. Borda sutil define a elevação. |
| `{colors.surface-card-hover}` | #f8fafc | Estado hover de cards clicáveis. |
| `{colors.surface-elevated}` | #ffffff | Modais, dropdowns, popovers — componentes浮浮 sobre o conteúdo. |
| `{colors.surface-overlay}` | rgba(0, 0, 0, 0.5) | Backdrop de modais e drawers. |

### Text

| Token | Hex | Uso |
|---|---|---|
| `{colors.ink}` | #0f172a | Headlines, corpo de texto, labels — a voz primária. |
| `{colors.ink-secondary}` | #475569 | Texto secundário, descrições, hints. |
| `{colors.ink-muted}` | #94a3b8 | Placeholders, textos desabilitados, timestamps. |
| `{colors.ink-on-primary}` | #ffffff | Texto sobre fundos primary (botões, badges). |
| `{colors.ink-on-dark}` | #f8fafc | Texto sobre superfícies escuras (sidebar collapsed). |

### Status

| Token | Hex | Uso |
|---|---|---|
| `{colors.success}` | #16a34a | Status "pago", "confirmado", "ativo". |
| `{colors.success-light}` | #dcfce7 | Fundo de badges de sucesso. |
| `{colors.warning}` | #d97706 | Status "pendente", "próximo do vencimento". |
| `{colors.warning-light}` | #fef3c7 | Fundo de badges de warning. |
| `{colors.error}` | #dc2626 | Status "atrasado", "erro", "rejeitado". Erros de validação. |
| `{colors.error-light}` | #fee2e2 | Fundo de badges de erro, borda de inputs com erro. |
| `{colors.info}` | #0284c7 | Status "em processamento", "enviado". |
| `{colors.info-light}` | #e0f2fe | Fundo de badges informativos. |

### Borders & Dividers

| Token | Value | Uso |
|---|---|---|
| `{colors.border}` | #e2e8f0 | Bordas de cards, inputs, separadores. |
| `{colors.border-strong}` | #cbd5e1 | Bordas em foco, bordas de tabs ativos. |
| `{colors.divider}` | #f1f5f9 | Separadores sutis entre linhas de tabela, items de lista. |

---

## Typography

### Font Family

- **Primary**: `Inter, system-ui, -apple-system, BlinkMacSystemFont, sans-serif`
- **Monospace**: `JetBrains Mono, ui-monospace, monospace` — para valores financeiros, IDs, snippets de código.

### Hierarchy

| Token | Size | Weight | Line Height | Letter Spacing | Uso |
|---|---|---|---|---|---|
| `{typography.h1}` | 30px | 600 | 1.2 | -0.02em | Título da página. Raro — usado em dashboard principal e landing. |
| `{typography.h2}` | 24px | 600 | 1.3 | -0.01em | Títulos de seção (Agenda, Clientes, Financeiro). |
| `{typography.h3}` | 20px | 600 | 1.4 | 0 | Títulos de subsection, cards grandes. |
| `{typography.h4}` | 16px | 600 | 1.5 | 0 | Títulos de componentes internos, headers de tabela. |
| `{typography.body-lg}` | 16px | 400 | 1.6 | 0 | Corpo de texto em contexto de leitura (descrições longas). |
| `{typography.body}` | 14px | 400 | 1.5 | 0 | Corpo padrão. Labels, descrições curtas, texto de interface. |
| `{typography.body-sm}` | 13px | 400 | 1.4 | 0.01em | Texto compacto: timestamps, metadados, badges. |
| `{typography.caption}` | 12px | 400 | 1.4 | 0.02em | Captions, hints, textos auxiliares. |
| `{typography.caption-strong}` | 12px | 600 | 1.4 | 0.02em | Labels de status, badges. |
| `{typography.button}` | 14px | 500 | 1.0 | 0.01em | Botões primary e secondary. |
| `{typography.button-sm}` | 12px | 500 | 1.0 | 0.02em | Botões compactos, inline actions. |
| `{typography.nav-link}` | 14px | 500 | 1.0 | 0.01em | Links de navegação sidebar. |
| `{typography.nav-link-active}` | 14px | 600 | 1.0 | 0.01em | Link de nav ativo. |
| `{typography.mono}` | 13px | 400 | 1.5 | 0 | Valores financeiros, IDs, números de telefone formatados. |
| `{typography.overline}` | 11px | 600 | 1.5 | 0.08em | Overlines de seção (todas uppercase). |

### Principles

- **Peso 500 para ações, 600 para hierarquia.** Botões e links usam weight 500 (leve, confiante). Headlines usam 600 (assertivo, mas não pesado).
- **Body a 14px, não 16px.** Em dashboards SaaS, 14px é o padrão de leitura funcional — permite mais informação por tela sem sacrificar legibilidade.
- **Letter-spacing positivo em captions e overlines.** Em tamanhos menores que 13px, tracking positivo melhora a legibilidade.
- **Letter-spacing negativo em h1 e h2.** Para títulos grandes, tracking sutil negativo cria presença e sofisticação.
- **Line-height context-specific.** Títulos usam 1.2–1.4. Corpo usa 1.5. Dense lists (tabelas) usam 1.4. Nunca abaixo de 1.2 para corpo.
- **JetBrains Mono para dados.** Números em fonte monospaced garantem alinhamento vertical em colunas de tabela e valores financeiros.

### Font Substitutes

- **Inter** é a escolha padrão — mais close de SF Pro para interfaces SaaS.
- Para ambientes sem acesso a Google Fonts, usar `system-ui, -apple-system, BlinkMacSystemFont` que resolve para SF Pro em Apple e Segoe UI em Windows.
- **JetBrains Mono** pode ser substituído por `ui-monospace, SFMono-Regular, monospace`.

---

## Layout

### Spacing System

- **Base unit:** 4px.
- **Tokens:** `{spacing.xxs}` 2px · `{spacing.xs}` 4px · `{spacing.sm}` 8px · `{spacing.md}` 12px · `{spacing.lg}` 16px · `{spacing.xl}` 24px · `{spacing.xxl}` 32px · `{spacing.section}` 48px.
- **Content padding:** `{spacing.xl}` (24px) inside cards; `{spacing.xxl}` (32px) para seções de página.
- **Component spacing:** `{spacing.md}` (12px) entre elementos inline; `{spacing.lg}` (16px) entre blocos.
- **Section vertical:** `{spacing.section}` (48px) entre seções maiores.

### Grid & Container

- **Max content width:** 1200px para conteúdo principal; 1440px para dashboards com sidebar.
- **Sidebar width:** 240px expandida · 64px colapsada.
- **Column patterns:**
  - Dashboard: sidebar (240px) + conteúdo fluido (restante).
  - Tabelas: colunas proporcionais, mínimas 120px.
  - Cards grid: 1 coluna (mobile) → 2 colunas (tablet) → 3-4 colunas (desktop).
  - Formulários: 1 coluna (mobile) → 2 colunas (desktop, campos alinhados).
- **Gutters:** 16px entre cards em grid; 24px entre seções de formulário.

### Whitespace Philosophy

O whitespace do Severina AI é **proporcional à importância da informação**. Seções de leitura (dashboard, relatórios) recebem mais ar. Seções de ação (formulários, tabelas) são mais densas mas nunca apertadas. O usuário nunca sente que a tela está "lotada" — sempre há espaço para respirar entre blocos de informação.

---

## Elevation & Depth

| Level | Treatment | Use |
|---|---|---|
| Flat | No shadow, no border | Fundo de página, containers de seção. |
| Subtle | 1px `{colors.border}` border | Cards padrão, inputs, containers de conteúdo. |
| Elevated | 0 1px 3px rgba(0, 0, 0, 0.1) | Modais, dropdowns, popovers — flutuam sobre o conteúdo. |
| Floating | 0 4px 6px rgba(0, 0, 0, 0.1) | Tooltips, notifications flutuantes, drag handles. |

**Filosofia de elevation.** O Severina AI usa **elevation mínima**. A maioria dos componentes usa apenas borda sutil para definir limites. Sombra é reservada para elementos que literalmente flutuam sobre o conteúdo (modais, dropdowns, notificações). Nunca sombra em cards de dados, botões ou texto.

---

## Shapes

### Border Radius Scale

| Token | Value | Use |
|---|---|---|
| `{rounded.none}` | 0px | Tabelas, dividers, elementos full-bleed. |
| `{rounded.sm}` | 4px | Badges inline, tags, chips compactos. |
| `{rounded.md}` | 6px | Inputs, botões secondary, cards de conteúdo. |
| `{rounded.lg}` | 8px | Cards de dashboard, modais, dropdowns. |
| `{rounded.xl}` | 12px | Cards de feature, hero cards. |
| `{rounded.pill}` | 9999px | Botões primary, badges de status, avatares pequenos. |
| `{rounded.full}` | 50% | Avatares circulares, indicators. |

### Photography & Imagery

- **Avatares:** circular `{rounded.full}`, 32px (nav), 40px (cards), 48px (perfis).
- **Ilustrações de estado vazio:** SVG inline ou imagem estática, fundo `{colors.canvas-muted}`, centralizada com `{spacing.xxl}` padding.
- **Ícones:** Heroicons (outline para nav, solid para status). Tamanho padrão: 20px. Variants: 16px (inline), 24px (feature).

---

## Components

### Navigation

**`sidebar`** — Navegação lateral fixa. Background `{colors.canvas-subtle}`, width 240px expandida / 64px colapsada. Itens de nav em `{typography.nav-link}` (14px / 500). Item ativo: background `{colors.primary-light}`, text `{colors.primary}`, font-weight 600. Mobile: drawer overlay com backdrop `{colors.surface-overlay}`.

**`topbar`** — Header horizontal. Background `{colors.canvas}`, height 56px, padding `{spacing.xl}` horizontal. Left: breadcrumbs ou título da página. Right: avatar do usuário, ícone de notificações com badge, botão de configurações. Border-bottom: 1px `{colors.border}`.

**`breadcrumb`** — `{typography.caption}` (12px / 400), `{colors.ink-secondary}`. Separador: ícone chevron-right 14px. Último item: `{colors.ink}`, font-weight 500.

**`tabs`** — Navegação por abas dentro de uma seção. Background transparente. Tab items: `{typography.body}` (14px / 400), padding 8px 16px, border-bottom 2px transparent. Tab ativa: border-bottom 2px `{colors.primary}`, text `{colors.primary}`, font-weight 500. Tab hover: text `{colors.ink}`.

### Buttons

**`button-primary`** — A ação principal. Background `{colors.primary}` (#2563eb), text `{colors.ink-on-primary}` (white), `{typography.button}` (14px / 500), rounded `{rounded.pill}`, padding 10px 20px. Hover: `{colors.primary-hover}`. Focus: 2px solid `{colors.primary-focus}`. Active: `transform: scale(0.98)`. Disabled: opacity 0.5, cursor not-allowed.

**`button-secondary`** — Ação secundária. Background transparent, text `{colors.primary}`, 1px solid `{colors.border}`, `{typography.button}` (14px / 500), rounded `{rounded.md}`, padding 10px 20px. Hover: background `{colors.canvas-subtle}`. Focus: 2px solid `{colors.primary-focus}`.

**`button-ghost`** — Ação terciária. Background transparent, text `{colors.ink-secondary}`, `{typography.button}` (14px / 500), rounded `{rounded.md}`, padding 8px 16px. Hover: background `{colors.canvas-subtle}`, text `{colors.ink}`.

**`button-danger`** — Ação destrutiva. Background `{colors.error}`, text white, `{typography.button}` (14px / 500), rounded `{rounded.md}`, padding 10px 20px. Hover: darken 10%. Focus: 2px solid `{colors.error}`.

**`button-icon`** — Ação sem label. 36x36px, background transparent, text `{colors.ink-secondary}`, rounded `{rounded.md}`. Hover: background `{colors.canvas-subtle}`. Used for table row actions, inline controls.

**`button-sm`** — Variante compacta. Same styles as primary/secondary/ghost but padding 6px 12px, `{typography.button-sm}` (12px / 500). For inline actions, table cells, tight spaces.

### Cards

**`card`** — Container padrão. Background `{colors.surface-card}`, 1px solid `{colors.border}`, rounded `{rounded.lg}` (8px), padding `{spacing.xl}` (24px). No shadow by default. Used for dashboard widgets, content blocks, data displays.

**`card-interactive`** — Card clicável. Same as `{component.card}` but with hover state: background `{colors.surface-card-hover}`, cursor pointer. Used for navigation cards, selectable items.

**`card-stat`** — Widget de métrica. Same as `{component.card}` with internal layout: overline label in `{typography.overline}` (11px / 600), large value in `{typography.h1}` (30px / 600), change indicator below (green up / red down arrow + percentage).

**`card-empty`** — Estado vazio. Background `{colors.canvas-muted}`, border dashed `{colors.border}`, rounded `{rounded.lg}`, padding `{spacing.xxl}` (32px). Centered content: illustration/icon + message + action button.

### Forms

**`input`** — Campo de texto. Background `{colors.canvas}`, border 1px `{colors.border}`, rounded `{rounded.md}` (6px), padding 10px 12px, `{typography.body}` (14px / 400), height 40px. Focus: border `{colors.primary}`, 2px solid `{colors.primary-focus}`. Error: border `{colors.error}`, 2px solid `{colors.error-light}`.

**`input-label`** — Label do campo. `{typography.body}` (14px / 500), color `{colors.ink}`, margin-bottom 6px. Required: asterisco `{colors.error}`.

**`input-hint`** — Texto auxiliar. `{typography.caption}` (12px / 400), color `{colors.ink-muted}`, margin-top 4px.

**`input-error`** — Mensagem de erro. `{typography.caption}` (12px / 400), color `{colors.error}`, margin-top 4px. Ícone de alerta 14px inline.

**`select`** — Dropdown. Same as `{component.input}` with chevron-down icon right-aligned. Options: same typography, selected state with `{colors.primary-light}` background.

**`checkbox`** — Caixa de seleção. 18x18px, border 2px `{colors.border}`, rounded `{rounded.sm}` (4px). Checked: background `{colors.primary}`, border `{colors.primary}`, ícone check branco. Focus: 2px solid `{colors.primary-focus}`.

**`radio`** — Botão de rádio. 18x18px, border 2px `{colors.border}`, rounded `{rounded.full}`. Selected: border `{colors.primary}`, inner dot 8px `{colors.primary}`. Focus: 2px solid `{colors.primary-focus}`.

**`textarea`** — Área de texto. Same as `{component.input}` with min-height 80px, resize vertical.

**`toggle`** — Switch de alternância. 44x24px, track `{colors.border}`, thumb 18x18px white. Checked: track `{colors.primary}`, thumb slides right. Focus: 2px solid `{colors.primary-focus}`.

**`form-group`** — Wrapper de campo. Margin-bottom `{spacing.xl}` (24px) between fields. Contains label + input + hint/error.

**`form-row`** — Campos lado a lado. Flex row with `{spacing.lg}` (16px) gap. On mobile: stacks to single column.

### Tables

**`table`** — Tabela de dados. Background `{colors.canvas}`, border-collapse collapse. Header row: background `{colors.canvas-subtle}`, text `{colors.ink-secondary}`, `{typography.caption-strong}` (12px / 600), text-transform uppercase, letter-spacing 0.05em. Body rows: border-bottom 1px `{colors.divider}`, text `{typography.body}` (14px / 400). Row hover: background `{colors.canvas-subtle}`. Selected row: background `{colors.primary-light}`.

**`table-cell`** — Célula padrão. Padding 12px 16px, vertical-align middle. Monospace values: `{typography.mono}` (13px).

**`table-actions`** — Coluna de ações. Right-aligned, `{component.button-icon}` buttons. Visible on row hover (desktop) or always visible (mobile).

**`table-pagination`** — Paginação inferior. Background `{colors.canvas}`, border-top 1px `{colors.border}`, padding `{spacing.lg}`. Text: `{typography.body-sm}` (13px). Page buttons: `{component.button-ghost}`.

### Status & Badges

**`badge`** — Status indicator. `{typography.caption-strong}` (12px / 600), padding 2px 8px, rounded `{rounded.pill}`. Variants: default (`{colors.canvas-subtle}`, `{colors.ink-secondary}`), success (`{colors.success-light}`, `{colors.success}`), warning (`{colors.warning-light}`, `{colors.warning}`), error (`{colors.error-light}`, `{colors.error}`), info (`{colors.info-light}`, `{colors.info}`).

**`status-dot`** — Indicador de status inline. 8x8px circle, filled with status color. Used in tables, timelines, lists.

### Data Display

**`metric`** — Exibição de métrica. Layout vertical: label in `{typography.overline}`, value in `{typography.h2}` or `{typography.h3}`, subtext in `{typography.caption}` with color `{colors.ink-secondary}`.

**`timeline`** — Linha do tempo de interações. Vertical line 2px `{colors.border}` on left, events as nodes (12px circle) with content cards to the right. Event timestamp: `{typography.caption}`. Event content: `{typography.body}`. Event type icon: 16px, colored by type.

**`progress-bar`** — Barra de progresso. Height 6px, rounded `{rounded.pill}`, background `{colors.canvas-muted}`, fill `{colors.primary}`. Label: `{typography.caption}` above or below.

**`empty-state`** — Estado vazio. Centered layout: 64px icon (muted), heading `{typography.h3}`, description `{typography.body}` color `{colors.ink-secondary}`, action button `{component.button-primary}`.

### Feedback

**`toast`** — Notificação temporária. Background `{colors.surface-elevated}`, border 1px `{colors.border}`, rounded `{rounded.lg}`, padding `{spacing.lg}`, shadow elevated. Left: status icon (16px, colored). Right: close button. Position: bottom-right, auto-dismiss 5s. Variants: success, error, warning, info.

**`alert`** — Banner de status. Padding `{spacing.lg}`, rounded `{rounded.md}`, border-left 4px solid (color by variant). Background: variant light color. Icon 16px left. Text: `{typography.body}`. Variants: success, error, warning, info.

**`modal`** — Diálogo centrado. Background `{colors.surface-elevated}`, rounded `{rounded.lg}`, shadow elevated, max-width 480px (sm) / 640px (md) / 800px (lg). Backdrop: `{colors.surface-overlay}`. Header: `{typography.h3}`, close button top-right. Footer: right-aligned buttons (secondary + primary).

**`drawer`** — Painel lateral. Background `{colors.canvas}`, width 400px (sm) / 560px (md) / 720px (lg). Slide-in from right. Header with title and close button. Body scrollable. Footer with actions.

### Data Entry

**`search-input`** — Campo de busca. Same as `{component.input}` with search icon (16px) left-aligned, padding-left 36px. Rounded `{rounded.pill}`. Keyboard shortcut hint: `{typography.caption}` with ⌘K / Ctrl+K badge.

**`combobox`** — Autocomplete. Same as `{component.input}` with dropdown results list. Results: `{typography.body}`, padding 8px 12px, hover background `{colors.canvas-subtle}`, selected background `{colors.primary-light}`.

**`date-picker`** — Seletor de data. Trigger: same as `{component.input}` with calendar icon. Dropdown: calendar grid, today highlighted with `{colors.primary-light}`, selected date `{colors.primary}` filled circle.

**`file-upload`** — Upload de arquivo. Dashed border `{colors.border}`, rounded `{rounded.lg}`, padding `{spacing.xl}`, centered icon + text. Drag-over state: border `{colors.primary}`, background `{colors.primary-light}`.

### Navigation Components

**`pagination`** — Navegação de páginas. Previous/Next buttons + page numbers. Active page: `{colors.primary}` background, white text. Page numbers: `{component.button-ghost}`.

**`breadcrumb`** — Migalha de pão. `{typography.caption}`, `{colors.ink-secondary}`. Separators: chevron-right. Last item: `{colors.ink}`, font-weight 500.

**`stepper`** — Passos de processo. Horizontal: circles connected by lines. Active: `{colors.primary}` filled circle, white number. Completed: `{colors.success}` filled circle, check icon. Pending: `{colors.border}` circle, gray number. Labels below circles: `{typography.caption}`.

---

## Do's and Don'ts

### Do
- Use `{colors.primary}` (#2563eb) para **todo** elemento interativo — links, botões primary, focus signals, badges ativos. O accent é único e universal.
- Mantenha `{typography.body}` em 14px / 400 / 1.5 para todo corpo de interface. A consistência tipográfica é a base da legibilidade.
- Use `{rounded.pill}` para botões primary e badges de status — é o sinal visual de "ação" do sistema.
- Mantenha a sidebar como âncora visual — ela nunca muda de posição, apenas colapsa.
- Use `{colors.status.*}` consistentemente: verde = sucesso/pago, amarelo = pendente, vermelho = erro/atrasado, azul = info/processando.
- Aplique `transform: scale(0.98)` como estado active/pressed em botões — é o micro-interaction padrão.
- Mantenha cards com `{rounded.lg}` (8px) e sem sombra — a borda sutil define a elevação.
- Use JetBrains Mono para valores financeiros e IDs — alinhamento vertical em colunas de tabela.

### Don't
- Não introduza uma segunda cor de accent — todo "clica aqui" é `{colors.primary}`.
- Não adicione sombras em cards de dados, botões ou texto — sombra é para modais e dropdowns apenas.
- Não use gradients como backgrounds decorativos — a atmosferia vem do conteúdo, não do CSS.
- Não defina body abaixo de 14px — a legibilidade funcional é primária.
- Não misture border-radius — `{rounded.md}` para inputs, `{rounded.lg}` para cards, `{rounded.pill}` para CTAs, nada entre eles.
- Não use `{colors.error}` para warnings — erro é para falhas, warning é para pendências.
- Não coloque mais de 3 campos por linha em formulários — a densidade excessiva frustra o público-alvo.
- Não use ícones sem labels no MVP — o público-alvo precisa de clareza, não de economia de espaço.
- Não crie variações de cor para cada status — use o sistema de badges e dots, não mude o accent.

---

## Responsive Behavior

### Breakpoints

| Name | Width | Key Changes |
|---|---|---|
| Mobile | ≤ 640px | Sidebar becomes drawer overlay. Single-column layout. Tables become card lists. Forms stack vertically. |
| Tablet | 641–1024px | Sidebar visible (64px collapsed). 2-column grids. Tables with essential columns only. |
| Desktop | 1025–1440px | Full sidebar (240px). 3-4 column grids. All table columns visible. |
| Wide | ≥ 1441px | Content locks at 1200px, margins absorb extra width. |

### Touch Targets
- Minimum 44x44px for all interactive elements on mobile.
- `{component.button-primary}` on mobile: min-height 44px, full-width when primary action.
- `{component.button-icon}`: 44x44px on mobile (36x36px on desktop).

### Collapsing Strategy
- **Sidebar:** 240px (desktop) → 64px icons-only (tablet) → drawer overlay (mobile).
- **Tables:** Full columns (desktop) → essential columns only (tablet) → card list view (mobile).
- **Forms:** 2-column (desktop) → 1-column with full-width fields (mobile).
- **Dashboard grid:** 4-col (desktop) → 2-col (tablet) → 1-col (mobile).

---

## Accessibility (WCAG 2.1 AA)

### Color Contrast
- All text meets 4.5:1 contrast ratio against its background (AA standard).
- `{colors.ink}` (#0f172a) on `{colors.canvas}` (#ffffff): contrast 17.5:1.
- `{colors.ink-secondary}` (#475569) on `{colors.canvas}`: contrast 7.1:1.
- `{colors.ink-muted}` (#94a3b8) on `{colors.canvas}`: contrast 3.5:1 — use only for non-essential text (placeholders, timestamps).
- `{colors.primary}` (#2563eb) on `{colors.canvas}`: contrast 4.6:1 — meets AA for normal text.
- `{colors.ink-on-primary}` (white) on `{colors.primary}`: contrast 4.6:1 — meets AA.

### Focus Management
- All interactive elements have visible focus indicator: 2px solid `{colors.primary-focus}` with 2px offset.
- Focus order follows logical reading/navigation order (top-to-bottom, left-to-right).
- Modals trap focus and return focus to trigger element on close.
- Skip navigation link available for keyboard users.

### ARIA & Semantics
- All form inputs have associated labels (via `htmlFor` or `aria-label`).
- Error messages linked to inputs via `aria-describedby` and `aria-invalid`.
- Status badges use `aria-label` for screen reader context.
- Tables use `scope="col"` for headers and `aria-sort` for sortable columns.
- Loading states announced via `aria-live="polite"`.

### Motion
- Respect `prefers-reduced-motion` media query — disable `transform: scale()` transitions and animations.
- Transitions use `200ms ease-out` as default — short enough to feel responsive, long enough to be perceivable.

---

## Iteration Guide

1. Focus on ONE component at a time. Reference its YAML key directly (`{component.card}`, `{component.button-primary}`).
2. Variants of an existing component (`-active`, `-focus`, `-error`) live as separate entries in `components:`.
3. Use `{token.refs}` everywhere — never inline hex values.
4. Never document hover states in component specs — only default, active/pressed, and error states.
5. Headlines stay Inter 600. Body stays Inter 400 at 14px. The boundary is unbreakable.
6. The single elevation system (border → shadow for modals only) is reserved for functional depth.
7. When in doubt about emphasis: use `{colors.status.*}` badges before adding chrome.

---

## Known Gaps

- Dark mode counterparts were not defined in this version — the system documented is the light-mode dominant variant.
- Form validation and error states for complex multi-step forms need further specification.
- Empty state illustrations are placeholder — custom artwork should be commissioned for each major flow.
- Notification system UI (toast positioning, stacking behavior) needs user testing for optimal placement.
- WhatsApp integration preview (message bubbles, chat interface) is out of scope for this design system version.
- Mobile-specific interactions (swipe to delete, pull to refresh) need native testing validation.
- Print styles for invoices and reports were not documented.
- Internationalization (i18n) layout adaptations (RTL support, date/number formatting) are future considerations.
