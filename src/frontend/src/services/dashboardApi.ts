import api from './api';
import type { DashboardResponse } from '@/types/dashboard';

export const dashboardApi = {
  async get(): Promise<DashboardResponse> {
    const response = await api.get('/api/v1/dashboard');
    return response.data;
  },
};
