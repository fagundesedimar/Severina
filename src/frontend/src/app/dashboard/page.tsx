'use client';

import { useQuery } from '@tanstack/react-query';
import { dashboardApi } from '@/services/dashboardApi';
import type { DashboardResponse } from '@/types/dashboard';
import { KpiGrid } from '@/components/dashboard/KpiGrid';
import { DashboardCharts } from '@/components/dashboard/DashboardCharts';
import { ActionButtons } from '@/components/dashboard/ActionButtons';
import { ActivityFeed } from '@/components/dashboard/ActivityFeed';
import { PendingTasks } from '@/components/dashboard/PendingTasks';

function DashboardSkeleton() {
  return (
    <div className="animate-pulse space-y-6">
      <div className="h-8 w-64 bg-gray-200 dark:bg-gray-700 rounded" />
      <div className="h-4 w-96 bg-gray-200 dark:bg-gray-700 rounded" />
      <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-4">
        {Array.from({ length: 4 }).map((_, i) => (
          <div key={i} className="h-32 bg-gray-200 dark:bg-gray-700 rounded-xl" />
        ))}
      </div>
      <div className="grid grid-cols-1 lg:grid-cols-2 gap-6">
        <div className="h-64 bg-gray-200 dark:bg-gray-700 rounded-xl" />
        <div className="h-64 bg-gray-200 dark:bg-gray-700 rounded-xl" />
      </div>
    </div>
  );
}

function DashboardError() {
  return (
    <div className="bg-red-50 dark:bg-red-900/20 border border-red-200 dark:border-red-800 rounded-xl p-6">
      <h3 className="text-lg font-semibold text-red-700 dark:text-red-400 mb-2">
        Erro ao carregar dashboard
      </h3>
      <p className="text-sm text-red-600 dark:text-red-300">
        Alguns dados podem estar indisponíveis. Tente novamente em alguns instantes.
      </p>
    </div>
  );
}

export default function DashboardPage() {
  const { data, isLoading, error } = useQuery<DashboardResponse>({
    queryKey: ['dashboard'],
    queryFn: () => dashboardApi.get(),
    refetchInterval: 5 * 60 * 1000,
    staleTime: 2 * 60 * 1000,
  });

  if (isLoading) {
    return (
      <div className="min-h-screen bg-surface p-4 md:p-6">
        <DashboardSkeleton />
      </div>
    );
  }

  if (error) {
    return (
      <div className="min-h-screen bg-surface p-4 md:p-6">
        <DashboardError />
      </div>
    );
  }

  return (
    <div className="min-h-screen bg-surface">
      <header className="mb-8 px-4 md:px-6 pt-4 md:pt-6">
        <h1 className="text-2xl md:text-3xl font-bold text-on-surface">
          Visão Geral do Negócio
        </h1>
        <p className="text-sm text-on-surface/60 mt-1">
          Métricas consolidadas de todos os módulos
        </p>
      </header>

      <main className="px-4 md:px-6 max-w-[1440px] mx-auto space-y-6">
        <ActionButtons />

        {data?.kpis && <KpiGrid kpis={data.kpis} />}

        <div className="grid grid-cols-1 lg:grid-cols-2 gap-6">
          <div className="space-y-6">
            {data?.charts && <DashboardCharts charts={data.charts} />}
          </div>
          <div className="space-y-6">
            {data?.activities && <ActivityFeed activities={data.activities} />}
            {data?.pendingTasks && (
              <PendingTasks
                tasks={data.pendingTasks}
                totalCount={data.pendingTasks.length}
              />
            )}
          </div>
        </div>
      </main>
    </div>
  );
}
