import api from './api';
import type {
  FinancialDashboard,
  PagedTransactionResponse,
  Transaction,
  PagedInvoiceResponse,
  Invoice,
  TransactionListParams,
  InvoiceListParams,
} from '@/types/financial';

export const financialApi = {
  async getDashboard(): Promise<FinancialDashboard> {
    const response = await api.get('/api/v1/financialdashboard');
    return response.data;
  },

  async listTransactions(params?: TransactionListParams): Promise<PagedTransactionResponse> {
    const response = await api.get('/api/v1/transactions', { params });
    return response.data;
  },

  async getTransactionById(id: string): Promise<Transaction> {
    const response = await api.get(`/api/v1/transactions/${id}`);
    return response.data;
  },

  async createTransaction(data: {
    tipo: string;
    valor: number;
    data: string;
    categoria: string;
    clientId?: string;
    descricao?: string;
  }): Promise<Transaction> {
    const response = await api.post('/api/v1/transactions', data);
    return response.data;
  },

  async updateTransaction(
    id: string,
    data: {
      tipo: string;
      valor: number;
      data: string;
      categoria: string;
      clientId?: string;
      descricao?: string;
    }
  ): Promise<Transaction> {
    const response = await api.put(`/api/v1/transactions/${id}`, data);
    return response.data;
  },

  async deleteTransaction(id: string): Promise<void> {
    await api.delete(`/api/v1/transactions/${id}`);
  },

  async approveTransaction(id: string): Promise<Transaction> {
    const response = await api.post(`/api/v1/transactions/${id}/approve`);
    return response.data;
  },

  async rejectTransaction(id: string, motivo: string): Promise<Transaction> {
    const response = await api.post(`/api/v1/transactions/${id}/reject`, { motivo });
    return response.data;
  },

  async listInvoices(params?: InvoiceListParams): Promise<PagedInvoiceResponse> {
    const response = await api.get('/api/v1/invoices', { params });
    return response.data;
  },

  async getInvoiceById(id: string): Promise<Invoice> {
    const response = await api.get(`/api/v1/invoices/${id}`);
    return response.data;
  },

  async createInvoice(data: {
    valor: number;
    dataVencimento: string;
    clientId?: string;
    descricao?: string;
  }): Promise<Invoice> {
    const response = await api.post('/api/v1/invoices', data);
    return response.data;
  },

  async updateInvoice(
    id: string,
    data: {
      valor: number;
      dataVencimento: string;
      clientId?: string;
      descricao?: string;
    }
  ): Promise<Invoice> {
    const response = await api.put(`/api/v1/invoices/${id}`, data);
    return response.data;
  },

  async deleteInvoice(id: string): Promise<void> {
    await api.delete(`/api/v1/invoices/${id}`);
  },

  async payInvoice(id: string, valorPago: number, dataPagamento: string): Promise<Invoice> {
    const response = await api.post(`/api/v1/invoices/${id}/pay`, { valorPago, dataPagamento });
    return response.data;
  },

  async cancelInvoice(id: string): Promise<void> {
    await api.post(`/api/v1/invoices/${id}/cancel`);
  },
};
