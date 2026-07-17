# Definição de Requisitos do Produto (PRD)

## Descrição do produto

### Problema

* O problema é a sobrecarga de tarefas administrativas e operacionais em pequenas empresas, com atendimento, agenda, cobrança e follow-up fragmentados.
* Esse problema afeta pequenos empresários e profissionais autônomos, causando atendimento inconsistente, perda de receita e baixa produtividade.
* A insuficiência de automação e a divisão entre canais de comunicação geram atrasos, erros em agendamentos e dificuldade em manter o fluxo de caixa.

### Solução

* A solução proposta é a Severina AI, uma secretária virtual baseada em IA que automatiza atendimento omnichannel, agendamentos, cobranças, follow-up e geração de insights.
* O produto resolve o problema ao centralizar canais, automatizar fluxos administrativos e fornecer um assistente virtual integrado para pequenas empresas.
* Os principais módulos ou fluxos são Atendimento, CRM, Agenda, Financeiro, Analytics e Integração com WhatsApp.
* Para pequenos empresários de serviços: reduz o tempo gasto em tarefas administrativas e melhora o controle de agenda e pagamentos.
* Para autônomos e microempresas: garante respostas rápidas aos clientes, follow-up automático e menos dependência de processos manuais.

### Diferenciais

* **Atendimento omnichannel integrado**: unifica WhatsApp, web e outros canais em um único fluxo inteligente.
* **Agendamento inteligente e follow-up automático**: evita conflitos de horários e aumenta a taxa de presença em compromissos.
* **Cobrança automática e insights de negócio**: melhora fluxo de caixa e ajuda o empreendedor a tomar decisões estratégicas.

---

## Perfis de Usuário

### Empreendedor de Serviços Autônomo

#### Problemas

* Dificuldade em gerenciar atendimento ao cliente e agenda ao mesmo tempo.
* Perda de oportunidades por falta de follow-up e cobrança estruturada.

#### Objetivos

* Automatizar atendimento e agendamento para ganhar tempo operacional.
* Manter o fluxo financeiro sob controle com cobranças mais consistentes.

#### Dados demográficos

* Faixa etária: 25-50 anos
* Localização: pequenas cidades e regiões metropolitanas do Brasil
* Outras características relevantes: usa WhatsApp como canal principal de atendimento, tem equipe reduzida ou trabalha sozinho.

#### Motivações

* Aumentar produtividade sem aumentar custos fixos.
* Ter mais tempo para focar em entrega de serviços e relacionamento com clientes.

#### Frustrações

* Responder a clientes em múltiplos canais de forma manual.
* Esquecer de cobrar ou de acompanhar um compromisso marcado.

### Pequena Empresa com Baixa Estrutura Administrativa

#### Problemas

* Processos internos fragmentados entre planilhas, telefonemas e aplicativos diferentes.
* Dificuldade em acompanhar agenda, clientes e finanças de forma integrada.

#### Objetivos

* Centralizar a gestão de clientes e agenda em uma plataforma fácil de usar.
* Reduzir erros administrativos e melhorar a previsibilidade de caixa.

#### Dados demográficos

* Faixa etária: 30-55 anos
* Localização: centros urbanos e regiões de negócios locais
* Outras características relevantes: equipe de até 10 pessoas, pouco investimento em TI, necessidade de conformidade com LGPD.

#### Motivações

* Otimizar a operação com tecnologia acessível.
* Melhorar a experiência do cliente e a eficiência interna.

#### Frustrações

* Gastar muito tempo em tarefas administrativas em vez de focar no core business.
* Não ter visibilidade clara dos compromissos e recebíveis.

---

## Funcionalidades

### Requisitos Funcionais

#### RF-001 Atendimento Omnichannel

* Objetivo: permitir que a Severina AI receba e responda clientes por WhatsApp, web e outros canais em um único fluxo.

#### RF-002 Agendamento Inteligente

* Objetivo: automatizar a criação e o gerenciamento de compromissos, evitando conflitos de horários e reduzindo faltas.

#### RF-003 Cobrança Automática

* Objetivo: gerar e enviar cobranças aos clientes, com follow-up e lembretes automáticos para aumentar a taxa de pagamento.

