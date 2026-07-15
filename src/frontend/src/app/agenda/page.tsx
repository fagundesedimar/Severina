'use client';

import { useEffect, useState } from 'react';
import { useRouter } from 'next/navigation';
import { useQuery } from '@tanstack/react-query';
import { useAuthStore } from '@/stores/useAuthStore';
import { appointmentApi } from '@/services/appointmentApi';
import WeekView from '@/components/schedule/WeekView';
import MonthView from '@/components/schedule/MonthView';

export default function AgendaPage() {
  const router = useRouter();
  const { isAuthenticated } = useAuthStore();
  const [viewMode, setViewMode] = useState<'week' | 'month'>('week');
  const [currentDate, setCurrentDate] = useState(new Date());

  useEffect(() => {
    if (!isAuthenticated) {
      router.push('/login');
    }
  }, [isAuthenticated, router]);

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
    <div className="min-h-screen bg-surface">
      <header className="bg-white dark:bg-gray-800 shadow sticky top-0 z-50">
        <div className="max-w-7xl mx-auto px-4 py-4 flex justify-between items-center">
          <h1 className="text-xl font-bold text-on-surface">Agenda</h1>
          <div className="flex items-center gap-4">
            <div className="flex bg-gray-100 dark:bg-gray-700 rounded-lg p-1">
              <button
                onClick={() => setViewMode('week')}
                aria-pressed={viewMode === 'week'}
                className={`px-4 py-2 text-sm rounded-md transition-colors ${
                  viewMode === 'week'
                    ? 'bg-white dark:bg-gray-600 text-primary shadow-sm'
                    : 'text-gray-600 dark:text-gray-300 hover:text-gray-800'
                }`}
              >
                Semana
              </button>
              <button
                onClick={() => setViewMode('month')}
                aria-pressed={viewMode === 'month'}
                className={`px-4 py-2 text-sm rounded-md transition-colors ${
                  viewMode === 'month'
                    ? 'bg-white dark:bg-gray-600 text-primary shadow-sm'
                    : 'text-gray-600 dark:text-gray-300 hover:text-gray-800'
                }`}
              >
                Mês
              </button>
            </div>
          </div>
        </div>
      </header>

      <main className="max-w-7xl mx-auto px-4 py-6">
        {viewMode === 'week' ? (
          <WeekView
            currentDate={currentDate}
            setCurrentDate={setCurrentDate}
            appointments={appointments?.items || []}
            isLoading={isLoading}
            onDateClick={(date) => {
              setCurrentDate(date);
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
      </main>

      <button
        onClick={() => router.push('/agenda/novo')}
        aria-label="Novo compromisso"
        className="fixed bottom-20 right-4 bg-primary text-on-primary shadow-xl w-14 h-14 rounded-full flex items-center justify-center hover:opacity-90 transition-opacity z-40"
      >
        <span className="text-2xl">+</span>
      </button>
    </div>
  );
}
