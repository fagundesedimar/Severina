'use client';

import { useQuery } from '@tanstack/react-query';
import { financialApi } from '@/services/financialApi';
import { KpiCards } from '@/components/financial/KpiCards';
import { MonthlyChart } from '@/components/financial/MonthlyChart';
import { CategoryChart } from '@/components/financial/CategoryChart';
import { RecentTransactions } from '@/components/financial/RecentTransactions';
import { useAuthStore } from '@/stores/useAuthStore';
import { useRouter } from 'next/navigation';
import { useEffect } from 'react';
import { AppShell } from '@/components/layout/AppShell';

export default function FinanceiroPage() {
  const { user } = useAuthStore();
  const router = useRouter();

  useEffect(() => {
    if (!user) router.push('/login');
  }, [user, router]);

  const { data: dashboard, isLoading, error } = useQuery({
    queryKey: ['financial-dashboard'],
    queryFn: financialApi.getDashboard,
    enabled: !!user,
    staleTime: 300_000,
  });

  if (!user) return null;

  return (
    <AppShell title="Financeiro">
      <div className="max-w-[1440px] mx-auto">
        {isLoading && <p className="text-muted-foreground">Carregando...</p>}
        {error && <p className="text-destructive">Erro ao carregar dados</p>}
        {dashboard && (
          <div className="space-y-6">
            <KpiCards kpis={dashboard.kpis} />
            <div className="grid grid-cols-1 lg:grid-cols-2 gap-6">
              <MonthlyChart data={dashboard.charts.monthlyData} />
              <CategoryChart data={dashboard.charts.categoryData} />
            </div>
            <RecentTransactions transactions={dashboard.recentTransactions} />
          </div>
        )}
      </div>
    </AppShell>
  );
}
