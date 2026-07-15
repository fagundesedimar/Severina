'use client';

import { useMemo } from 'react';
import type { Appointment } from '@/types/appointment';
import AppointmentCard from './AppointmentCard';
import CurrentTimeIndicator from './CurrentTimeIndicator';

interface WeekViewProps {
  currentDate: Date;
  setCurrentDate: (date: Date) => void;
  appointments: Appointment[];
  isLoading: boolean;
  onDateClick: (date: Date) => void;
}

const HOURS = Array.from({ length: 13 }, (_, i) => i + 8);
const DAY_NAMES = ['DOM', 'SEG', 'TER', 'QUA', 'QUI', 'SEX', 'SAB'];

export default function WeekView({
  currentDate,
  setCurrentDate,
  appointments,
  isLoading,
  onDateClick,
}: WeekViewProps) {
  void isLoading;
  const weekDays = useMemo(() => {
    const start = new Date(currentDate);
    start.setDate(start.getDate() - start.getDay() + 1);
    start.setHours(0, 0, 0, 0);

    return Array.from({ length: 7 }, (_, i) => {
      const date = new Date(start);
      date.setDate(date.getDate() + i);
      return date;
    });
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

  const getAppointmentsForHour = (date: Date, hour: number) => {
    const dayAppointments = getAppointmentsForDay(date);
    return dayAppointments.filter((apt) => {
      const startHour = new Date(apt.dataHoraInicio).getHours();
      return startHour === hour;
    });
  };

  const navigateWeek = (direction: number) => {
    const newDate = new Date(currentDate);
    newDate.setDate(newDate.getDate() + direction * 7);
    setCurrentDate(newDate);
  };

  const goToToday = () => {
    setCurrentDate(new Date());
  };

  const formatDateRange = () => {
    const start = weekDays[0];
    const end = weekDays[6];
    const monthNames = [
      'Janeiro', 'Fevereiro', 'Março', 'Abril', 'Maio', 'Junho',
      'Julho', 'Agosto', 'Setembro', 'Outubro', 'Novembro', 'Dezembro',
    ];

    if (start.getMonth() === end.getMonth()) {
      return `${start.getDate()} - ${end.getDate()} de ${monthNames[start.getMonth()]} ${start.getFullYear()}`;
    }

    return `${start.getDate()} ${monthNames[start.getMonth()].substring(0, 3)} - ${end.getDate()} ${monthNames[end.getMonth()].substring(0, 3)} ${end.getFullYear()}`;
  };

  return (
    <div>
      <div className="flex items-center justify-between mb-6">
        <div className="flex items-center gap-2">
          <button
            onClick={() => navigateWeek(-1)}
            aria-label="Semana anterior"
            className="p-2 hover:bg-gray-100 rounded-lg transition-colors"
          >
            ←
          </button>
          <h2 className="text-lg font-semibold">{formatDateRange()}</h2>
          <button
            onClick={() => navigateWeek(1)}
            aria-label="Próxima semana"
            className="p-2 hover:bg-gray-100 rounded-lg transition-colors"
          >
            →
          </button>
        </div>
        <button
          onClick={goToToday}
          className="px-4 py-2 text-sm bg-primary text-white rounded-lg hover:bg-primary/90 transition-colors"
        >
          Hoje
        </button>
      </div>

      <div className="flex gap-2 overflow-x-auto pb-4 mb-6">
        {weekDays.map((date, index) => {
          const isToday = date.toDateString() === new Date().toDateString();
          const hasAppointments = getAppointmentsForDay(date).length > 0;

          return (
            <button
              key={index}
              onClick={() => onDateClick(date)}
              aria-label={`${DAY_NAMES[date.getDay()]} ${date.getDate()}`}
              aria-current={isToday ? 'date' : undefined}
              className={`flex flex-col items-center justify-center min-w-[56px] h-20 rounded-xl border transition-all ${
                isToday
                  ? 'border-primary bg-primary/5 text-primary'
                  : 'border-gray-200 bg-white hover:border-gray-300'
              }`}
            >
              <span className="text-xs text-gray-500">{DAY_NAMES[date.getDay()]}</span>
              <span className={`text-lg font-semibold ${isToday ? 'text-primary' : ''}`}>
                {date.getDate()}
              </span>
              {hasAppointments && (
                <div className="w-1.5 h-1.5 bg-primary rounded-full mt-1" />
              )}
            </button>
          );
        })}
      </div>

      <div className="bg-white rounded-xl border border-gray-200 overflow-x-auto">
        <div className="grid grid-cols-[60px_repeat(7,1fr)] min-w-[600px] border-b border-gray-200">
          <div className="p-2" />
          {weekDays.map((date, index) => {
            const isToday = date.toDateString() === new Date().toDateString();
            return (
              <div
                key={index}
                className={`p-2 text-center text-sm font-medium ${
                  isToday ? 'text-primary bg-primary/5' : 'text-gray-600'
                }`}
              >
                {DAY_NAMES[date.getDay()]} {date.getDate()}
              </div>
            );
          })}
        </div>

        <div className="grid grid-cols-[60px_repeat(7,1fr)] min-w-[600px] relative">
          {HOURS.map((hour) => (
            <div key={hour} className="contents">
              <div className="p-2 text-xs text-gray-500 text-right border-r border-gray-100">
                {hour.toString().padStart(2, '0')}:00
              </div>
              {weekDays.map((date, dayIndex) => {
                const hourAppointments = getAppointmentsForHour(date, hour);
                return (
                  <div
                    key={dayIndex}
                    className="border-r border-b border-gray-100 p-1 min-h-[60px] hover:bg-gray-50 transition-colors"
                  >
                    {hourAppointments.map((apt) => (
                      <AppointmentCard key={apt.id} appointment={apt} />
                    ))}
                  </div>
                );
              })}
            </div>
          ))}

          <CurrentTimeIndicator
            startHour={HOURS[0]}
            endHour={HOURS[HOURS.length - 1] + 1}
          />
        </div>
      </div>
    </div>
  );
}
