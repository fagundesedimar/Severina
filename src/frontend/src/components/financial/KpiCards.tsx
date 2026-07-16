'use client';

import type { FinancialKpis } from '@/types/financial';

interface KpiCardsProps {
  kpis: FinancialKpis;
}

export function KpiCards({ kpis }: KpiCardsProps) {
  const formatCurrency = (value: number) =>
    new Intl.NumberFormat('pt-BR', { style: 'currency', currency: 'BRL' }).format(value);

  return (
    <div className="grid grid-cols-2 md:grid-cols-4 gap-4">
      <div className="bg-surface-container-lowest dark:bg-surface-container border border-outline-variant dark:border-outline p-4 rounded-xl">
        <p className="text-xs font-semibold text-on-surface/50 uppercase">Saldo</p>
        <h3 className="text-2xl font-bold text-on-surface mt-1 font-mono">{formatCurrency(kpis.saldo)}</h3>
      </div>
      <div className="bg-surface-container-lowest dark:bg-surface-container border border-outline-variant dark:border-outline p-4 rounded-xl">
        <p className="text-xs font-semibold text-on-surface/50 uppercase">Receitas do Mês</p>
        <h3 className="text-2xl font-bold text-emerald-600 mt-1 font-mono">{formatCurrency(kpis.receitasMes)}</h3>
      </div>
      <div className="bg-surface-container-lowest dark:bg-surface-container border border-outline-variant dark:border-outline p-4 rounded-xl">
        <p className="text-xs font-semibold text-on-surface/50 uppercase">Despesas do Mês</p>
        <h3 className="text-2xl font-bold text-red-600 mt-1 font-mono">{formatCurrency(kpis.despesasMes)}</h3>
      </div>
      <div className="bg-surface-container-lowest dark:bg-surface-container border border-outline-variant dark:border-outline p-4 rounded-xl">
        <p className="text-xs font-semibold text-on-surface/50 uppercase">Contas Pendentes</p>
        <h3 className="text-2xl font-bold text-amber-600 mt-1 font-mono">{kpis.contasPendentes}</h3>
        {kpis.contasAtrasadas > 0 && (
          <p className="text-xs text-red-500 mt-1">{kpis.contasAtrasadas} atrasadas</p>
        )}
      </div>
    </div>
  );
}
