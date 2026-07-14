## Summary

Implementar dashboard principal com métricas de atendimento, KPIs de produtividade, e widgets de resumo, integrando dados de todos os módulos.

## Problem Statement

Não há visão consolidada do desempenho do negócio. Métricas ficam dispersas em diferentes sistemas.

## Solution Approach

### Backend
- Endpoints de agregação: GET /api/v1/dashboard/summary
- Métricas: atendimentos hoje, pendentes, taxa_conversao, faturamento
- Cache Redis para dados de alta frequência (TTL 5min)

### Frontend
- Dashboard conforme protótipo Stitch (dashboard.html)
- KPIs: atendimentos, pendentes, conversão, faturamento
- Gráficos: barras (atendimentos/dia), pizza (origens)
- Acesso rápido: novo atendimento, novo cliente, nova cobrança

### Integrações
- Dados do WhatsApp Business API (atendimentos)
- Dados do módulo financeiro (faturamento)

## Impact

- **Arquivos novos**: Dashboard service, endpoints, pages
- **Dependencies**: Redis (cache), dados dos outros módulos
- **Risco**: Performance com muitos dados - usar cache e paginação

## Stitch References
- `stitch-prototypes/dashboard.html` - Layout, KPIs, gráficos
