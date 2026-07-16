'use client';

import { useState } from 'react';
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { financialApi } from '@/services/financialApi';
import { ExportModal } from '@/components/financial/ExportModal';
import { useAuthStore } from '@/store/authStore';
import { useRouter } from 'next/navigation';
import { useEffect } from 'react';

export default function CobrancasPage() {
  const { user } = useAuthStore();
  const router = useRouter();
  const queryClient = useQueryClient();
  const [status, setStatus] = useState('');
  const [page, setPage] = useState(1);
  const [showCreate, setShowCreate] = useState(false);
  const [showExport, setShowExport] = useState(false);
  const [form, setForm] = useState({ valor: '', dataVencimento: '', descricao: '', clientId: '' });
  const [showPay, setShowPay] = useState<string | null>(null);
  const [payForm, setPayForm] = useState({ valorPago: '', dataPagamento: '' });

  useEffect(() => {
    if (!user) router.push('/login');
  }, [user, router]);

  const { data, isLoading } = useQuery({
    queryKey: ['invoices', { status, page }],
    queryFn: () => financialApi.listInvoices({ status: status || undefined, page, pageSize: 15 }),
    enabled: !!user,
  });

  const createMutation = useMutation({
    mutationFn: (f: typeof form) =>
      financialApi.createInvoice({
        valor: parseFloat(f.valor),
        dataVencimento: f.dataVencimento,
        clientId: f.clientId || undefined,
        descricao: f.descricao || undefined,
      }),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['invoices'] });
      setShowCreate(false);
      setForm({ valor: '', dataVencimento: '', descricao: '', clientId: '' });
    },
  });

  const payMutation = useMutation({
    mutationFn: ({ id, f }: { id: string; f: typeof payForm }) =>
      financialApi.payInvoice(id, parseFloat(f.valorPago), f.dataPagamento),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['invoices'] });
      setShowPay(null);
      setPayForm({ valorPago: '', dataPagamento: '' });
    },
  });

  const cancelMutation = useMutation({
    mutationFn: (id: string) => financialApi.cancelInvoice(id),
    onSuccess: () => queryClient.invalidateQueries({ queryKey: ['invoices'] }),
  });

  const formatCurrency = (v: number) =>
    new Intl.NumberFormat('pt-BR', { style: 'currency', currency: 'BRL' }).format(v);

  const statusColors: Record<string, string> = {
    Pending: 'bg-amber-100 text-amber-800',
    Partial: 'bg-blue-100 text-blue-800',
    Paid: 'bg-emerald-100 text-emerald-800',
    Overdue: 'bg-red-100 text-red-800',
    Cancelled: 'bg-gray-100 text-gray-500',
  };

  const statusLabels: Record<string, string> = {
    Pending: 'Pendente',
    Partial: 'Parcial',
    Paid: 'Paga',
    Overdue: 'Atrasada',
    Cancelled: 'Cancelada',
  };

  if (!user) return null;

  return (
    <div className="min-h-screen bg-surface dark:bg-dark-surface px-6 py-8">
      <div className="flex items-center justify-between mb-6">
        <h1 className="text-2xl font-bold text-on-surface">Cobranças / Faturas</h1>
        <div className="flex gap-2">
          <button onClick={() => setShowExport(true)} className="border border-outline-variant dark:border-outline text-on-surface px-4 py-2 rounded-lg text-sm font-semibold hover:bg-surface-container-lowest dark:hover:bg-surface-container transition-colors">
            Exportar
          </button>
          <button onClick={() => setShowCreate(!showCreate)} className="bg-primary text-on-primary px-4 py-2 rounded-lg text-sm font-semibold hover:bg-primary/90 transition-colors">
            {showCreate ? 'Cancelar' : '+ Nova Fatura'}
          </button>
        </div>
      </div>

      {showCreate && (
        <form
          onSubmit={(e) => { e.preventDefault(); createMutation.mutate(form); }}
          className="bg-surface-container-lowest dark:bg-surface-container border border-outline-variant dark:border-outline rounded-xl p-6 mb-6 space-y-4"
        >
          <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
            <input type="number" step="0.01" placeholder="Valor" required value={form.valor} onChange={(e) => setForm({ ...form, valor: e.target.value })} className="border border-outline-variant dark:border-outline rounded-lg px-3 py-2 text-sm bg-surface dark:bg-dark-surface text-on-surface" />
            <input type="date" required value={form.dataVencimento} onChange={(e) => setForm({ ...form, dataVencimento: e.target.value })} className="border border-outline-variant dark:border-outline rounded-lg px-3 py-2 text-sm bg-surface dark:bg-dark-surface text-on-surface" />
          </div>
          <input type="text" placeholder="Descrição (opcional)" value={form.descricao} onChange={(e) => setForm({ ...form, descricao: e.target.value })} className="w-full border border-outline-variant dark:border-outline rounded-lg px-3 py-2 text-sm bg-surface dark:bg-dark-surface text-on-surface" />
          <div className="flex justify-end gap-2">
            <button type="button" onClick={() => setShowCreate(false)} className="px-4 py-2 text-sm text-on-surface/60 hover:text-on-surface">Cancelar</button>
            <button type="submit" disabled={createMutation.isPending} className="bg-primary text-on-primary px-6 py-2 rounded-lg text-sm font-semibold hover:bg-primary/90 transition-colors disabled:opacity-50">
              {createMutation.isPending ? 'Salvando...' : 'Salvar'}
            </button>
          </div>
        </form>
      )}

      <div className="flex gap-3 mb-4">
        <select value={status} onChange={(e) => { setStatus(e.target.value); setPage(1); }} className="border border-outline-variant dark:border-outline rounded-lg px-3 py-2 text-sm bg-surface dark:bg-dark-surface text-on-surface">
          <option value="">Todos os status</option>
          <option value="Pending">Pendente</option>
          <option value="Partial">Parcial</option>
          <option value="Paid">Paga</option>
          <option value="Overdue">Atrasada</option>
          <option value="Cancelled">Cancelada</option>
        </select>
      </div>

      {isLoading ? (
        <p className="text-on-surface/50">Carregando...</p>
      ) : (
        <>
          <div className="overflow-x-auto">
            <table className="w-full">
              <thead>
                <tr className="border-b border-outline-variant dark:border-outline">
                  <th className="text-left py-3 px-4 text-xs font-semibold text-on-surface/50 uppercase">Nº</th>
                  <th className="text-left py-3 px-4 text-xs font-semibold text-on-surface/50 uppercase">Vencimento</th>
                  <th className="text-left py-3 px-4 text-xs font-semibold text-on-surface/50 uppercase">Descrição</th>
                  <th className="text-right py-3 px-4 text-xs font-semibold text-on-surface/50 uppercase">Valor</th>
                  <th className="text-right py-3 px-4 text-xs font-semibold text-on-surface/50 uppercase">Pago</th>
                  <th className="text-center py-3 px-4 text-xs font-semibold text-on-surface/50 uppercase">Status</th>
                  <th className="text-center py-3 px-4 text-xs font-semibold text-on-surface/50 uppercase">Ações</th>
                </tr>
              </thead>
              <tbody>
                {data?.items.map((inv) => (
                  <tr key={inv.id} className="border-b border-outline-variant dark:border-outline hover:bg-surface-container-lowest dark:hover:bg-surface-container transition-colors">
                    <td className="py-3 px-4 text-sm font-mono text-on-surface">{inv.numero}</td>
                    <td className="py-3 px-4 text-sm text-on-surface">{new Date(inv.dataVencimento).toLocaleDateString('pt-BR')}</td>
                    <td className="py-3 px-4 text-sm text-on-surface/70">{inv.descricao || '-'}</td>
                    <td className="py-3 px-4 text-right text-sm font-mono text-on-surface">{formatCurrency(inv.valor)}</td>
                    <td className="py-3 px-4 text-right text-sm font-mono text-on-surface">{formatCurrency(inv.valorPago)}</td>
                    <td className="py-3 px-4 text-center">
                      <span className={`text-xs font-medium px-2 py-1 rounded-full ${statusColors[inv.status]}`}>
                        {statusLabels[inv.status]}
                      </span>
                    </td>
                    <td className="py-3 px-4 text-center">
                      {inv.status !== 'Paid' && inv.status !== 'Cancelled' && (
                        <div className="flex justify-center gap-1">
                          <button onClick={() => setShowPay(inv.id)} className="text-xs text-emerald-600 hover:underline">Pagar</button>
                          <button onClick={() => cancelMutation.mutate(inv.id)} className="text-xs text-red-600 hover:underline">Cancelar</button>
                        </div>
                      )}
                    </td>
                  </tr>
                ))}
                {(!data?.items || data.items.length === 0) && (
                  <tr><td colSpan={7} className="py-8 text-center text-on-surface/50 text-sm">Nenhuma fatura encontrada</td></tr>
                )}
              </tbody>
            </table>
          </div>

          {showPay && (
            <div className="fixed inset-0 bg-black/50 flex items-center justify-center z-50">
              <div className="bg-surface-container-lowest dark:bg-surface-container border border-outline-variant dark:border-outline rounded-xl p-6 w-96">
                <h3 className="text-lg font-bold text-on-surface mb-4">Registrar Pagamento</h3>
                <div className="space-y-4">
                  <input type="number" step="0.01" placeholder="Valor pago" required value={payForm.valorPago} onChange={(e) => setPayForm({ ...payForm, valorPago: e.target.value })} className="w-full border border-outline-variant dark:border-outline rounded-lg px-3 py-2 text-sm bg-surface dark:bg-dark-surface text-on-surface" />
                  <input type="date" required value={payForm.dataPagamento} onChange={(e) => setPayForm({ ...payForm, dataPagamento: e.target.value })} className="w-full border border-outline-variant dark:border-outline rounded-lg px-3 py-2 text-sm bg-surface dark:bg-dark-surface text-on-surface" />
                  <div className="flex justify-end gap-2">
                    <button onClick={() => { setShowPay(null); setPayForm({ valorPago: '', dataPagamento: '' }); }} className="px-4 py-2 text-sm text-on-surface/60 hover:text-on-surface">Cancelar</button>
                    <button onClick={() => showPay && payMutation.mutate({ id: showPay, f: payForm })} disabled={payMutation.isPending} className="bg-emerald-600 text-white px-6 py-2 rounded-lg text-sm font-semibold hover:bg-emerald-700 transition-colors disabled:opacity-50">
                      Confirmar
                    </button>
                  </div>
                </div>
              </div>
            </div>
          )}

          {data && data.totalCount > 15 && (
            <div className="flex justify-center gap-2 mt-4">
              <button onClick={() => setPage(Math.max(1, page - 1))} disabled={page === 1} className="px-3 py-1 text-sm border border-outline-variant dark:border-outline rounded-lg disabled:opacity-50 text-on-surface hover:bg-surface-container-lowest dark:hover:bg-surface-container">Anterior</button>
              <span className="px-3 py-1 text-sm text-on-surface/50">Página {page} de {Math.ceil(data.totalCount / 15)}</span>
              <button onClick={() => setPage(page + 1)} disabled={page >= Math.ceil(data.totalCount / 15)} className="px-3 py-1 text-sm border border-outline-variant dark:border-outline rounded-lg disabled:opacity-50 text-on-surface hover:bg-surface-container-lowest dark:hover:bg-surface-container">Próxima</button>
            </div>
          )}
        </>
      )}
      {showExport && <ExportModal type="invoices" onClose={() => setShowExport(false)} />}
    </div>
  );
}
