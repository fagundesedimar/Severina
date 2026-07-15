export interface ClientNote {
  id: string;
  content: string;
  authorId: string;
  createdAt: string;
}

export interface Client {
  id: string;
  companyId: string;
  nome: string;
  email: string | null;
  telefone: string | null;
  empresa: string | null;
  status: 'Ativo' | 'Inativo';
  tags: string[];
  notes: ClientNote[];
  createdAt: string;
  updatedAt: string;
}

export interface PagedClientResponse {
  items: Client[];
  totalCount: number;
  page: number;
  pageSize: number;
}

export interface Interaction {
  id: string;
  clientId: string;
  type: 'Message' | 'Call' | 'Email' | 'Note' | 'Appointment';
  content: string;
  metadataDirection: string | null;
  metadataDurationSeconds: number | null;
  metadataStatus: string | null;
  conversationId: string | null;
  createdAt: string;
}

export interface PagedInteractionResponse {
  items: Interaction[];
  totalCount: number;
  page: number;
  pageSize: number;
}

export interface ImportJob {
  id: string;
  fileName: string;
  totalRows: number;
  processedRows: number;
  importedRows: number;
  skippedRows: number;
  errorRows: number;
  status: 'Processing' | 'Completed' | 'Failed';
  errorMessage: string | null;
  createdAt: string;
}

export interface ClientListParams {
  page?: number;
  pageSize?: number;
  search?: string;
  status?: string;
}

export interface InteractionListParams {
  page?: number;
  pageSize?: number;
  type?: string;
}
