'use client';

import type { KpisDto, TrendDto } from '@/types/dashboard';

interface KpiCardProps {
  icon: string;
  label: string;
  value: string;
  trend?: TrendDto;
  subtitle?: string;
}

function TrendIndicator({ trend }: { trend?: TrendDto }) {
  if (!trend) return null;

  const colors = {
    Up: 'text-green-500',
    Down: 'text-red-500',
    Neutral: 'text-gray-500',
  };

  const icons = {
    Up: 'trending_up',
    Down: 'trending_down',
    Neutral: 'remove',
  };

  return (
    <span className={`flex items-center gap-1 text-xs font-medium ${colors[trend.direction]}`}>
      <span className="material-symbols-outlined text-sm">{icons[trend.direction]}</span>
      {trend.value}%
    </span>
  );
}

function formatCurrency(value: number): string {
  return new Intl.NumberFormat('pt-BR', {
    style: 'currency',
    currency: 'BRL',
  }).format(value);
}

function formatPercentage(value: number): string {
  return `${value.toFixed(1)}%`;
}

function formatTime(minutes: number): string {
  if (minutes === 0) return '--';
  if (minutes < 60) return `${minutes}min`;
  const hours = Math.floor(minutes / 60);
  const mins = minutes % 60;
  return mins > 0 ? `${hours}h${mins}min` : `${hours}h`;
}

export function KpiCard({ icon, label, value, trend, subtitle }: KpiCardProps) {
  return (
    <div className="bg-white dark:bg-gray-800 border border-gray-200 dark:border-gray-700 p-5 rounded-xl relative overflow-hidden group hover:shadow-md transition-shadow">
      <div className="absolute -right-4 -bottom-4 opacity-5 dark:opacity-10 transform group-hover:scale-110 transition-transform">
        <span className="material-symbols-outlined text-[100px] text-primary">{icon}</span>
      </div>
      <span className="text-[11px] font-semibold text-gray-500 dark:text-gray-400 uppercase tracking-widest block mb-2">
        {label}
      </span>
      <div className="flex items-baseline gap-2 mb-3">
        <span className="text-2xl font-bold text-on-surface font-mono">{value}</span>
        <TrendIndicator trend={trend} />
      </div>
      {subtitle && (
        <p className="text-xs text-gray-500 dark:text-gray-400">{subtitle}</p>
      )}
    </div>
  );
}

export function KpiGrid({ kpis }: { kpis: KpisDto }) {
  const cards: KpiCardProps[] = [
    {
      icon: 'calendar_month',
      label: 'AGENDAMENTOS HOJE',
      value: kpis.atendimentosHoje.toString(),
      trend: kpis.atendimentosTrend,
      subtitle: `${kpis.atendimentosPendentes} pendentes`,
    },
    {
      icon: 'bolt',
      label: 'TAXA DE CONVERSÃO',
      value: formatPercentage(kpis.taxaConversao),
      subtitle: 'Mês atual',
    },
    {
      icon: 'group',
      label: 'CLIENTES ATIVOS',
      value: kpis.clientesAtivos.toString(),
      trend: kpis.clientesTrend,
      subtitle: `+${kpis.novosClientes} novos este mês`,
    },
    {
      icon: 'payments',
      label: 'SALDO DO MÊS',
      value: formatCurrency(kpis.saldo),
      trend: kpis.faturamentoTrend,
      subtitle: `${formatCurrency(kpis.faturamento)} receita`,
    },
    {
      icon: 'timer',
      label: 'TEMPO RESPOSTA',
      value: formatTime(kpis.tempoMedioResposta),
      subtitle: 'Média de primeira resposta',
    },
    {
      icon: 'event',
      label: 'COMPROMISSOS HOJE',
      value: kpis.compromissosHoje.toString(),
      subtitle: 'Agenda do dia',
    },
    {
      icon: 'account_balance',
      label: 'RECEITA',
      value: formatCurrency(kpis.faturamento),
      subtitle: 'Faturamento do mês',
    },
    {
      icon: 'receipt_long',
      label: 'DESPESAS',
      value: formatCurrency(kpis.despesas),
      subtitle: 'Despesas do mês',
    },
  ];

  return (
    <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-4 gap-4">
      {cards.map((card) => (
        <KpiCard key={card.label} {...card} />
      ))}
    </div>
  );
}
