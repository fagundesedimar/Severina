'use client';

import { useMemo } from 'react';
import type { Appointment } from '@/types/appointment';

interface MonthViewProps {
  currentDate: Date;
  setCurrentDate: (date: Date) => void;
  appointments: Appointment[];
  isLoading: boolean;
  onDayClick: (date: Date) => void;
}

const DAY_NAMES = ['DOM', 'SEG', 'TER', 'QUA', 'QUI', 'SEX', 'SAB'];
const MONTH_NAMES = [
  'Janeiro', 'Fevereiro', 'Março', 'Abril', 'Maio', 'Junho',
  'Julho', 'Agosto', 'Setembro', 'Outubro', 'Novembro', 'Dezembro',
];

export default function MonthView({
  currentDate,
  setCurrentDate,
  appointments,
  isLoading,
  onDayClick,
}: MonthViewProps) {
  void isLoading;
  const calendarDays = useMemo(() => {
    const year = currentDate.getFullYear();
    const month = currentDate.getMonth();

    const firstDay = new Date(year, month, 1);
    const lastDay = new Date(year, month + 1, 0);

    const startOffset = firstDay.getDay();
    const totalDays = lastDay.getDate();

    const days: Array<{ date: Date; isCurrentMonth: boolean }> = [];

    for (let i = startOffset - 1; i >= 0; i--) {
      const date = new Date(year, month, -i);
      days.push({ date, isCurrentMonth: false });
    }

    for (let i = 1; i <= totalDays; i++) {
      const date = new Date(year, month, i);
      days.push({ date, isCurrentMonth: true });
    }

    const remaining = 42 - days.length;
    for (let i = 1; i <= remaining; i++) {
      const date = new Date(year, month + 1, i);
      days.push({ date, isCurrentMonth: false });
    }

    return days;
  }, [currentDate]);

  const getAppointmentsForDay = (date: Date) => {
    return appointments.filter((apt) => {
      const aptDate = new Date(apt.dataHoraInicio);
      return (
        aptDate.getDate() === date.getDate() &&
        aptDate.getMonth() === date.getMonth() &&
        aptDate.getFullYear() === date.getFullYear()
      );
    });
  };

  const navigateMonth = (direction: number) => {
    const newDate = new Date(currentDate);
    newDate.setMonth(newDate.getMonth() + direction);
    setCurrentDate(newDate);
  };

  return (
    <div>
      <div className="flex items-center justify-between mb-3">
        <div className="flex items-center gap-2">
          <button
            onClick={() => navigateMonth(-1)}
            aria-label="Mês anterior"
            className="p-1.5 hover:bg-gray-100 rounded-lg transition-colors"
          >
            ←
          </button>
          <h2 className="text-sm font-semibold">
            {MONTH_NAMES[currentDate.getMonth()]} {currentDate.getFullYear()}
          </h2>
          <button
            onClick={() => navigateMonth(1)}
            aria-label="Próximo mês"
            className="p-1.5 hover:bg-gray-100 rounded-lg transition-colors"
          >
            →
          </button>
        </div>
        <button
          onClick={() => setCurrentDate(new Date())}
          className="px-3 py-1 text-xs bg-primary text-on-primary rounded-lg hover:bg-primary/90 transition-colors"
        >
          Hoje
        </button>
      </div>

      <div className="bg-white rounded-xl border border-gray-200 overflow-hidden">
        <div className="grid grid-cols-7 border-b border-gray-200">
          {DAY_NAMES.map((day) => (
            <div key={day} className="p-2 text-center text-xs font-medium text-gray-600">
              {day}
            </div>
          ))}
        </div>

        <div className="grid grid-cols-7">
          {calendarDays.map(({ date, isCurrentMonth }, index) => {
            const isToday = date.toDateString() === new Date().toDateString();
            const dayAppointments = getAppointmentsForDay(date);

            return (
              <div
                key={index}
                role="button"
                tabIndex={0}
                onClick={() => onDayClick(date)}
                onKeyDown={(e) => { if (e.key === 'Enter' || e.key === ' ') { e.preventDefault(); onDayClick(date); } }}
                aria-label={`${MONTH_NAMES[date.getMonth()]} ${date.getDate()}`}
                className={`min-h-[112px] p-2 border-r border-b border-gray-100 cursor-pointer hover:bg-gray-50 transition-colors outline-none focus:ring-2 focus:ring-primary ${
                  !isCurrentMonth ? 'bg-gray-50/50' : ''
                }`}
              >
                <div className="flex justify-between items-start mb-0.5">
                  <span
                    className={`text-xs ${
                      isToday
                        ? 'bg-primary text-on-primary w-5 h-5 rounded-full flex items-center justify-center'
                        : isCurrentMonth
                        ? 'text-gray-700'
                        : 'text-gray-400'
                    }`}
                  >
                    {date.getDate()}
                  </span>
                </div>

                <div className="space-y-1">
                  {dayAppointments.slice(0, 3).map((apt) => (
                    <div
                      key={apt.id}
                      className="text-xs p-1 rounded bg-primary/10 text-primary truncate"
                    >
                      {apt.titulo}
                    </div>
                  ))}
                  {dayAppointments.length > 3 && (
                    <div className="text-xs text-gray-500">
                      +{dayAppointments.length - 3} mais
                    </div>
                  )}
                </div>
              </div>
            );
          })}
        </div>
      </div>
    </div>
  );
}
