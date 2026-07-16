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
    <div className="min-h-screen bg-surface dark:bg-dark-surface px-6 py-8">
      <h1 className="text-2xl font-bold text-on-surface mb-6">Financeiro</h1>
      {isLoading && <p className="text-on-surface/50">Carregando...</p>}
      {error && <p className="text-red-500">Erro ao carregar dados</p>}
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
  );
}
