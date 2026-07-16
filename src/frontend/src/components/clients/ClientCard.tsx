'use client';

import { useState } from 'react';
import Link from 'next/link';
import type { Client } from '@/types/client';
import { clientApi } from '@/services/clientApi';
import { highlightText } from '@/utils/highlight';

interface ClientCardProps {
  client: Client;
  search?: string;
}

export function ClientCard({ client, search }: ClientCardProps) {
  const [showTimeline, setShowTimeline] = useState(false);
  const [interactions, setInteractions] = useState<{ content: string; type: string; createdAt: string }[]>([]);
  const [loadingTimeline, setLoadingTimeline] = useState(false);

  const initials = client.nome
    .split(' ')
    .map((n) => n[0])
    .slice(0, 2)
    .join('')
    .toUpperCase();

  const toggleTimeline = async () => {
    if (showTimeline) {
      setShowTimeline(false);
      return;
    }

    setLoadingTimeline(true);
    try {
      const result = await clientApi.listInteractions(client.id, { pageSize: 5 });
      setInteractions(result.items);
    } catch {
      setInteractions([]);
    } finally {
      setLoadingTimeline(false);
      setShowTimeline(true);
    }
  };

  const statusColor =
    client.status === 'Ativo'
      ? 'bg-emerald-50 dark:bg-emerald-900/30 text-emerald-600 dark:text-emerald-400'
      : 'bg-red-50 dark:bg-red-900/30 text-red-600 dark:text-red-400';

  return (
    <div
      className="grid grid-cols-1 md:grid-cols-12 gap-4 px-4 md:px-6 py-4 hover:bg-surface-container-low dark:hover:bg-surface-variant transition-colors cursor-pointer group"
      onClick={toggleTimeline}
    >
      <div className="col-span-4 flex items-center gap-3">
        <div className="w-10 h-10 rounded-full bg-primary-container dark:bg-primary-fixed-dim flex items-center justify-center text-on-primary-container dark:text-on-primary-fixed font-semibold text-sm">
          {initials}
        </div>
        <div>
          <Link
            href={`/clientes/${client.id}`}
            className="font-semibold text-on-surface hover:text-primary transition-colors"
            onClick={(e) => e.stopPropagation()}
          >
            {highlightText(client.nome, search)}
          </Link>
          {client.empresa && (
            <p className="text-xs text-on-surface/50">{highlightText(client.empresa, search)}</p>
          )}
        </div>
      </div>

      <div className="col-span-3 flex items-center gap-2">
        {client.telefone && (
          <>
            <span className="material-symbols-outlined text-emerald-500 text-lg">chat</span>
            <p className="text-sm text-on-surface/70">{client.telefone}</p>
          </>
        )}
      </div>

      <div className="col-span-2 flex items-center">
        <span className={`px-3 py-1 rounded-full text-xs font-medium ${statusColor}`}>
          {client.status}
        </span>
      </div>

      <div className="col-span-3 flex items-center justify-end gap-2">
        <Link
          href={`/clientes/${client.id}`}
          onClick={(e) => e.stopPropagation()}
          className="p-2 rounded-lg hover:bg-surface-container-highest dark:hover:bg-surface-container-high transition-colors text-primary"
        >
          <span className="material-symbols-outlined">history</span>
        </Link>
        <button
          onClick={(e) => {
            e.stopPropagation();
            clientApi.remove(client.id).then(() => window.location.reload());
          }}
          className="p-2 rounded-lg hover:bg-surface-container-highest dark:hover:bg-surface-container-high transition-colors text-on-surface/40 hover:text-red-500"
        >
          <span className="material-symbols-outlined">delete</span>
        </button>
      </div>

      {showTimeline && (
        <div className="col-span-full mt-4 pt-4 border-t border-dashed border-outline-variant dark:border-outline">
          {loadingTimeline ? (
            <div className="h-16 bg-gray-200 dark:bg-gray-700 rounded animate-pulse" />
          ) : interactions.length > 0 ? (
            <div className="relative border-l-2 border-primary ml-2 pl-6 space-y-4">
              {interactions.map((interaction, i) => (
                <div key={i} className="relative">
                  <span className="absolute -left-[31px] top-0 w-4 h-4 rounded-full bg-primary border-4 border-surface dark:border-inverse-surface" />
                  <p className="text-xs font-semibold text-primary">
                    {new Date(interaction.createdAt).toLocaleDateString('pt-BR')}
                  </p>
                  <p className="text-sm text-on-surface/70">{interaction.content}</p>
                </div>
              ))}
            </div>
          ) : (
            <p className="text-sm text-on-surface/50 italic">Nenhuma interação recente encontrada.</p>
          )}
        </div>
      )}
    </div>
  );
}
