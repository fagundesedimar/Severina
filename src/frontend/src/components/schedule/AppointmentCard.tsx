'use client';

import type { Appointment } from '@/types/appointment';
import { TipoCompromissoLabels, TipoCompromissoColors } from '@/types/appointment';

interface AppointmentCardProps {
  appointment: Appointment;
  onClick?: () => void;
}

export default function AppointmentCard({ appointment, onClick }: AppointmentCardProps) {
  const startTime = new Date(appointment.dataHoraInicio);
  const endTime = new Date(appointment.dataHoraFim);

  const formatTime = (date: Date) => {
    return date.toLocaleTimeString('pt-BR', { hour: '2-digit', minute: '2-digit' });
  };

  const tipoColor = TipoCompromissoColors[appointment.tipo] || 'bg-gray-100 border-gray-500 text-gray-800';

  return (
    <div
      role="button"
      tabIndex={0}
      onClick={onClick}
      onKeyDown={(e) => { if ((e.key === 'Enter' || e.key === ' ') && onClick) { e.preventDefault(); onClick(); } }}
      aria-label={`${appointment.titulo}, ${TipoCompromissoLabels[appointment.tipo]}, ${formatTime(startTime)} até ${formatTime(endTime)}`}
      className={`p-2 rounded-lg border-l-4 cursor-pointer hover:opacity-80 transition-opacity outline-none focus:ring-2 focus:ring-primary ${tipoColor}`}
    >
      <div className="flex items-center justify-between">
        <span className="text-xs font-semibold truncate">{appointment.titulo}</span>
      </div>
      <div className="text-xs opacity-75 mt-1">
        {formatTime(startTime)} - {formatTime(endTime)}
      </div>
      <div className="text-xs opacity-75 mt-1">
        {TipoCompromissoLabels[appointment.tipo]}
      </div>
    </div>
  );
}
