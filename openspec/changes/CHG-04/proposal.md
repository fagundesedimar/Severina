# CHG-04: Agendamento e Compromissos

## Status
`proposta` | `em_andamento` | `concluida`

## Resumo
Implementar o módulo de agendamento inteligente com criação, gerenciamento e lembretes de compromissos (INT-03 modal, listagem).

## Protótipo de Referência
- **Arquivo:** `stitch-prototypes/agenda.html`
- **Tela:** Agenda - Severina AI (Português)
- **Dispositivo:** Mobile (780x2346)
- **Screen ID:** `projects/2167347516013784741/screens/fe120f54a4584ad7b407e067ef64ba97`

## Componentes do Protótipo (Agenda)

### Estrutura HTML
```
agenda-container
├── topbar (mobile)
├── content
│   ├── header
│   │   ├── h1: "Agenda"
│   │   └── button: "+ Novo Compromisso" (primary pill)
│   ├── calendar-view
│   │   ├── date-selector (prev/next arrows)
│   │   └── time-slots (grid de horários)
│   ├── appointments-list
│   │   ├── appointment-card
│   │   │   ├── time: "09:00"
│   │   │   ├── client: "Sarah Jenkins"
│   │   │   ├── service: "Consulta Completa"
│   │   │   └── status: "Confirmado" (badge)
│   │   └── appointment-card...
│   └── modal#new-appointment
│       ├── client-autocomplete
│       ├── service-dropdown
│       ├── datetime-picker
│       ├── channel-radio (WhatsApp, Presencial, Online)
│       ├── notes-textarea
│       └── actions (Agendar, Cancelar)
└── bottomnav (mobile)
```

### Design Tokens Extraídos
| Token | Valor | Uso |
|-------|-------|-----|
| `success` | #16a34a | Status "Confirmado" |
| `warning` | #d97706 | Status "Pendente" |
| `info` | #0284c7 | Status "Pendente IA" |
| `surface-container-highest` | #dae2fd | Status "Lista de Espera" |

### Tabela de Compromissos (Dashboard)
```html
<table>
  <thead>
    <tr>
      <th>CLIENTE</th>
      <th>SERVIÇO</th>
      <th>HORA</th>
      <th>STATUS</th>
    </tr>
  </thead>
  <tbody>
    <tr>
      <td><span class="font-label-md">Sarah Jenkins</span></td>
      <td><span class="font-body-sm">Consulta Completa</span></td>
      <td><span class="font-body-sm">14:30</span></td>
      <td><span class="badge-success">Confirmado</span></td>
    </tr>
    <tr>
      <td><span class="font-label-md">Robert Wilson</span></td>
      <td><span class="font-body-sm">Renovação de Pacote</span></td>
      <td><span class="font-body-sm">16:00</span></td>
      <td><span class="badge-info">Pendente IA</span></td>
    </tr>
  </tbody>
</table>
```

### Badges de Status
```css
.badge-success { background: green-100; color: green-700; }
.badge-info { background: blue-100; color: blue-700; }
.badge-warning { background: surface-container-highest; color: on-surface-variant; }
```

## Escopo Funcional
- Endpoint CRUD /api/v1/appointments (criar, listar, editar, cancelar)
- Validação de conflito de horário (não permitir sobreposição)
- Modal INT-03 de novo agendamento conforme protótipo
- DateTime Picker com bloqueio de datas retroativas e horário comercial
- Alerta de conflito de horário (warning amarelo)
- Lembretes automáticos via notificação push (24h antes)
- Listagem de compromissos com filtros por período e status

## RF Relacionados
- RF-004: Agendamento Inteligente

## Dependências
- CHG-01 (Autenticação)
- CHG-02 (Gestão de empresa/usuários)
- CHG-03 (Gestão de clientes)

## Riscos
- Validação de conflito pode falhar em concorrência
- DateTime Picker pode ter problemas de timezone

## Tamanho Estimado
- **Complexidade**: Média-Alta
- **Esforço**: 2-3 semanas
- **Risco**: Médio

## Critérios de Conclusão
- [ ] CRUD de compromissos funciona
- [ ] Validação de conflito de horário rejeita sobreposição
- [ ] Modal INT-03 renderiza conforme protótipo (agenda.html)
- [ ] Autocomplete de clientes funciona com debounce
- [ ] DateTime Picker bloqueia datas retroativas
- [ ] Alerta de conflito aparece quando aplicável
- [ ] Lembrete é enviado 24h antes do compromisso
- [ ] Testes unitários de validação passam
- [ ] Testes de integração de endpoints passam
- [ ] Teste E2E de agendamento passa

## Testes Obrigatórios

### Unitários
- Validação de conflito de horário
- Cálculo de data de lembrete
- Validação de horário comercial

### Integração
- Criação de compromisso com/s sem conflito
- Listagem com filtros de período
- Cancelamento de compromisso

### E2E
- Fluxo: Dashboard → +Novo Agendamento → Preencher → Agendar → Ver na lista
- Conflito de horário exibe alerta
