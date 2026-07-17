'use client';

import type { ChartsDto } from '@/types/dashboard';

function BarChart({ data }: { data: ChartsDto['barData'] }) {
  const maxValue = Math.max(...data.map((d) => d.value), 1);

  return (
    <div className="bg-white dark:bg-gray-800 border border-gray-200 dark:border-gray-700 rounded-xl p-3 h-full flex flex-col">
      <h3 className="text-xs font-semibold text-on-surface mb-1 flex items-center gap-2">
        <span className="material-symbols-outlined text-primary text-sm">bar_chart</span>
        Atendimentos por Dia
      </h3>
      <div className="flex items-end gap-1 flex-1 overflow-x-auto">
        {data.map((item, i) => (
          <div key={i} className="flex flex-col items-center min-w-[12px] group">
            <div className="relative w-full flex justify-center">
              <div className="absolute -top-8 bg-gray-900 text-white text-[10px] px-2 py-1 rounded opacity-0 group-hover:opacity-100 transition-opacity whitespace-nowrap z-10">
                {item.label}: {item.value}
              </div>
            </div>
            <div
              className="w-full bg-primary rounded-t transition-all duration-300 hover:bg-primary/80"
              style={{ height: `${(item.value / maxValue) * 100}%`, minHeight: item.value > 0 ? '4px' : '0' }}
            />
            {i % 5 === 0 && (
              <span className="text-[9px] text-gray-500 mt-1 whitespace-nowrap">{item.label}</span>
            )}
          </div>
        ))}
      </div>
    </div>
  );
}

function PieChart({ data }: { data: ChartsDto['pieData'] }) {
  const total = data.reduce((sum, d) => sum + d.value, 0);

  const segments = data.reduce<{ percent: number; dashOffset: number }[]>((acc, item) => {
    const percent = (item.value / total) * 100;
    const dashOffset = acc.length > 0 ? acc[acc.length - 1].dashOffset + acc[acc.length - 1].percent : 0;
    acc.push({ percent, dashOffset });
    return acc;
  }, []);

  return (
    <div className="bg-white dark:bg-gray-800 border border-gray-200 dark:border-gray-700 rounded-xl p-3 h-full flex flex-col">
      <h3 className="text-xs font-semibold text-on-surface mb-1 flex items-center gap-2">
        <span className="material-symbols-outlined text-primary text-sm">pie_chart</span>
        Atendimentos por Origem
      </h3>
      <div className="flex items-center gap-3 flex-1">
        <div className="relative w-24 h-24 flex-shrink-0">
          <svg viewBox="0 0 100 100" className="w-full h-full -rotate-90">
            {data.map((item, i) => {
              const { percent, dashOffset } = segments[i];
              const dashArray = `${percent} ${100 - percent}`;
              return (
                <circle
                  key={i}
                  cx="50"
                  cy="50"
                  r="40"
                  fill="none"
                  stroke={item.color}
                  strokeWidth="20"
                  strokeDasharray={dashArray}
                  strokeDashoffset={-dashOffset}
                  className="transition-all duration-500"
                />
              );
            })}
          </svg>
          <div className="absolute inset-0 flex items-center justify-center">
            <span className="text-sm font-bold text-on-surface">{total}</span>
          </div>
        </div>
        <div className="space-y-1 flex-1 justify-center">
          {data.map((item, i) => (
            <div key={i} className="flex items-center justify-between text-xs">
              <div className="flex items-center gap-1.5">
                <div className="w-2.5 h-2.5 rounded-full" style={{ backgroundColor: item.color }} />
                <span className="text-gray-700 dark:text-gray-300">{item.label}</span>
              </div>
              <span className="font-medium text-on-surface">{item.value}</span>
            </div>
          ))}
        </div>
      </div>
    </div>
  );
}

function LineChart({ data }: { data: ChartsDto['lineData'] }) {
  const maxValue = Math.max(...data.map((d) => d.value), 1);
  const width = 100;
  const height = 40;
  const padding = 2;

  const points = data.map((d, i) => {
    const x = padding + (i / (data.length - 1)) * (width - padding * 2);
    const y = height - padding - (d.value / maxValue) * (height - padding * 2);
    return `${x},${y}`;
  });

  const pathD = `M ${points.join(' L ')}`;

  return (
    <div className="bg-white dark:bg-gray-800 border border-gray-200 dark:border-gray-700 rounded-xl p-3 h-full flex flex-col">
      <h3 className="text-xs font-semibold text-on-surface mb-1 flex items-center gap-2">
        <span className="material-symbols-outlined text-primary text-sm">show_chart</span>
        Tendência de Atendimentos
      </h3>
      <div className="overflow-x-auto flex-1 flex flex-col">
        <svg viewBox={`0 0 ${width} ${height}`} className="w-full flex-1" preserveAspectRatio="none">
          <defs>
            <linearGradient id="lineGradient" x1="0%" y1="0%" x2="0%" y2="100%">
              <stop offset="0%" stopColor="rgb(var(--color-primary-rgb))" stopOpacity="0.3" />
              <stop offset="100%" stopColor="rgb(var(--color-primary-rgb))" stopOpacity="0" />
            </linearGradient>
          </defs>
          <path
            d={`${pathD} L ${width - padding},${height - padding} L ${padding},${height - padding} Z`}
            fill="url(#lineGradient)"
          />
          <path d={pathD} fill="none" stroke="rgb(var(--color-primary-rgb))" strokeWidth="0.5" />
          {data.map((d, i) => {
            const x = padding + (i / (data.length - 1)) * (width - padding * 2);
            return i % 5 === 0 ? (
              <text key={i} x={x} y={height - 0.5} textAnchor="middle" className="fill-gray-500" fontSize="2">
                {d.label}
              </text>
            ) : null;
          })}
        </svg>
      </div>
    </div>
  );
}

export function DashboardCharts({ charts }: { charts: ChartsDto }) {
  return (
    <div className="grid grid-rows-[1.3fr_1fr] gap-3 h-full">
      <BarChart data={charts.barData} />
      <div className="grid grid-cols-1 md:grid-cols-2 gap-3">
        <PieChart data={charts.pieData} />
        <LineChart data={charts.lineData} />
      </div>
    </div>
  );
}
