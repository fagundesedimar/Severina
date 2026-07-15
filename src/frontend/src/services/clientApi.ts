import api from './api';
import type {
  PagedClientResponse,
  Client,
  PagedInteractionResponse,
  ImportJob,
  ClientListParams,
  InteractionListParams,
} from '@/types/client';

export const clientApi = {
  async list(params?: ClientListParams): Promise<PagedClientResponse> {
    const response = await api.get('/api/v1/clients', { params });
    return response.data;
  },

  async getById(id: string): Promise<Client> {
    const response = await api.get(`/api/v1/clients/${id}`);
    return response.data;
  },

  async search(searchTerm: string, page = 1, pageSize = 20): Promise<PagedClientResponse> {
    const response = await api.get('/api/v1/clients/search', {
      params: { searchTerm, page, pageSize },
    });
    return response.data;
  },

  async create(data: { nome: string; email?: string; telefone?: string; empresa?: string }): Promise<Client> {
    const response = await api.post('/api/v1/clients', data);
    return response.data;
  },

  async update(
    id: string,
    data: { nome: string; email?: string; telefone?: string; empresa?: string }
  ): Promise<void> {
    await api.put(`/api/v1/clients/${id}`, data);
  },

  async remove(id: string): Promise<void> {
    await api.delete(`/api/v1/clients/${id}`);
  },

  async addTag(id: string, tagName: string): Promise<void> {
    await api.post(`/api/v1/clients/${id}/tags`, { tagName });
  },

  async removeTag(id: string, tagName: string): Promise<void> {
    await api.delete(`/api/v1/clients/${id}/tags/${encodeURIComponent(tagName)}`);
  },

  async addNote(id: string, content: string): Promise<void> {
    await api.post(`/api/v1/clients/${id}/notes`, { content });
  },

  async listInteractions(
    clientId: string,
    params?: InteractionListParams
  ): Promise<PagedInteractionResponse> {
    const response = await api.get(`/api/v1/clients/${clientId}/interactions`, { params });
    return response.data;
  },

  async createInteraction(
    clientId: string,
    data: { type: string; content: string; conversationId?: string }
  ): Promise<void> {
    await api.post(`/api/v1/clients/${clientId}/interactions`, data);
  },

  async importClients(file: File): Promise<ImportJob> {
    const formData = new FormData();
    formData.append('file', file);
    const response = await api.post('/api/v1/clients/import', formData, {
      headers: { 'Content-Type': 'multipart/form-data' },
    });
    return response.data;
  },
};
