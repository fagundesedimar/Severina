# CHG-03: Gestão de Clientes (CRM)

## Status
`proposta` | `em_andamento` | `concluida`

## Resumo
Implementar o módulo de CRM com CRUD de clientes, busca, filtros, timeline de interações e painel de detalhes (INT-04).

## Protótipo de Referência
- **Arquivo:** `stitch-prototypes/clientes.html`
- **Tela:** Clientes - Severina AI (Português)
- **Dispositivo:** Mobile (780x2470)
- **Screen ID:** `projects/2167347516013784741/screens/92e0a7df51e54e1d99f3a065833dbd54`

## Componentes do Protótipo (Clientes)

### Estrutura HTML
```
clientes-container
├── topbar (mobile)
│   ├── user-avatar
│   ├── title: "Severina AI"
│   └── actions (theme-toggle, notifications)
├── content
│   ├── header
│   │   ├── h1: "Gestão de Clientes"
│   │   └── p: "Gerencie sua base de clientes"
│   ├── search-bar (search icon + input)
│   ├── filters (status dropdown, período)
│   ├── clients-table (desktop)
│   │   ├── thead: NOME, TELEFONE, EMAIL, STATUS, AÇÕES
│   │   └── tbody (rows with client data)
│   └── clients-cards (mobile)
│       └── card per client (nome, telefone, status badge)
└── bottomnav (mobile)
```

### Design Tokens Extraídos
| Token | Valor | Uso |
|-------|-------|-----|
| `surface-container-lowest` | #ffffff | Fundo de cards |
| `outline-variant` | #c3c6d7 | Bordas de tabela |
| `on-surface-variant` | #434655 | Texto secundário |
| `primary` | #004ac6 | Links, botões de ação |

### Tabela de Clientes
```html
<table>
  <thead>
    <tr>
      <th>NOME</th>
      <th>TELEFONE</th>
      <th>EMAIL</th>
      <th>STATUS</th>
      <th>AÇÕES</th>
    </tr>
  </thead>
  <tbody>
    <tr>
      <td><span class="font-label-md">Sarah Jenkins</span></td>
      <td><span class="font-body-sm">+55 11 99999-1234</span></td>
      <td><span class="font-body-sm">sarah@email.com</span></td>
      <td><span class="badge-success">Ativo</span></td>
      <td><button-icon>edit</button-icon></td>
    </tr>
  </tbody>
</table>
```

### Cards Mobile
```html
<div class="card">
  <div class="flex items-center gap-md">
    <div class="avatar-circle">SJ</div>
    <div>
      <span class="font-label-md">Sarah Jenkins</span>
      <span class="font-body-sm">+55 11 99999-1234</span>
    </div>
  </div>
  <span class="badge-success">Ativo</span>
</div>
```

### Micro-interações
- **Card Hover:** `hover:bg-surface-container-low` + cursor pointer
- **Row Hover:** `hover:bg-surface-container-lowest/50`
- **Action Button:** `opacity-0 group-hover:opacity-100` (desktop)

## Escopo Funcional
- Endpoint CRUD /api/v1/clients (criar, listar, editar, desativar)
- Endpoint GET /api/v1/clients/{id} com timeline de interações
- Busca por nome, telefone, email (debounce 300ms)
- Filtros: status (ativo/inativo), período de último contato
- Paginação assíncrona (20 itens/página)
- Tela INT-04 com tabela responsiva (desktop) / cards (mobile)
- Painel lateral de detalhes com timeline vertical
- Desativar cliente mantém histórico (soft delete)
- Exportação CSV da lista filtrada

## RF Relacionados
- RF-002: Cadastro de Clientes
- RF-007: CRM de Clientes

## Dependências
- CHG-01 (Autenticação)
- CHG-02 (Gestão de empresa/usuários)

## Riscos
- Busca sem debounce pode sobrecarregar o banco
- Timeline pode ficar lenta com muitas interações

## Tamanho Estimado
- **Complexidade**: Média
- **Esforço**: 2 semanas
- **Risco**: Baixo

## Critérios de Conclusão
- [ ] CRUD de clientes funciona
- [ ] Busca com debounce retorna resultados corretos
- [ ] Filtros por status e período funcionam
- [ ] Paginação retorna 20 itens por página
- [ ] Tela INT-04 renderiza conforme protótipo (clientes.html)
- [ ] Tabela desktop e cards mobile funcionam
- [ ] Painel de detalhes exibe timeline de interações
- [ ] Exportação CSV gera arquivo correto
- [ ] Testes unitários de validação passam
- [ ] Testes de integração de endpoints passam
- [ ] Teste E2E de fluxo completo passa

## Testes Obrigatórios

### Unitários
- Validação de dados de cliente (email, telefone format)
- Lógica de paginação
- Filtros de status e período

### Integração
- CRUD completo de cliente via API
- Busca com múltiplos termos
- Desativar cliente mantém relacionamentos

### E2E
- Fluxo: Dashboard → Clientes → Adicionar → Editar → Desativar
- Busca e filtros funcionam
- Timeline exibe interações
