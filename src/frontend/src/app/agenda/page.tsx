'use client';

import { useState } from 'react';
import { useRouter } from 'next/navigation';
import { useQuery } from '@tanstack/react-query';
import { useAuthStore } from '@/stores/useAuthStore';
import { appointmentApi } from '@/services/appointmentApi';
import WeekView from '@/components/schedule/WeekView';
import MonthView from '@/components/schedule/MonthView';
import { AppShell } from '@/components/layout/AppShell';
import { Button } from '@/components/ui/button';

export default function AgendaPage() {
  const router = useRouter();
  const { isAuthenticated } = useAuthStore();
  const [viewMode, setViewMode] = useState<'week' | 'month'>('week');
  const [currentDate, setCurrentDate] = useState(new Date());

  const { data: appointments, isLoading } = useQuery({
    queryKey: ['appointments', currentDate, viewMode],
    queryFn: () => {
      const from = new Date(currentDate);
      const to = new Date(currentDate);

      if (viewMode === 'week') {
        from.setDate(from.getDate() - from.getDay() + 1);
        to.setDate(from.getDate() + 6);
      } else {
        from.setDate(1);
        to.setMonth(to.getMonth() + 1);
        to.setDate(0);
      }

      from.setHours(0, 0, 0, 0);
      to.setHours(23, 59, 59, 999);

      return appointmentApi.list({
        from: from.toISOString(),
        to: to.toISOString(),
      });
    },
    enabled: isAuthenticated,
  });

  if (!isAuthenticated) {
    return null;
  }

  return (
    <AppShell
      title="Agenda"
      actions={
        <div className="flex items-center gap-2">
          <div className="flex bg-muted rounded-lg p-1">
            <button
              onClick={() => setViewMode('week')}
              className={`px-3 py-1.5 text-sm rounded-md transition-colors ${
                viewMode === 'week'
                  ? 'bg-background text-foreground shadow-sm'
                  : 'text-muted-foreground hover:text-foreground'
              }`}
            >
              Semana
            </button>
            <button
              onClick={() => setViewMode('month')}
              className={`px-3 py-1.5 text-sm rounded-md transition-colors ${
                viewMode === 'month'
                  ? 'bg-background text-foreground shadow-sm'
                  : 'text-muted-foreground hover:text-foreground'
              }`}
            >
              Mês
            </button>
          </div>
        </div>
      }
    >
      <div className="max-w-[1440px] mx-auto">
        {viewMode === 'week' ? (
          <WeekView
            currentDate={currentDate}
            setCurrentDate={setCurrentDate}
            appointments={appointments?.items || []}
            isLoading={isLoading}
            onDateClick={(date) => {
              const yyyy = date.getFullYear();
              const mm = String(date.getMonth() + 1).padStart(2, '0');
              const dd = String(date.getDate()).padStart(2, '0');
              router.push(`/agenda/novo?data=${yyyy}-${mm}-${dd}`);
            }}
          />
        ) : (
          <MonthView
            currentDate={currentDate}
            setCurrentDate={setCurrentDate}
            appointments={appointments?.items || []}
            isLoading={isLoading}
            onDayClick={(date) => {
              setCurrentDate(date);
              setViewMode('week');
            }}
          />
        )}
      </div>

      <button
        onClick={() => router.push('/agenda/novo')}
        aria-label="Novo compromisso"
        className="fixed bottom-24 right-4 lg:bottom-8 lg:right-8 bg-primary text-primary-foreground shadow-xl w-14 h-14 rounded-full flex items-center justify-center hover:opacity-90 transition-opacity z-40"
      >
        <span className="material-symbols-outlined text-2xl">add</span>
      </button>
    </AppShell>
  );
}
