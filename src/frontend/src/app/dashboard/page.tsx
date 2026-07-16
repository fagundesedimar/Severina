'use client';

import { useQuery } from '@tanstack/react-query';
import { dashboardApi } from '@/services/dashboardApi';
import type { DashboardResponse } from '@/types/dashboard';
import { KpiGrid } from '@/components/dashboard/KpiGrid';
import { DashboardCharts } from '@/components/dashboard/DashboardCharts';
import { ActionButtons } from '@/components/dashboard/ActionButtons';
import { ActivityFeed } from '@/components/dashboard/ActivityFeed';
import { PendingTasks } from '@/components/dashboard/PendingTasks';
import { AppShell } from '@/components/layout/AppShell';

function DashboardSkeleton() {
  return (
    <div className="animate-pulse space-y-6">
      <div className="h-8 w-64 bg-muted rounded" />
      <div className="h-4 w-96 bg-muted rounded" />
      <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-4">
        {Array.from({ length: 4 }).map((_, i) => (
          <div key={i} className="h-32 bg-muted rounded-lg" />
        ))}
      </div>
      <div className="grid grid-cols-1 lg:grid-cols-2 gap-6">
        <div className="h-64 bg-muted rounded-lg" />
        <div className="h-64 bg-muted rounded-lg" />
      </div>
    </div>
  );
}

function DashboardError() {
  return (
    <div className="bg-destructive/10 border border-destructive/20 rounded-lg p-6">
      <h3 className="text-lg font-semibold text-destructive mb-2">
        Erro ao carregar dashboard
      </h3>
      <p className="text-sm text-destructive/80">
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

  return (
    <AppShell title="Dashboard">
      {isLoading && <DashboardSkeleton />}
      {error && <DashboardError />}
      {data && (
        <div className="max-w-[1440px] mx-auto space-y-6">
          <div className="mb-8">
            <h1 className="text-2xl md:text-3xl font-bold text-foreground">
              Visão Geral do Negócio
            </h1>
            <p className="text-sm text-muted-foreground mt-1">
              Métricas consolidadas de todos os módulos
            </p>
          </div>

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
        </div>
      )}
    </AppShell>
  );
}
