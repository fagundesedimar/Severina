import api from './api';
import type { Appointment, PagedAppointmentResponse, CreateAppointmentRequest, UpdateAppointmentRequest } from '@/types/appointment';

export const appointmentApi = {
  async list(params: {
    from?: string;
    to?: string;
    clientId?: string;
    status?: number;
    page?: number;
    pageSize?: number;
  }): Promise<PagedAppointmentResponse> {
    const response = await api.get('/api/v1/appointments', { params });
    return response.data;
  },

  async getById(id: string): Promise<Appointment> {
    const response = await api.get(`/api/v1/appointments/${id}`);
    return response.data;
  },

  async create(data: CreateAppointmentRequest): Promise<Appointment> {
    const response = await api.post('/api/v1/appointments', data);
    return response.data;
  },

  async update(id: string, data: UpdateAppointmentRequest): Promise<Appointment> {
    const response = await api.put(`/api/v1/appointments/${id}`, data);
    return response.data;
  },

  async cancel(id: string): Promise<void> {
    await api.post(`/api/v1/appointments/${id}/cancel`);
  },

  async complete(id: string): Promise<void> {
    await api.post(`/api/v1/appointments/${id}/complete`);
  },

  async delete(id: string): Promise<void> {
    await api.delete(`/api/v1/appointments/${id}`);
  },
};