#### RF-004 Dashboard de Insights

* Objetivo: apresentar métricas e insights de desempenho comercial, financeiro e de atendimento.

#### RF-005 CRM de Clientes

* Objetivo: centralizar informações de clientes, histórico de conversas e oportunidades em uma base única.

#### RF-006 Preferências de Tema

* Objetivo: permitir que o usuário alterne entre modo claro, escuro e sistema, com preferência persistida por conta.
* A toggle de tema deve estar posicionada no topbar, próxima ao avatar do usuário logado.
* A preferência deve ser sincronizada entre dispositivos via API e armazenada localmente (localStorage).
* O padrão deve ser `system` (respeita a configuração do SO do usuário).
* A acessibilidade deve ser garantida: `aria-label`, `role="switch"`, `aria-checked`.

#### RF-007 Convite de Usuários por Email

* Objetivo: permitir que administradores convidem novos usuários para a empresa por email, com link de aceitação que expira em 7 dias.
* O administrador informa o email e o perfil (Administrador/Operacional) do convite.
* O email de convite é enviado via Resend com template HTML contendo link `/convite/{code}`.
* O convite expira automaticamente após 7 dias e pode ser revogado pelo administrador.
* O usuário convidado acessa o link, define nome e senha, e é redirecionado para o login.

---

## Requisitos Não Funcionais

### RNF-01 Segurança

* Proteger dados de clientes e transações com criptografia, autenticação JWT e suporte a MFA.

### RNF-02 Auditoria

* Registrar eventos de acesso, alterações e interações em logs auditáveis.

### RNF-03 Observabilidade

* Oferecer monitoramento de desempenho e erros com alertas em tempo real.

### RNF-04 Escalabilidade

* Suportar crescimento para até 1 mil empresa ativa simultaneamente no MVP, com escalabilidade horizontal para 1 milhão de empresas em versões futuras, em arquitetura SaaS multi-tenant.

### RNF-05 Portabilidade

* Permitir implantação em nuvem com suporte a padrões como Kubernetes e contêineres.

### RNF-06 Testabilidade

* Facilitar testes automatizados de unidade, integração e ponta a ponta com pipelines CI/CD.
* Cobertura mínima de 80% de linhas no backend e 70% no frontend.
* Toda alteração de regra de negócio deve cobrir Happy Path, Sad Path e Edge Cases.

### RNF-07 Compliance

* Atender integralmente à LGPD com políticas de consentimento, direito ao esquecimento e trilhas de auditoria.

### RNF-08 Usabilidade e Acessibilidade

* Suportar modo claro e escuro com toggle acessível (WCAG 2.1 AA).
* A preferência de tema deve ser persistida por usuário e sincronizada entre dispositivos.
* A interface deve respeitar `prefers-color-scheme` do sistema operacional como fallback.
* A troca de tema deve ser instantânea sem recarregamento da página.
* Contraste de cores deve manter razão mínima de 4.5:1 em ambos os modos (claro e escuro).

---

## Métricas de Sucesso

### Métricas de Negócio

* Tempo médio gasto em tarefas administrativas

  * Valor atual: 100% do tempo operacional em atividades manuais
  * Meta: reduzir para 50%
  * Prazo: 6 meses após MVP

* Taxa de conversão de agendamentos confirmados

  * Valor atual: 50%
  * Meta: 75%
  * Prazo: 6 meses após lançamento

### Métricas de Produto

* Taxa de adoção da plataforma entre pequenas empresas

  * Valor atual: 0% (pré-lançamento)
  * Meta: 100 empresas ativas no MVP
  * Prazo: 6 meses após lançamento

* Nível de satisfação com atendimento automatizado (NPS)

  * Valor atual: N/A
  * Meta: NPS >= 50
  * Prazo: 3 meses após lançamento

### Métricas de Operação

* Disponibilidade do sistema

  * Valor atual: N/A
  * Meta: 99,9% de uptime
  * Prazo: Contínuo

* Tempo de resposta médio da API

  * Valor atual: N/A
  * Meta: < 200ms para 95% das requisições
  * Prazo: Contínuo

