# Prompt de Prototipagem - Severina AI

Este arquivo contém um prompt estruturado para ser utilizado em ferramentas de geração de protótipos de interface assistidas por IA (como Google Stitch, Figma GenAI ou v0.dev). O prompt atua sob o papel de um **Designer de UX Sênior** e fornece todas as regras, tokens de design, layouts de telas e fluxos de navegação definidos para a plataforma **Severina AI**.

---

## Como usar este prompt
Copie o bloco de texto abaixo e insira-o no campo de instruções da sua ferramenta de prototipagem baseada em IA para gerar os templates interativos de alta fidelidade para a **Severina AI**.

---

```text
Atue como: Designer de UX Sênior e Engenheiro de Prototipagem de UI.

Instrução: Crie um protótipo de alta fidelidade e totalmente interativo contendo templates para o projeto de software "Severina AI" (Plataforma Web responsiva com suporte desktop e layout adaptável para tablet). O design deve ser moderno, clean e profissional, aplicando um sistema de design neutro com toques de verde e azul para transmitir confiabilidade, produtividade e automação.

### 1. GUIA DE DESIGN (DESIGN SYSTEM)
- Tipografia: Fonte do Google Fonts 'Inter' ou 'Plus Jakarta Sans'. Títulos principais com peso 700 (bold), títulos secundários 500 (medium) e textos de corpo 400 (regular).
- Paleta de Cores:
  * Azul Primário: #16548a (Base séria e corporativa).
  * Verde de Ação: #1f8a5a (Botões primários e estados positivos).
  * Cinza Neutro: #f6f8fa (Fundo claro e áreas de conteúdo).
  * Cinza Escuro: #333d4b (Texto e elementos de alta prioridade).
  * Amarelo de Aviso: #f5a623 (Alertas e notificações urgentes).
- Efeito Glassmorphism: Use em cards de destaque, painéis de dashboard e modais com "backdrop-filter: blur(8px)" e borda fina semitransparente "border: 1px solid rgba(255, 255, 255, 0.12)".
- Sombra sutil: cards com sombra suave "0 12px 24px rgba(20, 30, 60, 0.08)".
- Responsividade: Layout adaptável para mobile (1 coluna), tablet (2 colunas) e desktop (sidebar lateral fixa).
- Acessibilidade: contraste de texto mínimo de 4.5:1, foco visível, labels claros e navegação por teclado.

---

### 2. ESTRUTURA DOS TEMPLATES DE TELA (INTERFACES)

Gere os templates funcionais para as seguintes 7 interfaces gráficas:

#### [INT-01] - Tela de Login e Cadastro (Autenticação)
- Contêiner: tela centralizada com fundo em gradiente suave de azul escuro para cinza claro e um painel glassmorphism.
- Estado 1 (Login):
  * Campos: E-mail (email input), Senha (password input).
  * Botões: "Entrar" (principal verde), "Criar conta" (link secundário).
  * Links: "Esqueci minha senha".
- Estado 2 (Cadastro - alterna via clique):
  * Campos adicionais: Nome da Empresa, Nome do Usuário, E-mail, Senha, Telefone, WhatsApp de Contato.
  * Botões: "Confirmar Cadastro", "Voltar para Login".

#### [INT-02] - Dashboard Empresarial
- Contêiner: layout com sidebar fixa à esquerda em azul escuro e área principal à direita com grid responsivo.
- Painel de Indicadores (Cartões):
  * Card 1: "Atendimentos em Aberto" (ex: "12")
  * Card 2: "Compromissos Hoje" (ex: "8")
  * Card 3: "Cobranças Pendentes" (ex: "R$ 4.250")
- Seção de Atividades Recentes: lista de itens com cópia de conversas, novos agendamentos e lembretes.
- Botões:
  * Botão principal "+ Novo Agendamento" (verde)
  * Botão secundário "+ Nova Cobrança" (outline)
  * Ação rápida "Abrir Atendimento" para iniciar um novo chat.

#### [INT-03] - Modal de Novo Agendamento
- Contêiner: modal centralizado com fundo desfocado atrás e painel em vidro levemente esbranquiçado.
- Campos: Cliente (autocomplete), Serviço/Tipo de Atendimento (dropdown), Data e Hora (datetime picker), Canal de atendimento (WhatsApp, Presencial, On-line), Observações (textarea).
- Botões: "Agendar" (verde), "Cancelar" (cinza).
- Comportamento: exibir alerta amarelo se o horário escolhido conflitar com outro compromisso existente.

#### [INT-04] - Gestão de Clientes
- Contêiner: tela de listagem com barra de pesquisa no topo e cards ou tabela de clientes.
- Campos principais: Nome, Telefone, Último Contato, Próximo Compromisso, Status.
- Comandos: "Adicionar Cliente" (botão), "Editar" em cada linha/cartão, "Abrir Histórico".
- Detalhe do cliente: painel lateral ou modal com histórico de conversas, agendamentos e cobranças.

#### [INT-05] - Tela de Conversa / CRM
- Contêiner: layout dividido em duas colunas: lista de clientes à esquerda e área de conversa à direita.
- Elementos: cabeçalho com nome do cliente, tags de status, histórico de mensagens e campo de entrada com ação rápida de resposta.
- Comandos: "Enviar Mensagem", "Marcar como Resolvido", "Agendar Follow-up".

#### [INT-06] - Painel Financeiro e Cobranças
- Contêiner: painel centralizado em duas seções: indicadores no topo e tabela de cobranças abaixo.
- Indicadores: "Total a Receber", "Pagos nos Últimos 30 Dias", "Cobranças Atrasadas".
- Tabela de Cobranças: colunas: Cliente, Valor, Vencimento, Status, Ações.
- Botões: "Gerar Cobrança", "Enviar Lembrete" e ações de filtro por status.

#### [INT-07] - Configurações e Preferências de Tema
- Contêiner: tela de configurações com cards para categoria de preferências.
- Campos: Toggle de tema (Claro/Escuro/Sistema), Configurações de Notificações, Integrações de WhatsApp.
- Comandos: "Salvar Preferências", "Testar Integração".

---

### 3. FLUXO DE NAVEGAÇÃO E INTERAÇÃO
Implemente as seguintes transições interativas de tela:
1. Autenticação: ao logar em [INT-01], redirecione para o Dashboard Empresarial [INT-02].
2. Criação de agendamento: ao clicar em "+ Novo Agendamento" em [INT-02], abra o modal [INT-03]. Depois de salvar, feche o modal e insira o compromisso no painel de atividades recentes de [INT-02].
3. Gestão de clientes: ao clicar em "Abrir Histórico" em [INT-04], exiba o detalhe do cliente com conversa e agendamentos relacionados.
4. Atendimento: ao clicar em "Abrir Atendimento" em [INT-02], navegue para [INT-05] com o cliente selecionado e permita envio de mensagem e criação de follow-up.
5. Financeiro: ao clicar em "Gerar Cobrança" em [INT-06], abra um formulário de cobrança simples; após salvar, atualize a tabela e os indicadores do painel.
6. Preferências de tema: ao alterar o toggle em [INT-07], aplique imediatamente o modo claro ou escuro sem recarregar a página.

---

### 4. DIRETRIZES ADICIONAIS
- O protótipo deve priorizar usabilidade para pequenos empresários e autônomos.
- Use iconografia limpa e minimalista para ações principais.
- Mantenha a hierarquia visual clara com destaque para botões de ação e indicadores.
- Inclua estados visuais para feedback: sucesso, erro, carregamento, vazio.
- Considere um layout desktop com sidebar, topbar e cards de conteúdo para facilitar a leitura.
```