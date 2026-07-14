# CHG-05: Cobranças e Financeiro

## Status
`proposta` | `em_andamento` | `concluida`

## Resumo
Implementar o módulo financeiro com geração de cobranças, acompanhamento de pagamentos e lembretes via WhatsApp (INT-06).

## Protótipo de Referência
- **Arquivo:** `stitch-prototypes/financeiro.html`
- **Tela:** Financeiro - Severina AI (Português)
- **Dispositivo:** Mobile (780x4158)
- **Screen ID:** `projects/2167347516013784741/screens/1a23804950da41dca1ccd660f0c19bea`

## Componentes do Protótipo (Financeiro)

### Estrutura HTML
```
financeiro-container
├── topbar (mobile)
├── content
│   ├── header
│   │   ├── h1: "Financeiro"
│   │   └── button: "+ Gerar Cobrança" (primary pill)
│   ├── stats-grid (3 cards)
│   │   ├── card: "TOTAL A RECEBER" (R$ valor)
│   │   ├── card: "PAGOS (30 DIAS)" (R$ valor)
│   │   └── card: "ATRASADOS" (R$ valor + alerta)
│   ├── filters (status toggle: Todos, Pendentes, Pagos, Atrasados)
│   ├── invoices-table
│   │   ├── thead: CLIENTE, VALOR, VENCIMENTO, STATUS, AÇÕES
│   │   └── tbody (rows with invoice data)
│   └── invoice-card (mobile)
│       ├── client info
│       ├── value (JetBrains Mono)
│       ├── due date
│       └── status badge
└── bottomnav (mobile)
```

### Design Tokens Extraídos
| Token | Valor | Uso |
|-------|-------|-----|
| `error` | #ba1a1a | Status "Atrasado" |
| `success` | #16a34a | Status "Pago" |
| `warning` | #d97706 | Status "Pendente" |
| `mono` | JetBrains Mono | Valores financeiros |

### Cards de Indicadores
```html
<div class="stat-card">
  <span class="overline">TOTAL A RECEBER</span>
  <span class="headline-lg">R$ 12.450,00</span>
  <span class="caption">+8% vs mês anterior</span>
</div>
```

### Tabela de Cobranças
```html
<table>
  <thead>
    <tr>
      <th>CLIENTE</th>
      <th>VALOR</th>
      <th>VENCIMENTO</th>
      <th>STATUS</th>
      <th>AÇÕES</th>
    </tr>
  </thead>
  <tbody>
    <tr>
      <td><span class="font-label-md">Sarah Jenkins</span></td>
      <td><span class="mono">R$ 450,00</span></td>
      <td><span class="mono">10/08/2026</span></td>
      <td><span class="badge-warning">Pendente</span></td>
      <td><button>Lembrete</button></td>
    </tr>
  </tbody>
</table>
```

### Botões de Ação
- **Enviar Lembrete:** Habilitado apenas para Pendente/Atrasado
- **Filtrar por Status:** Toggle group (Todos, Pendentes, Pagos, Atrasados)

## Escopo Funcional
- Endpoint CRUD /api/v1/invoices (criar, listar, atualizar status)
- Endpoint POST /api/v1/invoices/{id}/reminder (enviar lembrete)
- Cards de indicadores: Total a Receber, Pagos 30 dias, Atrasados
- Tabela de cobranças conforme protótipo
- Status: Pendente (warning), Pago (success), Atrasado (error)
- Botão "Enviar Lembrete" (habilitado para Pendente/Atrasado)
- Atualização automática de status quando pagamento é registrado
- Valores em JetBrains Mono (alinhamento vertical)
- Geração de relatório PDF com métricas do período

## RF Relacionados
- RF-005: Cobrança Automática

## Dependências
- CHG-01 (Autenticação)
- CHG-02 (Gestão de empresa/usuários)
- CHG-03 (Gestão de clientes)
- CHG-04 (Agendamentos)

## Riscos
- Integração com gateway de pagamento não está no MVP
- Lembretes via WhatsApp podem falhar se API estiver indisponível

## Tamanho Estimado
- **Complexidade**: Média
- **Esforço**: 2 semanas
- **Risco**: Médio

## Critérios de Conclusão
- [ ] CRUD de cobranças funciona
- [ ] Cards de indicadores calculam valores corretos
- [ ] Tabela exibe cobranças conforme protótipo (financeiro.html)
- [ ] Botão "Enviar Lembrete" funciona
- [ ] Status atualiza automaticamente com pagamento
- [ ] Valores usam JetBrains Mono
- [ ] Relatório PDF é gerado
- [ ] Testes unitários de cálculos passam
- [ ] Testes de integração de endpoints passam
- [ ] Teste E2E de fluxo financeiro passa

## Testes Obrigatórios

### Unitários
- Cálculo de totais (pendentes, pagos, atrasados)
- Validação de status transitions
- Formatação de valores monetários

### Integração
- Criação de cobrança vinculada a compromisso
- Envio de lembrete
- Atualização de status via webhook

### E2E
- Fluxo: Dashboard → Financeiro → Gerar Cobrança → Enviar Lembrete
- Indicadores atualizam corretamente
