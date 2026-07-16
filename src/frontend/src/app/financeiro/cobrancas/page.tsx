'use client';

import { useState } from 'react';
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { financialApi } from '@/services/financialApi';
import { ExportModal } from '@/components/financial/ExportModal';
import { useAuthStore } from '@/stores/useAuthStore';
import { useRouter } from 'next/navigation';
import { useEffect } from 'react';
import { AppShell } from '@/components/layout/AppShell';
import { Button } from '@/components/ui/button';
import { Input } from '@/components/ui/input';

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
    <AppShell
      title="Cobranças / Faturas"
      actions={
        <div className="flex gap-2">
          <Button variant="outline" size="sm" onClick={() => setShowExport(true)}>
            Exportar
          </Button>
          <Button size="sm" onClick={() => setShowCreate(!showCreate)}>
            {showCreate ? 'Cancelar' : '+ Nova Fatura'}
          </Button>
        </div>
      }
    >
      <div className="max-w-[1440px] mx-auto">
        {showCreate && (
          <form
            onSubmit={(e) => { e.preventDefault(); createMutation.mutate(form); }}
            className="bg-card border border-border rounded-lg p-6 mb-6 space-y-4"
          >
            <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
              <Input type="number" step="0.01" placeholder="Valor" required value={form.valor} onChange={(e) => setForm({ ...form, valor: e.target.value })} />
              <Input type="date" required value={form.dataVencimento} onChange={(e) => setForm({ ...form, dataVencimento: e.target.value })} />
            </div>
            <Input type="text" placeholder="Descrição (opcional)" value={form.descricao} onChange={(e) => setForm({ ...form, descricao: e.target.value })} />
            <div className="flex justify-end gap-2">
              <Button type="button" variant="ghost" onClick={() => setShowCreate(false)}>Cancelar</Button>
              <Button type="submit" disabled={createMutation.isPending}>
                {createMutation.isPending ? 'Salvando...' : 'Salvar'}
              </Button>
            </div>
          </form>
        )}

        <div className="flex gap-3 mb-4">
          <select value={status} onChange={(e) => { setStatus(e.target.value); setPage(1); }} className="border border-border rounded-lg px-3 py-2 text-sm bg-background text-foreground">
            <option value="">Todos os status</option>
            <option value="Pending">Pendente</option>
            <option value="Partial">Parcial</option>
            <option value="Paid">Paga</option>
            <option value="Overdue">Atrasada</option>
            <option value="Cancelled">Cancelada</option>
          </select>
        </div>

        {isLoading ? (
          <p className="text-muted-foreground">Carregando...</p>
        ) : (
          <>
            <div className="overflow-x-auto">
              <table className="w-full">
                <thead>
                  <tr className="border-b border-border">
                    <th className="text-left py-3 px-4 text-xs font-semibold text-muted-foreground uppercase">Nº</th>
                    <th className="text-left py-3 px-4 text-xs font-semibold text-muted-foreground uppercase">Vencimento</th>
                    <th className="text-left py-3 px-4 text-xs font-semibold text-muted-foreground uppercase">Descrição</th>
                    <th className="text-right py-3 px-4 text-xs font-semibold text-muted-foreground uppercase">Valor</th>
                    <th className="text-right py-3 px-4 text-xs font-semibold text-muted-foreground uppercase">Pago</th>
                    <th className="text-center py-3 px-4 text-xs font-semibold text-muted-foreground uppercase">Status</th>
                    <th className="text-center py-3 px-4 text-xs font-semibold text-muted-foreground uppercase">Ações</th>
                  </tr>
                </thead>
                <tbody>
                  {data?.items.map((inv) => (
                    <tr key={inv.id} className="border-b border-border hover:bg-muted transition-colors">
                      <td className="py-3 px-4 text-sm font-mono text-foreground">{inv.numero}</td>
                      <td className="py-3 px-4 text-sm text-foreground">{new Date(inv.dataVencimento).toLocaleDateString('pt-BR')}</td>
                      <td className="py-3 px-4 text-sm text-muted-foreground">{inv.descricao || '-'}</td>
                      <td className="py-3 px-4 text-right text-sm font-mono text-foreground">{formatCurrency(inv.valor)}</td>
                      <td className="py-3 px-4 text-right text-sm font-mono text-foreground">{formatCurrency(inv.valorPago)}</td>
                      <td className="py-3 px-4 text-center">
                        <span className={`text-xs font-medium px-2 py-1 rounded-full ${statusColors[inv.status]}`}>
                          {statusLabels[inv.status]}
                        </span>
                      </td>
                      <td className="py-3 px-4 text-center">
                        {inv.status !== 'Paid' && inv.status !== 'Cancelled' && (
                          <div className="flex justify-center gap-1">
                            <button onClick={() => setShowPay(inv.id)} className="text-xs text-primary hover:underline">Pagar</button>
                            <button onClick={() => cancelMutation.mutate(inv.id)} className="text-xs text-destructive hover:underline">Cancelar</button>
                          </div>
                        )}
                      </td>
                    </tr>
                  ))}
                  {(!data?.items || data.items.length === 0) && (
                    <tr><td colSpan={7} className="py-8 text-center text-muted-foreground text-sm">Nenhuma fatura encontrada</td></tr>
                  )}
                </tbody>
              </table>
            </div>

            {showPay && (
              <div className="fixed inset-0 bg-black/50 flex items-center justify-center z-50">
                <div className="bg-card border border-border rounded-lg p-6 w-96">
                  <h3 className="text-lg font-bold text-foreground mb-4">Registrar Pagamento</h3>
                  <div className="space-y-4">
                    <Input type="number" step="0.01" placeholder="Valor pago" required value={payForm.valorPago} onChange={(e) => setPayForm({ ...payForm, valorPago: e.target.value })} />
                    <Input type="date" required value={payForm.dataPagamento} onChange={(e) => setPayForm({ ...payForm, dataPagamento: e.target.value })} />
                    <div className="flex justify-end gap-2">
                      <Button variant="ghost" onClick={() => { setShowPay(null); setPayForm({ valorPago: '', dataPagamento: '' }); }}>Cancelar</Button>
                      <Button onClick={() => showPay && payMutation.mutate({ id: showPay, f: payForm })} disabled={payMutation.isPending}>
                        Confirmar
                      </Button>
                    </div>
                  </div>
                </div>
              </div>
            )}

            {data && data.totalCount > 15 && (
              <div className="flex justify-center gap-2 mt-4">
                <Button variant="outline" size="sm" onClick={() => setPage(Math.max(1, page - 1))} disabled={page === 1}>Anterior</Button>
                <span className="px-3 py-1 text-sm text-muted-foreground">Página {page} de {Math.ceil(data.totalCount / 15)}</span>
                <Button variant="outline" size="sm" onClick={() => setPage(page + 1)} disabled={page >= Math.ceil(data.totalCount / 15)}>Próxima</Button>
              </div>
            )}
          </>
        )}
      </div>
      {showExport && <ExportModal type="invoices" onClose={() => setShowExport(false)} />}
    </AppShell>
  );
}
