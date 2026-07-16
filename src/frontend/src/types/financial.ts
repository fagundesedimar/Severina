export interface Transaction {
  id: string;
  companyId: string;
  clientId: string | null;
  tipo: 'Receita' | 'Despesa';
  valor: number;
  data: string;
  categoria: 'Servicos' | 'Materiais' | 'Frente' | 'Impostos' | 'Outros';
  descricao: string | null;
  status: 'Pending' | 'Approved' | 'Rejected';
  createdAt: string;
  updatedAt: string;
}

export interface PagedTransactionResponse {
  items: Transaction[];
  totalCount: number;
  page: number;
  pageSize: number;
}

export interface Invoice {
  id: string;
  companyId: string;
  clientId: string | null;
  numero: string;
  valor: number;
  valorPago: number;
  dataVencimento: string;
  dataPagamento: string | null;
  descricao: string | null;
  status: 'Pending' | 'Partial' | 'Paid' | 'Overdue' | 'Cancelled';
  createdAt: string;
  updatedAt: string;
}

export interface PagedInvoiceResponse {
  items: Invoice[];
  totalCount: number;
  page: number;
  pageSize: number;
}

export interface FinancialKpis {
  saldo: number;
  receitasMes: number;
  despesasMes: number;
  previsaoProximoMes: number;
  contasPendentes: number;
  contasAtrasadas: number;
}

export interface MonthlyData {
  mes: string;
  receitas: number;
  despesas: number;
}

export interface CategoryData {
  categoria: string;
  valor: number;
  percentual: number;
}

export interface FinancialDashboard {
  kpis: FinancialKpis;
  charts: {
    monthlyData: MonthlyData[];
    categoryData: CategoryData[];
  };
  recentTransactions: Transaction[];
}

export interface TransactionListParams {
  page?: number;
  pageSize?: number;
  tipo?: string;
  categoria?: string;
  from?: string;
  to?: string;
  clientId?: string;
}

export interface InvoiceListParams {
  page?: number;
  pageSize?: number;
  status?: string;
  clientId?: string;
  from?: string;
  to?: string;
}
