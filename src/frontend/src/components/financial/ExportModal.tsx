'use client';

import { useState } from 'react';
import { useMutation } from '@tanstack/react-query';

interface ExportModalProps {
  type: 'transactions' | 'invoices';
  onClose: () => void;
}

export function ExportModal({ type, onClose }: ExportModalProps) {
  const [format, setFormat] = useState<'csv' | 'xlsx'>('csv');
  const [from, setFrom] = useState('');
  const [to, setTo] = useState('');
  const [statusFilter, setStatusFilter] = useState('');

  const exportMutation = useMutation({
    mutationFn: async () => {
      const params = new URLSearchParams({ format });
      if (from) params.set('from', from);
      if (to) params.set('to', to);
      if (statusFilter) params.set('status', statusFilter);

      const url = type === 'transactions'
        ? `/api/v1/export/transactions?${params}`
        : `/api/v1/export/invoices?${params}`;

      const response = await fetch(url, {
        method: 'POST',
        headers: {
          'Authorization': `Bearer ${localStorage.getItem('token')}`,
        },
      });

      if (!response.ok) throw new Error('Export failed');

      const blob = await response.blob();
      const disposition = response.headers.get('content-disposition');
      const fileName = disposition?.match(/filename=(.+)/)?.[1] || `export.${format}`;

      const link = document.createElement('a');
      link.href = URL.createObjectURL(blob);
      link.download = fileName;
      link.click();
      URL.revokeObjectURL(link.href);
    },
    onSuccess: () => onClose(),
  });

  return (
    <div className="fixed inset-0 bg-black/50 flex items-center justify-center z-50">
      <div className="bg-surface-container-lowest dark:bg-surface-container border border-outline-variant dark:border-outline rounded-xl p-6 w-96">
        <h3 className="text-lg font-bold text-on-surface mb-4">
          Exportar {type === 'transactions' ? 'Transações' : 'Faturas'}
        </h3>
        <div className="space-y-4">
          <div>
            <label className="block text-xs font-semibold text-on-surface/50 mb-1">Formato</label>
            <select value={format} onChange={(e) => setFormat(e.target.value as 'csv' | 'xlsx')} className="w-full border border-outline-variant dark:border-outline rounded-lg px-3 py-2 text-sm bg-surface dark:bg-dark-surface text-on-surface">
              <option value="csv">CSV</option>
              <option value="xlsx">XLSX (Excel)</option>
            </select>
          </div>

          {type === 'transactions' && (
            <>
              <div>
                <label className="block text-xs font-semibold text-on-surface/50 mb-1">Data inicial</label>
                <input type="date" value={from} onChange={(e) => setFrom(e.target.value)} className="w-full border border-outline-variant dark:border-outline rounded-lg px-3 py-2 text-sm bg-surface dark:bg-dark-surface text-on-surface" />
              </div>
              <div>
                <label className="block text-xs font-semibold text-on-surface/50 mb-1">Data final</label>
                <input type="date" value={to} onChange={(e) => setTo(e.target.value)} className="w-full border border-outline-variant dark:border-outline rounded-lg px-3 py-2 text-sm bg-surface dark:bg-dark-surface text-on-surface" />
              </div>
            </>
          )}

          {type === 'invoices' && (
            <div>
              <label className="block text-xs font-semibold text-on-surface/50 mb-1">Status</label>
              <select value={statusFilter} onChange={(e) => setStatusFilter(e.target.value)} className="w-full border border-outline-variant dark:border-outline rounded-lg px-3 py-2 text-sm bg-surface dark:bg-dark-surface text-on-surface">
                <option value="">Todos</option>
                <option value="Pending">Pendente</option>
                <option value="Partial">Parcial</option>
                <option value="Paid">Paga</option>
                <option value="Overdue">Atrasada</option>
                <option value="Cancelled">Cancelada</option>
              </select>
            </div>
          )}

          <div className="flex justify-end gap-2 pt-2">
            <button onClick={onClose} className="px-4 py-2 text-sm text-on-surface/60 hover:text-on-surface">
              Cancelar
            </button>
            <button
              onClick={() => exportMutation.mutate()}
              disabled={exportMutation.isPending}
              className="bg-primary text-on-primary px-6 py-2 rounded-lg text-sm font-semibold hover:bg-primary/90 transition-colors disabled:opacity-50"
            >
              {exportMutation.isPending ? 'Exportando...' : 'Exportar'}
            </button>
          </div>
          {exportMutation.isError && (
            <p className="text-xs text-red-500">Erro ao exportar. Tente novamente.</p>
          )}
        </div>
      </div>
    </div>
  );
}
