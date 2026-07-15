export interface Appointment {
  id: string;
  companyId: string;
  clientId?: string;
  titulo: string;
  descricao?: string;
  dataHoraInicio: string;
  dataHoraFim: string;
  tipo: TipoCompromisso;
  status: StatusCompromisso;
  recurrenceRuleJson?: string;
  serieId?: string;
  createdAt: string;
  updatedAt: string;
}

export interface PagedAppointmentResponse {
  items: Appointment[];
  totalCount: number;
  page: number;
  pageSize: number;
}

export interface CreateAppointmentRequest {
  titulo: string;
  descricao?: string;
  dataHoraInicio: string;
  dataHoraFim: string;
  tipo: TipoCompromisso;
  clientId?: string;
}

export interface UpdateAppointmentRequest {
  titulo: string;
  descricao?: string;
  dataHoraInicio: string;
  dataHoraFim: string;
  tipo: TipoCompromisso;
  clientId?: string;
}

export enum TipoCompromisso {
  Reuniao = 0,
  FollowUp = 1,
  Lembrete = 2,
  Outro = 3,
}

export enum StatusCompromisso {
  Scheduled = 0,
  Confirmed = 1,
  Completed = 2,
  Cancelled = 3,
}

export const TipoCompromissoLabels: Record<TipoCompromisso, string> = {
  [TipoCompromisso.Reuniao]: 'Reunião',
  [TipoCompromisso.FollowUp]: 'Follow-up',
  [TipoCompromisso.Lembrete]: 'Lembrete',
  [TipoCompromisso.Outro]: 'Outro',
};

export const StatusCompromissoLabels: Record<StatusCompromisso, string> = {
  [StatusCompromisso.Scheduled]: 'Agendado',
  [StatusCompromisso.Confirmed]: 'Confirmado',
  [StatusCompromisso.Completed]: 'Concluído',
  [StatusCompromisso.Cancelled]: 'Cancelado',
};

export const TipoCompromissoColors: Record<TipoCompromisso, string> = {
  [TipoCompromisso.Reuniao]: 'bg-blue-100 border-blue-500 text-blue-800',
  [TipoCompromisso.FollowUp]: 'bg-green-100 border-green-500 text-green-800',
  [TipoCompromisso.Lembrete]: 'bg-yellow-100 border-yellow-500 text-yellow-800',
  [TipoCompromisso.Outro]: 'bg-gray-100 border-gray-500 text-gray-800',
};

export const StatusCompromissoColors: Record<StatusCompromisso, string> = {
  [StatusCompromisso.Scheduled]: 'bg-gray-100 text-gray-700',
  [StatusCompromisso.Confirmed]: 'bg-green-100 text-green-700',
  [StatusCompromisso.Completed]: 'bg-blue-100 text-blue-700',
  [StatusCompromisso.Cancelled]: 'bg-red-100 text-red-700',
};
