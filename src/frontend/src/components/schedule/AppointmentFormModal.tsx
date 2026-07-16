'use client';

import { useState, useMemo } from 'react';
import { useMutation, useQueryClient } from '@tanstack/react-query';
import { appointmentApi } from '@/services/appointmentApi';
import type { Appointment, CreateAppointmentRequest, TipoCompromisso } from '@/types/appointment';
import { TipoCompromissoLabels } from '@/types/appointment';

interface AppointmentFormModalProps {
  isOpen: boolean;
  onClose: () => void;
  appointment?: Appointment;
  defaultDate?: Date;
}

export default function AppointmentFormModal({
  isOpen,
  onClose,
  appointment,
  defaultDate,
}: AppointmentFormModalProps) {
  const queryClient = useQueryClient();
  const isEditing = !!appointment;

  const initialFormData = useMemo(() => {
    if (appointment) {
      const start = new Date(appointment.dataHoraInicio);
      const end = new Date(appointment.dataHoraFim);
      return {
        titulo: appointment.titulo,
        descricao: appointment.descricao || '',
        dataHoraInicio: formatDateTimeLocal(start),
        dataHoraFim: formatDateTimeLocal(end),
        tipo: appointment.tipo,
        clientId: appointment.clientId || '',
        recurrenceTipo: 'none',
        recurrenceIntervalo: 1,
        recurrenceDiasDaSemana: [] as number[],
        recurrenceFimTipo: 'date',
        recurrenceDataFim: '',
        recurrenceContagemFim: 10,
      };
    }
    if (defaultDate) {
      const start = new Date(defaultDate);
      const end = new Date(defaultDate);
      end.setHours(end.getHours() + 1);
      return {
        titulo: '',
        descricao: '',
        dataHoraInicio: formatDateTimeLocal(start),
        dataHoraFim: formatDateTimeLocal(end),
        tipo: 0 as TipoCompromisso,
        clientId: '',
        recurrenceTipo: 'none',
        recurrenceIntervalo: 1,
        recurrenceDiasDaSemana: [] as number[],
        recurrenceFimTipo: 'date',
        recurrenceDataFim: '',
        recurrenceContagemFim: 10,
      };
    }
    return {
      titulo: '',
      descricao: '',
      dataHoraInicio: '',
      dataHoraFim: '',
      tipo: 0 as TipoCompromisso,
      clientId: '',
      recurrenceTipo: 'none',
      recurrenceIntervalo: 1,
      recurrenceDiasDaSemana: [] as number[],
      recurrenceFimTipo: 'date',
      recurrenceDataFim: '',
      recurrenceContagemFim: 10,
    };
  }, [appointment, defaultDate]);

  const [formData, setFormData] = useState(initialFormData);

  const createMutation = useMutation({
    mutationFn: (data: CreateAppointmentRequest) => appointmentApi.create(data),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['appointments'] });
      onClose();
    },
  });

  const updateMutation = useMutation({
    mutationFn: (data: CreateAppointmentRequest) =>
      appointmentApi.update(appointment!.id, data),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['appointments'] });
      onClose();
    },
  });

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault();

    const data: CreateAppointmentRequest = {
      titulo: formData.titulo,
      descricao: formData.descricao || undefined,
      dataHoraInicio: new Date(formData.dataHoraInicio).toISOString(),
      dataHoraFim: new Date(formData.dataHoraFim).toISOString(),
      tipo: formData.tipo,
      clientId: formData.clientId || undefined,
    };

    if (isEditing) {
      updateMutation.mutate(data);
    } else {
      createMutation.mutate(data);
    }
  };

  const handleRecurrenceChange = (tipo: string) => {
    setFormData((prev) => ({ ...prev, recurrenceTipo: tipo }));
  };

  const toggleDayOfWeek = (day: number) => {
    setFormData((prev) => ({
      ...prev,
      recurrenceDiasDaSemana: prev.recurrenceDiasDaSemana.includes(day)
        ? prev.recurrenceDiasDaSemana.filter((d) => d !== day)
        : [...prev.recurrenceDiasDaSemana, day],
    }));
  };

  if (!isOpen) return null;

  return (
    <div className="fixed inset-0 bg-black/50 flex items-center justify-center z-50 p-4">
      <div className="bg-white dark:bg-gray-800 rounded-xl w-full max-w-md max-h-[90vh] overflow-y-auto">
        <div className="p-6">
          <div className="flex justify-between items-center mb-6">
            <h2 className="text-xl font-bold">
              {isEditing ? 'Editar Compromisso' : 'Novo Compromisso'}
            </h2>
            <button
              onClick={onClose}
              className="text-gray-500 hover:text-gray-700 text-2xl"
            >
              ×
            </button>
          </div>

          <form key={`${appointment?.id || 'new'}-${defaultDate?.toISOString() || ''}`} onSubmit={handleSubmit} className="space-y-4">
            <div>
              <label className="block text-sm font-medium mb-1">Título *</label>
              <input
                type="text"
                value={formData.titulo}
                onChange={(e) => setFormData({ ...formData, titulo: e.target.value })}
                className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-primary"
                required
              />
            </div>

            <div>
              <label className="block text-sm font-medium mb-1">Descrição</label>
              <textarea
                value={formData.descricao}
                onChange={(e) => setFormData({ ...formData, descricao: e.target.value })}
                className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-primary"
                rows={3}
              />
            </div>

            <div className="grid grid-cols-2 gap-4">
              <div>
                <label className="block text-sm font-medium mb-1">Início *</label>
                <input
                  type="datetime-local"
                  value={formData.dataHoraInicio}
                  onChange={(e) => setFormData({ ...formData, dataHoraInicio: e.target.value })}
                  className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-primary"
                  required
                />
              </div>
              <div>
                <label className="block text-sm font-medium mb-1">Fim *</label>
                <input
                  type="datetime-local"
                  value={formData.dataHoraFim}
                  onChange={(e) => setFormData({ ...formData, dataHoraFim: e.target.value })}
                  className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-primary"
                  required
                />
              </div>
            </div>

            <div>
              <label className="block text-sm font-medium mb-1">Tipo</label>
              <select
                value={formData.tipo}
                onChange={(e) => setFormData({ ...formData, tipo: Number(e.target.value) as TipoCompromisso })}
                className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-primary"
              >
                {Object.entries(TipoCompromissoLabels).map(([value, label]) => (
                  <option key={value} value={value}>
                    {label}
                  </option>
                ))}
              </select>
            </div>

            <div>
              <label className="block text-sm font-medium mb-1">Recorrência</label>
              <select
                value={formData.recurrenceTipo}
                onChange={(e) => handleRecurrenceChange(e.target.value)}
                className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-primary"
              >
                <option value="none">Nenhum</option>
                <option value="daily">Diário</option>
                <option value="weekly">Semanal</option>
                <option value="monthly">Mensal</option>
                <option value="custom">Custom</option>
              </select>
            </div>

            {formData.recurrenceTipo === 'weekly' && (
              <div>
                <label className="block text-sm font-medium mb-2">Dias da semana</label>
                <div className="flex gap-2">
                  {['DOM', 'SEG', 'TER', 'QUA', 'QUI', 'SEX', 'SAB'].map((day, index) => (
                    <button
                      key={day}
                      type="button"
                      onClick={() => toggleDayOfWeek(index)}
                      className={`w-10 h-10 rounded-lg text-sm font-medium transition-colors ${
                        formData.recurrenceDiasDaSemana.includes(index)
                          ? 'bg-primary text-on-primary'
                          : 'bg-gray-100 text-gray-600 hover:bg-gray-200'
                      }`}
                    >
                      {day}
                    </button>
                  ))}
                </div>
              </div>
            )}

            {formData.recurrenceTipo !== 'none' && (
              <div>
                <label className="block text-sm font-medium mb-1">Tipo de término</label>
                <select
                  value={formData.recurrenceFimTipo}
                  onChange={(e) => setFormData({ ...formData, recurrenceFimTipo: e.target.value })}
                  className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-primary"
                >
                  <option value="date">Data</option>
                  <option value="count">Contagem</option>
                </select>
              </div>
            )}

            {formData.recurrenceTipo !== 'none' && formData.recurrenceFimTipo === 'date' && (
              <div>
                <label className="block text-sm font-medium mb-1">Data de término</label>
                <input
                  type="date"
                  value={formData.recurrenceDataFim}
                  onChange={(e) => setFormData({ ...formData, recurrenceDataFim: e.target.value })}
                  className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-primary"
                />
              </div>
            )}

            {formData.recurrenceTipo !== 'none' && formData.recurrenceFimTipo === 'count' && (
              <div>
                <label className="block text-sm font-medium mb-1">Número de ocorrências</label>
                <input
                  type="number"
                  min="1"
                  value={formData.recurrenceContagemFim}
                  onChange={(e) => setFormData({ ...formData, recurrenceContagemFim: Number(e.target.value) })}
                  className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-primary"
                />
              </div>
            )}

            <div className="flex gap-3 pt-4">
              <button
                type="button"
                onClick={onClose}
                className="flex-1 px-4 py-2 border border-gray-300 rounded-lg hover:bg-gray-50 transition-colors"
              >
                Cancelar
              </button>
              <button
                type="submit"
                disabled={createMutation.isPending || updateMutation.isPending}
                className="flex-1 px-4 py-2 bg-primary text-on-primary rounded-lg hover:bg-primary/90 transition-colors disabled:opacity-50"
              >
                {createMutation.isPending || updateMutation.isPending
                  ? 'Salvando...'
                  : isEditing
                  ? 'Salvar'
                  : 'Criar'}
              </button>
            </div>
          </form>
        </div>
      </div>
    </div>
  );
}

function formatDateTimeLocal(date: Date): string {
  const year = date.getFullYear();
  const month = String(date.getMonth() + 1).padStart(2, '0');
  const day = String(date.getDate()).padStart(2, '0');
  const hours = String(date.getHours()).padStart(2, '0');
  const minutes = String(date.getMinutes()).padStart(2, '0');
  return `${year}-${month}-${day}T${hours}:${minutes}`;
}
