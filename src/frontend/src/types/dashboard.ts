export interface DashboardResponse {
  kpis: KpisDto;
  charts: ChartsDto;
  activities: ActivityDto[];
  pendingTasks: PendingTaskDto[];
}

export interface KpisDto {
  atendimentosHoje: number;
  atendimentosPendentes: number;
  taxaConversao: number;
  tempoMedioResposta: number;
  clientesAtivos: number;
  novosClientes: number;
  faturamento: number;
  despesas: number;
  saldo: number;
  compromissosHoje: number;
  atendimentosTrend?: TrendDto;
  faturamentoTrend?: TrendDto;
  clientesTrend?: TrendDto;
}

export interface TrendDto {
  value: number;
  direction: 'Up' | 'Down' | 'Neutral';
}

export interface ChartsDto {
  barData: BarChartItem[];
  pieData: PieChartItem[];
  lineData: LineChartItem[];
}

export interface BarChartItem {
  label: string;
  value: number;
}

export interface PieChartItem {
  label: string;
  value: number;
  color: string;
}

export interface LineChartItem {
  label: string;
  value: number;
  previousValue?: number;
}

export interface ActivityDto {
  id: string;
  type: string;
  description: string;
  timestamp: string;
  sourceUrl?: string;
}

export interface PendingTaskDto {
  id: string;
  type: string;
  title: string;
  priority: TaskPriority;
  dueDate?: string;
  sourceUrl?: string;
}

export enum TaskPriority {
  Overdue = 0,
  Pending = 1,
  Upcoming = 2,
}
