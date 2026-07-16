'use client';

import { useMemo } from 'react';
import type { CategoryData } from '@/types/financial';

interface CategoryChartProps {
  data: CategoryData[];
}

const COLORS = ['#004ac6', '#2563eb', '#60a5fa', '#93c5fd', '#bfdbfe', '#dbeafe'];

interface Segment {
  path: string;
  color: string;
  label: string;
  percentual: number;
}

function computeSegments(data: CategoryData[]): Segment[] {
  const total = data.reduce((sum, d) => sum + d.valor, 0);
  let accumulated = 0;

  return data.map((item, i) => {
    const angle = (item.valor / total) * 360;
    const startAngle = accumulated;
    accumulated += angle;

    const startRad = (startAngle - 90) * (Math.PI / 180);
    const endRad = (startAngle + angle - 90) * (Math.PI / 180);
    const x1 = 50 + 40 * Math.cos(startRad);
    const y1 = 50 + 40 * Math.sin(startRad);
    const x2 = 50 + 40 * Math.cos(endRad);
    const y2 = 50 + 40 * Math.sin(endRad);
    const largeArc = angle > 180 ? 1 : 0;

    return {
      path: `M 50 50 L ${x1} ${y1} A 40 40 0 ${largeArc} 1 ${x2} ${y2} Z`,
      color: COLORS[i % COLORS.length],
      label: item.categoria,
      percentual: item.percentual,
    };
  });
}

export function CategoryChart({ data }: CategoryChartProps) {
  const segments = useMemo(() => computeSegments(data), [data]);

  const hasData = data.length > 0;

  return (
    <div className="bg-surface-container-lowest dark:bg-surface-container border border-outline-variant dark:border-outline rounded-xl p-6">
      <h3 className="text-sm font-semibold text-on-surface mb-4">Despesas por Categoria</h3>
      {!hasData ? (
        <p className="text-sm text-on-surface/50 text-center py-8">Nenhuma despesa registrada</p>
      ) : (
        <div className="flex items-center gap-6">
          <svg viewBox="0 0 100 100" className="w-32 h-32">
            {segments.map((seg, i) => (
              <path key={i} d={seg.path} fill={seg.color} />
            ))}
          </svg>
          <div className="space-y-2">
            {segments.map((seg, i) => (
              <div key={i} className="flex items-center gap-2">
                <div className="w-3 h-3 rounded" style={{ backgroundColor: seg.color }} />
                <span className="text-xs text-on-surface/70">{seg.label}</span>
                <span className="text-xs font-mono text-on-surface/50">{seg.percentual}%</span>
              </div>
            ))}
          </div>
        </div>
      )}
    </div>
  );
}
