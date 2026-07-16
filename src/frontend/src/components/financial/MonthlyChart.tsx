'use client';

import type { MonthlyData } from '@/types/financial';

interface MonthlyChartProps {
  data: MonthlyData[];
}

export function MonthlyChart({ data }: MonthlyChartProps) {
  const maxValue = Math.max(...data.flatMap((d) => [d.receitas, d.despesas]), 1);
  const barHeight = 160;

  return (
    <div className="bg-surface-container-lowest dark:bg-surface-container border border-outline-variant dark:border-outline rounded-xl p-6">
      <h3 className="text-sm font-semibold text-on-surface mb-4">Receitas vs Despesas (12 meses)</h3>
      <div className="flex items-end gap-2 h-48">
        {data.map((item, i) => (
          <div key={i} className="flex-1 flex flex-col items-center gap-1">
            <div className="flex gap-1 items-end" style={{ height: barHeight }}>
              <div
                className="w-3 bg-emerald-500 rounded-t"
                style={{ height: `${(item.receitas / maxValue) * barHeight}px`, minHeight: item.receitas > 0 ? 2 : 0 }}
              />
              <div
                className="w-3 bg-red-500 rounded-t"
                style={{ height: `${(item.despesas / maxValue) * barHeight}px`, minHeight: item.despesas > 0 ? 2 : 0 }}
              />
            </div>
            <span className="text-[10px] text-on-surface/50">{item.mes}</span>
          </div>
        ))}
      </div>
      <div className="flex justify-center gap-4 mt-4">
        <div className="flex items-center gap-2">
          <div className="w-3 h-3 bg-emerald-500 rounded" />
          <span className="text-xs text-on-surface/60">Receitas</span>
        </div>
        <div className="flex items-center gap-2">
          <div className="w-3 h-3 bg-red-500 rounded" />
          <span className="text-xs text-on-surface/60">Despesas</span>
        </div>
      </div>
    </div>
  );
}
