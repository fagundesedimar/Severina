'use client';

import Link from 'next/link';
import type { Transaction } from '@/types/financial';

interface RecentTransactionsProps {
  transactions: Transaction[];
}

export function RecentTransactions({ transactions }: RecentTransactionsProps) {
  const formatCurrency = (value: number) =>
    new Intl.NumberFormat('pt-BR', { style: 'currency', currency: 'BRL' }).format(value);

  const tipoColors: Record<string, string> = {
    Receita: 'text-emerald-600',
    Despesa: 'text-red-600',
  };

  return (
    <div className="bg-surface-container-lowest dark:bg-surface-container border border-outline-variant dark:border-outline rounded-xl p-6">
      <div className="flex items-center justify-between mb-4">
        <h3 className="text-sm font-semibold text-on-surface">Transações Recentes</h3>
        <Link href="/financeiro/transacoes" className="text-xs text-primary hover:underline">
          Ver todas
        </Link>
      </div>
      {transactions.length === 0 ? (
        <p className="text-sm text-on-surface/50 text-center py-4">Nenhuma transação</p>
      ) : (
        <div className="space-y-3">
          {transactions.map((t) => (
            <div key={t.id} className="flex items-center justify-between py-2 border-b border-outline-variant dark:border-outline last:border-0">
              <div>
                <p className="text-sm font-medium text-on-surface">{t.descricao || t.categoria}</p>
                <p className="text-xs text-on-surface/50">{new Date(t.data).toLocaleDateString('pt-BR')}</p>
              </div>
              <span className={`text-sm font-mono font-semibold ${tipoColors[t.tipo]}`}>
                {t.tipo === 'Receita' ? '+' : '-'}{formatCurrency(t.valor)}
              </span>
            </div>
          ))}
        </div>
      )}
    </div>
  );
}