* Taxa de erro da API (5xx)

  * Valor atual: N/A
  * Meta: < 0,1%
  * Prazo: Contínuo

---

## Premissas e Restrições

### Premissas

* Pequenas empresas aceitam substituir processos manuais por uma plataforma de IA.
* O público-alvo utiliza WhatsApp como principal canal de comunicação.
* A solução será oferecida como SaaS com preço acessível.

### Restrições

* Limite orçamentário para implementações complexas no MVP.
* Adoção deve ser simples sem exigir grande treinamento ou suporte técnico.
* Devem ser atendidos requisitos de LGPD e segurança de dados.

### Dependências Externas

* Integração com APIs de WhatsApp para atendimento e mensagens.
* Serviços de mensageria como RabbitMQ para orquestração de eventos.

---

## Escopo

### MVP

**Prazo estimado:** 3 meses de desenvolvimento

#### Incluído

* Atendimento omnichannel básico
* Agendamento inteligente
* Cobrança automática
* Dashboard inicial de indicadores
* Modo claro/escuro com preferência por usuário

#### Não Incluído

* Funcionalidades avançadas de customização de fluxos
* Suporte a múltiplos idiomas além do português
* Integração com ERP corporativo

### Versão 1.0

* Treinamento de IA com dados da empresa
* Agente Financeiro especializado
* Relatórios avançados de analytics

### Versões Futuras

* Expansão para agentes executivos multi-domínio
* Integrações com novas plataformas de comunicação
* Recursos de automação de vendas e CRM avançado

---

## Critérios de Aceitação do Produto

### Critérios de Negócio

* A plataforma deve reduzir em pelo menos 50% o tempo gasto em tarefas administrativas.
* A solução deve ser adotada por usuários como principal assistente administrativo.

### Critérios Técnicos

* O sistema deve atender a disponibilidade de 99,9%.
* A API deve responder em menos de 200ms para 95% das requisições de leitura.
* Cobertura mínima de testes: 80% linhas backend, 70% linhas frontend.

### Critérios de Qualidade

* A interface deve ser intuitiva para usuários com pouca experiência em TI.
* As integrações devem funcionar de forma confiável para mensagens e agendamentos.

### Critérios de Teste

* Todos os requisitos funcionais devem ter testes automatizados de unidade e integração.
* Fluxos críticos (atendimento, agendamento, cobrança) devem ter testes E2E.
* Testes de regressão devem ser executados em cada pipeline CI/CD.
* Cobertura mínima de 80% (backend) e 70% (frontend) em linhas de código.

---

## Riscos

### Riscos de Negócio

* Falta de adoção pelos pequenos empresários

  * Probabilidade: MÉDIA
  * Impacto: ALTO
  * Mitigação: oferecer preço acessível, onboarding simples e provas de valor rápido.

### Riscos Técnicos

* Falha na integração com WhatsApp ou canais externos

  * Probabilidade: MÉDIA
  * Impacto: ALTO
  * Mitigação: utilizar provedores estáveis e implementar fallback para canais alternativos.

### Riscos Operacionais

* Indisponibilidade do provedor de IA (LLM)

  * Probabilidade: BAIXA
  * Impacto: ALTO
  * Mitigação: manter fallback com respostas padrão e suporte a múltiplos provedores de IA.

### Riscos de Conformidade

* Descumprimento da LGPD

  * Probabilidade: BAIXA
  * Impacto: MUITO ALTO
  * Mitigação: implementar consentimento granular, criptografia de dados sensíveis e trilhas de auditoria desde o MVP.

---

## Glossário

### Termos de Negócio

* **Secretária Virtual**: assistente baseada em IA que automatiza atendimento, agenda, cobrança e follow-up.
* **Omnichannel**: atendimento unificado em múltiplos canais de comunicação.
* **Follow-up**: acompanhamento automático de clientes após interações ou compromissos.

### Siglas

* **IA**: Inteligência Artificial
* **SaaS**: Software as a Service
* **LGPD**: Lei Geral de Proteção de Dados
* **CRM**: Customer Relationship Management
* **WCAG**: Web Content Accessibility Guidelines
