## Summary

Implementar gestão de cobranças, controle de receitas/despesas, e dashboard financeiro com KPIs, tudo integrado com o módulo de clientes e dashboard.

## Problem Statement

Não há controle financeiro integrado. Cobranças são gerenciadas manualmente sem rastreabilidade, dificultando o fluxo de caixa.

## Solution Approach

### Backend
- CRUD de Transaction (valor, data, categoria, tipo, cliente_id)
- CRUD de Invoice/Charge (valor, vencimento, status, cliente_id)
- Endpoints: GET/POST/PUT/DELETE /api/v1/transactions
- Endpoints: GET/POST/PUT /api/v1/invoices

### Frontend
- Dashboard financeiro conforme protótipo Stitch (financeiro.html)
- Lista de transações com filtros
- Formulário de cadastro de transação
- KPIs: saldo, receitas, despesas, previsão

### Integrações
- Webhook Mercado Pago (pagamentos)
- Exportação CSV/XLSX

## Impact

- **Arquivos novos**: Transaction/Invoice entities, controller, service, migration, pages
- **Dependencies**: Mercado Pago SDK (futuro)
- **Risco**: Cálculos financeiros precisam de precisão centesimal - usar decimal

## Stitch References
- `stitch-prototypes/financeiro.html` - Dashboard financeiro, KPIs, gráficos
