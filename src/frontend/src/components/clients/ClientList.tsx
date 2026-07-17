'use client';

import { useState, useEffect } from 'react';
import type { Client } from '@/types/client';
import { clientApi } from '@/services/clientApi';
import { ClientCard } from './ClientCard';

interface ClientListProps {
  search?: string;
  statusFilter?: string;
}

export function ClientList({ search, statusFilter }: ClientListProps) {
  const [clients, setClients] = useState<Client[]>([]);
  const [page, setPage] = useState(1);
  const [totalCount, setTotalCount] = useState(0);
  const [isLoading, setIsLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const pageSize = 20;

  const effectivePage = (search || statusFilter) ? 1 : page;

  useEffect(() => {
    let cancelled = false;

    async function fetchClients() {
      setIsLoading(true);
      setError(null);
      try {
        const result = await clientApi.list({ page: effectivePage, pageSize, search: search || undefined, status: statusFilter || undefined });
        if (!cancelled) {
          setClients(result.items);
          setTotalCount(result.totalCount);
        }
      } catch {
        if (!cancelled) setError('Erro ao carregar clientes');
      } finally {
        if (!cancelled) setIsLoading(false);
      }
    }

    fetchClients();
    return () => { cancelled = true; };
  }, [effectivePage, pageSize, search, statusFilter]);

  if (isLoading) {
    return (
      <div className="space-y-4">
        {Array.from({ length: 5 }).map((_, i) => (
          <div key={i} className="h-20 bg-gray-200 dark:bg-gray-700 rounded-xl animate-pulse" />
        ))}
      </div>
    );
  }

  if (error) {
    return (
      <div className="bg-red-50 dark:bg-red-900/20 border border-red-200 dark:border-red-800 rounded-xl p-4">
        <p className="text-sm text-red-600 dark:text-red-400">{error}</p>
      </div>
    );
  }

  if (clients.length === 0) {
    return (
      <div className="text-center py-12 text-on-surface/60">
        <span className="material-symbols-outlined text-4xl mb-2 block">group</span>
        <p className="text-sm">Nenhum cliente encontrado</p>
      </div>
    );
  }

  const totalPages = Math.ceil(totalCount / pageSize);

  return (
    <div>
      <div className="bg-surface-container-lowest dark:bg-surface-container border border-outline-variant dark:border-outline rounded-xl overflow-hidden">
        <div className="hidden md:grid grid-cols-12 gap-4 px-6 py-3 bg-surface-container-low dark:bg-surface-container-high border-b border-outline-variant dark:border-outline">
          <div className="col-span-4 text-xs font-semibold text-on-surface/50 uppercase">Nome do Cliente</div>
          <div className="col-span-3 text-xs font-semibold text-on-surface/50 uppercase">Telefone / WhatsApp</div>
          <div className="col-span-2 text-xs font-semibold text-on-surface/50 uppercase">Status</div>
          <div className="col-span-3 text-xs font-semibold text-on-surface/50 uppercase text-right">Ações</div>
        </div>

        <div className="divide-y divide-outline-variant dark:divide-outline">
          {clients.map((client) => (
            <ClientCard key={client.id} client={client} search={search} />
          ))}
        </div>
      </div>

      {totalPages > 1 && !search && !statusFilter && (
        <div className="mt-6 flex justify-center gap-2">
          <button
            onClick={() => setPage((p) => Math.max(1, p - 1))}
            disabled={page === 1}
            className="px-4 py-2 rounded-full border border-outline-variant dark:border-outline text-sm font-medium text-on-surface/70 hover:bg-surface-container-low disabled:opacity-50 transition-colors"
          >
            Anterior
          </button>
          <span className="px-4 py-2 text-sm text-on-surface/60">
            {page} / {totalPages}
          </span>
          <button
            onClick={() => setPage((p) => Math.min(totalPages, p + 1))}
            disabled={page === totalPages}
            className="px-4 py-2 rounded-full border border-outline-variant dark:border-outline text-sm font-medium text-on-surface/70 hover:bg-surface-container-low disabled:opacity-50 transition-colors"
          >
            Próxima
          </button>
        </div>
      )}
    </div>
  );
}
