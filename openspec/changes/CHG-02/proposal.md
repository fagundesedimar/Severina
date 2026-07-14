# CHG-02: Gestão de Empresa e Usuários

## Status
`proposta` | `em_andamento` | `concluida`

## Resumo
Implementar CRUD de empresa (dados cadastrais) e gestão de usuários (perfil, preferências, permissões), incluindo a tela de Configurações (INT-07).

## Protótipo de Referência
- **Sidebar do Dashboard:** `stitch-prototypes/dashboard.html` (seção "Configurações")
- **Elemento:** `<a href="#">Configurações</a>` com ícone `settings`
- **Seção "Sistema"** no sidebar com label `SISTEMA` (overline)

## Componentes do Protótipo (Configurações via Sidebar)

### Navegação
```
Sidebar (Desktop)
├── nav
│   ├── a: "Início" (home icon) - ativo
│   ├── a: "Clientes" (group icon)
│   ├── a: "Agenda" (calendar_today icon)
│   ├── a: "Financeiro" (payments icon)
│   ├── div.seção: "SISTEMA" (overline)
│   └── a: "Configurações" (settings icon)
```

### Design Tokens
| Token | Valor | Uso |
|-------|-------|-----|
| `secondary` | #515f74 | Texto de nav items |
| `secondary-fixed-dim` | #b9c7df | Texto nav items dark |
| `surface-container-low` | #f2f3ff | Fundo item ativo |
| `primary-container/20` | rgba(37,99,235,0.2) | Fundo item ativo dark |

## Escopo Funcional
- Endpoint GET/PUT /api/v1/companies (dados da empresa autenticada)
- Endpoint CRUD /api/v1/users (gestão de usuários da empresa)
- Endpoint GET/PUT /api/v1/users/preferences (tema, notificações)
- Tela de Configurações (INT-07) com cards agrupados:
  - Preferências de tema (toggle claro/escuro/sistema)
  - Configurações de notificações (email, push, cobrança)
  - Integrações (status WhatsApp)
  - Dados da empresa (nome, CNPJ/CPF, telefone, endereço)
- Controle de acesso: apenas Admin pode gerenciar usuários
- Auditoria: log de alterações de empresa e preferências

## RF Relacionados
- RF-001: Cadastro de Empresa
- RF-008: Preferências de Tema

## Dependências
- CHG-01 (Autenticação e estrutura base)

## Riscos
- Permissões incorretas podem expor dados de outras empresas
- Preferências de tema podem não sincronizar entre dispositivos

## Tamanho Estimado
- **Complexidade**: Média
- **Esforço**: 1-2 semanas
- **Risco**: Baixo

## Critérios de Conclusão
- [ ] GET /api/v1/companies retorna dados da empresa logada
- [ ] PUT /api/v1/companies atualiza dados (apenas Admin)
- [ ] GET /api/v1/users/preferences retorna preferências
- [ ] PUT /api/v1/users/preferences atualiza tema/notificações
- [ ] Tela INT-07 renderiza todas as seções conforme protótipo
- [ ] Toggle de tema funciona conforme micro-interações do protótipo
- [ ] Testes unitários de preferências passam
- [ ] Testes de integração de endpoints passam
- [ ] Teste E2E de configurações passa

## Testes Obrigatórios

### Unitários
- Validação de dados de empresa (CNPJ/CPF format)
- Validação de preferências (theme enum: light/dark/system)
- Controle de acesso por papel (Admin vs Operacional)

### Integração
- GET/PUT /api/v1/companies com autenticação
- GET/PUT /api/v1/users/preferences
- Tentativa de acesso sem permissão (403)

### E2E
- Navegação: Dashboard → Configurações → Alterar tema → Voltar
