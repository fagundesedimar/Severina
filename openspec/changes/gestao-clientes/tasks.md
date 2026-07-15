## 1. Domain Layer - Client

- [x] 1.1 Criar entidade Client (id, CompanyId, Nome, Email, Telefone, Empresa, Tags, Notas, CreatedAt, UpdatedAt, DeletedAt)
- [x] 1.2 Criar Value Object ClientEmail com validação de formato
- [x] 1.3 Criar Value Object ClientNote (id, Content, AuthorId, CreatedAt)
- [x] 1.4 Criar Value Object ClientTag (Name) com validação de tamanho
- [x] 1.5 Implementar comportamentos: Client.AddTag(), Client.RemoveTag(), Client.AddNote()
- [x] 1.6 Criar Domain Events: ClientCreatedEvent, ClientUpdatedEvent, ClientTagAddedEvent

## 2. Domain Layer - Interaction

- [x] 2.1 Criar entidade Interaction (id, ClientId, CompanyId, Type, Content, Metadata, ConversationId, CreatedAt)
- [x] 2.2 Criar enum InteractionType (Message, Call, Email, Note, Appointment)
- [x] 2.3 Criar Value Object InteractionMetadata (direção, duração, status)
- [x] 2.4 Implementar comportamento: Interaction.Create() com validação de tipo

## 3. Infrastructure - EF Core

- [x] 3.1 Criar ClientConfiguration com Global Query Filter para CompanyId
- [ ] 3.2 Criar index GIN para busca full-text (tsvector) em Nome, Email, Empresa (SQLite: LIKE indexes)
- [x] 3.3 Criar InteractionConfiguration com index em ClientId + CreatedAt
- [ ] 3.4 Criar migration para Clients e Interactions (EnsureCreated used)
- [x] 3.5 Criar IClientRepository com CRUD + busca full-text
- [x] 3.6 Criar IInteractionRepository com listagem por cliente

## 4. Application - Client CRUD

- [x] 4.1 Criar DTOs: ClientResponse, CreateClientRequest, UpdateClientRequest
- [x] 4.2 Criar CreateClientCommand + Handler com validação (FluentValidation)
- [x] 4.3 Criar ListClientsQuery + Handler com paginação
- [x] 4.4 Criar GetClientByIdQuery + Handler
- [x] 4.5 Criar UpdateClientCommand + Handler
- [x] 4.6 Criar DeleteClientCommand + Handler (soft delete)
- [x] 4.7 Criar AddClientTagCommand + Handler
- [x] 4.8 Criar RemoveClientTagCommand + Handler
- [x] 4.9 Criar AddClientNoteCommand + Handler

## 5. Application - Search

- [x] 5.1 Criar SearchClientsQuery + Handler com LIKE search
- [x] 5.2 Implementar paginação de resultados (page, pageSize)
- [ ] 5.3 Implementar highlight de termos com <mark> tags (frontend)
- [ ] 5.4 Criar índice GIN para performance (SQLite: LIKE indexes)

## 6. Application - Import

- [x] 6.1 Criar ImportClientsCommand + Handler
- [x] 6.2 Criar IImportService interface para processamento
- [x] 6.3 Criar CsvImportService com CsvHelper para parsing
- [x] 6.4 Implementar validação de linhas (email, campos obrigatórios)
- [x] 6.5 Implementar detecção de duplicatas
- [x] 6.6 Criar ImportJob entity para rastrear progresso
- [x] 6.7 Criar ImportJobRepository para persistir status

## 7. Application - History

- [x] 7.1 Criar ListClientInteractionsQuery + Handler
- [x] 7.2 Criar CreateInteractionCommand + Handler
- [x] 7.3 Implementar paginação de interações (page, pageSize)
- [x] 7.4 Implementar filtro por tipo de interação

## 8. API Layer

- [x] 8.1 Criar ClientsController com endpoints CRUD
- [x] 8.2 Busca integrada no ClientsController (GET /api/v1/clients/search)
- [x] 8.3 Criar ClientImportController com upload de arquivo
- [x] 8.4 Interação integrada no ClientsController (GET/POST /api/v1/clients/{id}/interactions)
- [x] 8.5 Configurar validação de arquivo (tamanho, tipo) no ClientImportController
- [x] 8.6 Configurar Rate Limiting nos endpoints de importação

## 9. Frontend - Client List

- [x] 9.1 Criar página /clientes conforme protótipo Stitch (clientes.html)
- [x] 9.2 Criar componente de lista de clientes com paginação
- [x] 9.3 Criar componente de busca com debounce
- [x] 9.4 Criar filtros por tags e status
- [x] 9.5 Integrar com GET /api/v1/clients e busca

## 10. Frontend - Client Detail

- [x] 10.1 Criar página /clientes/{id} com detalhes do cliente
- [x] 10.2 Criar formulário de edição inline
- [x] 10.3 Criar componente de tags (add/remove)
- [x] 10.4 Criar componente de notas (add/list)
- [x] 10.5 Criar timeline de interações conforme Stitch
- [x] 10.6 Integrar com endpoints de cliente e interações

## 11. Frontend - Import

- [x] 11.1 Criar página /clientes/importar
- [x] 11.2 Criar componente de drag-and-drop para upload
- [x] 11.3 Criar tabela de preview com validação
- [x] 11.4 Criar indicador de progresso
- [x] 11.5 Integrar com POST /api/v1/clients/import

## 12. Testes

- [ ] 12.1 Testes unitários de Client (addTag, removeTag, addNote)
- [ ] 12.2 Testes unitários de Interaction (validação de tipo)
- [ ] 12.3 Testes de integração de Client CRUD (EF Core + SQLite)
- [ ] 12.4 Testes de integração de busca
- [ ] 12.5 Testes de integração de importação (CSV parsing)
- [ ] 12.6 Testes de integração de histórico de interações
- [ ] 12.7 Testes de IDOR (across-tenant access deve retornar 404)
- [ ] 12.8 Testes E2E de criação de cliente (Playwright)
- [ ] 12.9 Testes E2E de importação (Playwright)

## 13. Lint e Qualidade

- [x] 13.1 Rodar dotnet format no backend
- [x] 13.2 Rodar ESLint no frontend (npm run lint)
- [ ] 13.3 Verificar cobertura mínima 80% backend, 70% frontend
- [ ] 13.4 Verificar acessibilidade (WCAG 2.1 AA) na lista de clientes
- [ ] 13.5 Verificar responsividade (mobile/tablet/desktop)
- [ ] 13.6 Verificar performance da busca (< 200ms para 95%)
