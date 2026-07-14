## Summary

Implementar CRUD completo de clientes, busca full-text, importação em lote, e histórico de interações por cliente, tudo isolado por company_id.

## Problem Statement

O WhatsApp Business não mantém histórico completo de clientes. Precisamos de uma fonte única de verdade para dados de clientes com busca avançada e visão 360°.

## Solution Approach

### Backend
- CRUD de Client (nome, email, telefone, empresa, tags, notas)
- Busca full-text por nome, email, empresa
- Importação CSV/XLSX (até 1000 registros)
- Endpoints: GET/POST/PUT/DELETE /api/v1/clients

### Frontend
- Lista de clientes com busca e filtros conforme protótipo Stitch (clientes.html)
- Formulário de cadastro PF/PJ
- Detalhe do cliente com histórico de interações
- Upload de arquivo para importação com preview

### Integrações
- Leitura de contatos do WhatsApp Business API (futuro)

## Impact

- **Arquivos novos**: Client entity, controller, service, migration, pages
- **Dependencies**: Nenhuma dependência externa nova
- **Risco**: Importação em lote pode causar timeout - usar background job

## Stitch References
- `stitch-prototypes/clientes.html` - Lista, cards, formulário PF/PJ
